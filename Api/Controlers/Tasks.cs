using DotLiquid;
using FeintSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Site.Models;
using Api.Util;
using Newtonsoft.Json;

namespace Api.Controlers
{
    public class Tasks
    {
        [ApiAuth]
        public static Response AddAll(Request request)
        {
            User user = User.GetLoggedUser(request.Session);
            if (user == null)
                return Response.Redirect("/login/");
            return add(request, user, Project.Find<Project>().Where().Execute()[0]);
        }

        [ApiAuth]
        public static Response Add(Request request)
        {
            int projectId;
            if (!int.TryParse(request.variables["project"].Value, out projectId))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            User user = User.GetLoggedUser(request.Session);
            var project = Project.Ref<Project>(projectId);
            if (!project.isOwnerOfProject(user))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 403 };

            return add(request, user, project);
        }

        [ApiAuth]
        private static Response add(Request request, User user, Project project)
        {
            int priority = int.Parse(request.FormData["priority"]);
            String message = request.FormData["message"];
            int status = int.Parse(request.FormData["status"]);
            DateTime deadline = DateTime.ParseExact(request.FormData["deadline"], "dd/MM/yyyy", null);
            var projectId = Project.Find<Project>().Where().Eq("Name", request.FormData["project_name"]);
            DateRegExpr dateRX = new DateRegExpr(message);
            if (dateRX.Success)
            {
                message = dateRX.Message;
                deadline = dateRX.Date;
            }
            PriorityRegExpr priorityRX = new PriorityRegExpr(message);
            if (priorityRX.Success)
            {
                message = priorityRX.Message;
                priority = priorityRX.Priority;
            }
            Task task;
            try
            {
                task = new Task()
                {
                    Owner = user,
                    ProjectToTask = project,
                    Priority = priority,
                    Message = message,
                    Status = status,
                    Updated = DateTime.Now,
                    Created = DateTime.Now,
                    Deadline = deadline
                };
                task.Save();
                return new Response(JsonConvert.SerializeObject(task.ToTaskSafe()));
            }
            catch (Exception e)
            {
                return new Response(JsonConvert.SerializeObject(false));
            }
        }

        [ApiAuth]
        public static Response UpdateAllTask(Request request)
        {
            int task;
            int project=0;
            if (!int.TryParse(request.variables["task"].Value, out task))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            return updateTask(request, task, project);
        }

        [ApiAuth]
        public static Response UpdateTask(Request request)
        {
            int task;
            int project;
            if (!int.TryParse(request.variables["task"].Value, out task))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            if (!int.TryParse(request.variables["project"].Value, out project))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            return updateTask(request, task, project);
        }

        private static Response updateTask(Request request,int task,int project)
        {
            var form = Form.FromFormData<TaskForm>(request.FormData);
            if (!form.IsValid)
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            Task taskModel = Task.Ref<Task>(task);
            if (taskModel == null)
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            if (taskModel.Owner.Id != User.GetLoggedUser(request.Session).Id)
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 403 };
            taskModel.ProjectToTask.Value.Id = form.project;
            taskModel.ProjectToTask.Value.Save();
            taskModel.Priority = form.priority;
            taskModel.Message = form.message;
            taskModel.Status = form.status;
            taskModel.Deadline = form.deadline;
            taskModel.Updated = DateTime.Now;
            taskModel.Save();
            return new Response(JsonConvert.SerializeObject(taskModel.ToTaskSafe()));
        }

        [ApiAuth]
        public static Response DeleteTask(Request request)
        {
            int taskId;
            if (!int.TryParse(request.variables["task"].Value, out taskId))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            var task = Task.Ref<Task>(taskId);
            if (task == null)
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            if (task.Owner.Id != User.GetLoggedUser(request.Session).Id)
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 403 };
            task.Remove();
            return new Response(JsonConvert.SerializeObject(true));
        }

        [ApiAuth]
        public static Response GetAllTasks(Request request)
        {
            User user = User.GetLoggedUser(request.Session);
            if (user == null)
                return Response.Redirect("/login/");
            List<Task> tasks = Task.getUserTask(user);
            return new Response(JsonConvert.SerializeObject(Task.ToTasksSafe(tasks)));
        }

        [ApiAuth]
        public static Response GetProjectTasks(Request request)
        {
            int projectId;
            if (!int.TryParse(request.variables["project"].Value, out projectId))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            var user = User.GetLoggedUser(request.Session);
            var tasks = Task.getUserTaskToProject(user, projectId);
            return new Response(JsonConvert.SerializeObject(Task.ToTasksSafe(tasks)));
        }


    }
}

