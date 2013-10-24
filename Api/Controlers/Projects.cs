using FeintSDK;
using Newtonsoft.Json;
using Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controlers
{
    class Projects
    {
        [ApiAuth]
        public static Response AddProject(Request request)
        {
            var form = Form.FromFormData<ProjectForm>(request.FormData);
            if (!form.IsValid)
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            Project project = new Project() { Name = form.name, Owner = User.GetLoggedUser(request.Session) };
            project.Save();
            return new Response(JsonConvert.SerializeObject(new ProjectDisplay(project.Id,project.Name,project.Owner.Id)));
        }

        [ApiAuth]
        public static Response GetAllProjects(Request request)
        {
            return new Response(JsonConvert.SerializeObject(Project.getUserProjectsDisplays(User.GetLoggedUser(request.Session))));
        }

        [ApiAuth]
        public static Response DeleteProject(Request request)
        {
            int projectId;
            if (!int.TryParse(request.variables["project"].Value, out projectId))
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) { Status = 400 };
            Project.Ref<Project>(projectId).Remove();
            return new Response(JsonConvert.SerializeObject(true));
        }
    }
}
