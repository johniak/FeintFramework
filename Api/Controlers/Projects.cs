using FeintSDK;
using Newtonsoft.Json;
using Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Api.Controlers
{
    class Projects
    {
        [ApiAuth]
        public static Response AddProject(Request request)
        {
            var form = Form.FromFormData<ProjectForm>(request.FormData);
            if (!form.IsValid)
                return new Response(request,JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            Project project = new Project() { Name = form.name, Owner = User.GetLoggedUser(request.Session) };
            project.Save();
            return new Response(request, JsonConvert.SerializeObject(new { id = project.Id, name = project.Name, user = project.Owner.Id })) { MimeType = "application/json" };
        }

        [ApiAuth]
        public static Response GetAllProjects(Request request)
        {
            return new Response(request, JsonConvert.SerializeObject(Project.getUserProjectsDisplays(User.GetLoggedUser(request.Session))));
        }

        [ApiAuth]
        public static Response DeleteProject(Request request)
        {
            int projectId;
            if (!int.TryParse(request.Variables["project"].Value, out projectId))
                return new Response(request, JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };

            var tasks = Task.getUserTaskToProject(User.GetLoggedUser(request.Session), projectId);
            foreach (var t in tasks)
            {
                t.Remove();
            }
            Project.Ref<Project>(projectId).Remove();
            return new Response(request, JsonConvert.SerializeObject(true));
        }
    }
}
