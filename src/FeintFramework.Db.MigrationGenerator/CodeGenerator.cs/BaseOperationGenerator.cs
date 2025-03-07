

using FeintFramework.Db.Migrator.Operations;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace FeintFramework.Db.MigrationGenerator;

public abstract class BaseOperationGenerator<T> where T : MigrationOperation{

    public T Operation { get; protected set; }

    public BaseOperationGenerator(T operation)
    {
        Operation = operation;
    }

    public abstract ExpressionSyntax GenerateOperation(); 

    public abstract string[] GetApplicationDependencies();
}

public abstract class FieldOperationGenerator<T> where T : FieldOperation{

    public T Operation { get; protected set; }

    public FieldOperationGenerator(T operation)
    {
        Operation = operation;
    }

    public abstract ExpressionSyntax GenerateOperation(); 

    public abstract string[] GetApplicationDependencies();
}