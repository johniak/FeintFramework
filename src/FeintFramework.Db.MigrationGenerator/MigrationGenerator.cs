using System.Reflection;
using FeintFramework.Core.Apps;
using FeintFramework.Core.Config.Settings;
using FeintFramework.Db.Migrator;
using FeintFramework.Db.Migrator.Fields;
using FeintFramework.Db.Migrator.Operations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using QuikGraph;
using QuikGraph.Algorithms;

namespace FeintFramework.Db.MigrationGenerator;
public class MigrationGenerator
{
    DatabaseState databaseState;
    MigrationHelper migrationHelper;
    Dictionary<string, (ExpressionSyntax OperationExpression, string[] Dependencies)?> operationsDependenciesDict = new Dictionary<string, (ExpressionSyntax OperationExpression, string[] Dependencies)?>();
    Type[] appTypes;

    Dictionary<string, string> migrationsPath = new Dictionary<string, string>();
    public MigrationGenerator(Type[] appTypes, string projectDirectory)
    {
        this.appTypes = appTypes;
        migrationHelper = new MigrationHelper(appTypes);
        databaseState = new DatabaseState(migrationHelper.CreateAllMigrationInstancesWithAppInstances());
        var installedAppWithPaths = appTypes.Select(a => (a.Name, a.FullName)).ToDictionary(a => a.Name, a => SourceFileSearcher.FindSourceFileForType(projectDirectory, a.FullName!));
        foreach (var app in installedAppWithPaths)
        {
            var baseDirectory = Path.GetDirectoryName(app.Value);
            string migrationDirectory = Path.Combine(baseDirectory!, "migrations");
            migrationsPath[app.Key] = migrationDirectory;
        }
    }

    public void GenerateMigration()
    {
        var installedModels = migrationHelper.GetAllModelTypesWithAppTypes();
        var modelsGroupedByApp = installedModels.GroupBy(m => m.AppicationType);
        var installedAppsByName = appTypes.ToDictionary(a => a.Name, a => (BaseApplication)Activator.CreateInstance(a)!);
        var existingModels = databaseState.GetExistingModels();
        foreach (var model in existingModels)
        {
            operationsDependenciesDict[model] = null;
        }
        foreach (var groupedByModel in modelsGroupedByApp)
        {
            var appType = groupedByModel.Key;
            var models = groupedByModel.Select(m => m.ModelType);
            var appInstance = (BaseApplication)Activator.CreateInstance(appType)!;
            foreach (var model in models)
            {
                generateModelMigration(appInstance, model);
            }
        }
        var sortedOperations = getOperationsGraphSorted();
        var currentMigrationsNumbers = migrationHelper.GetCurrentMigrationNumbers();

        var migrations = new List<(string AppName, ClassGenerator MigrationClassGenerator)>();
        string? currentAppName = null;
        ClassGenerator? classGenerator = null;
        foreach (var operation in sortedOperations)
        {
            if (currentAppName != operation.AppName)
            {
                currentAppName = operation.AppName;
                var lastMigrationNumber = currentMigrationsNumbers[operation.AppName];
                var initial = lastMigrationNumber == 0;
                var migrationClassName = $"_{(lastMigrationNumber + 1).ToString("D4")}_migration";
                classGenerator = new ClassGenerator(installedAppsByName[currentAppName], migrationClassName);
                migrations.Add((currentAppName, classGenerator));
                if (!initial)
                {
                    classGenerator.AddDependency(currentAppName, $"_{(lastMigrationNumber).ToString("D4")}_migration");
                }
                currentMigrationsNumbers[operation.AppName] = lastMigrationNumber + 1;
            }
            if (classGenerator == null)
            {
                throw new Exception("Class generator is null");
            }
            classGenerator.AddOperation(operation.Expression);
            foreach (var dependency in operation.Dependencies)
            {
                var dependencyAppName = dependency.Split('|')[0];
                var dependencyMigrationName = $"_{currentMigrationsNumbers[dependencyAppName].ToString("D4")}_migration";
                classGenerator.AddDependency(dependencyAppName, dependencyMigrationName);
            }
        }
        foreach (var migration in migrations)
        {
            saveMigration(migration.AppName, migration.MigrationClassGenerator.ClassName, migration.MigrationClassGenerator.ToString());
        }
    }

