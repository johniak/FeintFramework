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
        public static Response AddAll(Request request)
        {
            User user = User.GetLoggedUser(request.Session);
            if (user == null)
                return Response.Redirect("/login/");
            return add(request, user, Project.Find<Project>().Where().Execute()[0]);
        }


        public static Response Add(Request request)
        {
            User user = User.GetLoggedUser(request.Session);
            if (user == null)
                return Response.Redirect("/login/");
            var project = Project.Ref<Project>(int.Parse(request.FormData["project"])); //Find<Project> ().Where ().Eq ("Id", request.FormData ["project"]).Execute()[0];
            if (!project.isOwnerOfProject(user))
            {
                return null;
            }

            return add(request, user, project);
        }

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

        public static Response updateTask(Request request)
        {
            int task;
            int project;
            if (!int.TryParse(request.variables["task"].Value, out task))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            if (!int.TryParse(request.variables["project"].Value, out project))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
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

        public static Response DeleteTask(Request request)
        {
            int taskID;
            if (!int.TryParse(request.variables["task"].Value, out taskID))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            var task = Task.Ref<Task>(taskID);
            if (task == null)
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            task.Remove();
            return new Response(JsonConvert.SerializeObject(true));
        }

        public static Response GetAll(Request request)
        {
            User user = User.GetLoggedUser(request.Session);
            if (user == null)
                return Response.Redirect("/login/");

            List<Task> tasks = Task.getUserTask(user);

            List<TaskSafe> tasksSafe = new List<TaskSafe>();
            foreach (Task t in tasks)
            {
                tasksSafe.Add(t.ToTaskSafe());
            }
            return new Response(JsonConvert.SerializeObject(tasksSafe));
        }

    }
}