    protected void generateModelMigration(BaseApplication appInstance, Type model)
    {

        var modelState = databaseState.GetModelState(appInstance.Name, model.Name);
        if (modelState == null)
        {
            var operation = new CreateModel(model.Name);
            var fields = getModelFields(model);
            var dependencies = getOperationDependeciesFromFields(fields.Select(f => f.Field));
            operation.Fields = fields;
            var operationExpression = operation.GenerateOperation();
            var key = $"{appInstance.Name}|{model.Name}";
            operationsDependenciesDict[key] = (operationExpression, dependencies.Select(d => $"{d.AppName}|{d.Model}").ToArray());
            return;
        }

        var currentFields = getModelFields(model);
        var deletedFields = modelState?.Fields.Where(f => !currentFields.Any(cf => cf.FieldName == f.Key));
        foreach (var field in currentFields)
        {
            var fieldName = field.FieldName;
            var currentField = field.Field;
            var fieldSnapshot = modelState?.Fields.FirstOrDefault(f => f.Key == fieldName).Value;
            if (fieldSnapshot == null)
            {
                var operation = new AddField(model.Name, fieldName)
                {
                    Field = currentField
                };
                var dependencies = getOperationDependeciesFromFields([currentField]);
                var key = $"{appInstance.Name}|{model.Name}|{fieldName}";
                var operationExpression = operation.GenerateOperation();
                operationsDependenciesDict[key] = (operationExpression, dependencies.Select(d => $"{d.AppName}|{d.Model}").ToArray());
            }
            else if (fieldSnapshot != currentField)
            {
                var operation = new AlterField(model.Name, fieldName)
                {
                    Field = currentField
                };
                var dependencies = getOperationDependeciesFromFields([currentField]);
                var key = $"{appInstance.Name}|{model.Name}|{fieldName}";
                var operationExpression = operation.GenerateOperation();
                operationsDependenciesDict[key] = (operationExpression, dependencies.Select(d => $"{d.AppName}|{d.Model}").ToArray());
            }
        }
        foreach (var field in deletedFields!)
        {
            var fieldName = field.Key;
            var operation = new RemoveField(model.Name, fieldName);
            var dependencies = getOperationDependeciesFromFields([field.Value]);
            var key = $"{appInstance.Name}|{model.Name}|{fieldName}";
            var operationExpression = operation.GenerateOperation();
            operationsDependenciesDict[key] = (operationExpression, dependencies.Select(d => $"{d.AppName}|{d.Model}").ToArray());
        }
    }

    protected List<(string AppName, string Model, ExpressionSyntax Expression, string[] Dependencies)> getOperationsGraphSorted()
    {
        List<(string AppName, string Model, ExpressionSyntax Expression, string[] Dependencies)> fullOperationsInfo = new List<(string AppName, string Model, ExpressionSyntax expression, string[] Dependencies)>();
        var graph = new AdjacencyGraph<string, Edge<string>>();
        foreach (var key in operationsDependenciesDict.Keys.OrderBy(k => k))
        {
            graph.AddVertex(key);
        }
        foreach (var kvp in operationsDependenciesDict)
        {
            if (kvp.Value == null)
                continue;
            foreach (var dependency in kvp.Value.Value.Dependencies)
            {
                graph.AddEdge(new Edge<string>(dependency, kvp.Key));
            }
        }
        IEnumerable<string> sortedKeys;
        try
        {
            sortedKeys = graph.TopologicalSort();
        }
        catch (Exception ex)
        {
            throw new Exception("Cycle detected in migration dependencies.", ex);
        }
        foreach (var key in sortedKeys)
        {
            if (operationsDependenciesDict[key] == null)
                continue;
            var operation = operationsDependenciesDict[key]!.Value;
            var appName = key.Split('|')[0];
            var modelName = key.Split('|')[1];
            fullOperationsInfo.Add((appName, modelName, operation.OperationExpression, operation.Dependencies.Where(x => x.Split('|')[0] != appName).ToArray()));
        }
        return fullOperationsInfo;
    }

    protected List<(String AppName, string Model)> getOperationDependeciesFromFields(IEnumerable<BaseField> fields)
    {
        var dependencies = new List<(string AppName, string Migration)>();
        foreach (var field in fields)
        {
            if (field is ForeignKey fkField)
            {
                var appName = fkField.To.Split('.')[0];
                var modelName = fkField.To.Split('.')[1];
                dependencies.Add((appName, modelName));
            }
        }
        return dependencies;
    }


    protected (string FieldName, BaseField Field)[] getModelFields(Type modelType)
    {
        var fields = modelType.GetProperties();
        var fieldList = new List<(string FieldName, BaseField Field)>();
        foreach (var field in fields)
        {
            var baseField = field.GetCustomAttribute<BaseField>();
            if (baseField != null)
            {
                fieldList.Add((field.Name, baseField));
            }
        }
        return fieldList.ToArray();
    }

    protected void saveMigration(string appName, string migrationName, string migrationContent)
    {
        var migrationPath = migrationsPath[appName];
        Directory.CreateDirectory(migrationPath);
        var migrationFilePath = Path.Combine(migrationPath, $"{migrationName}.g.cs");
        File.WriteAllText(migrationFilePath, migrationContent);
    }
}
