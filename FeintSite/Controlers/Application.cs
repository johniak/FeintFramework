using DotLiquid;
using FeintSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Site.Models;
namespace Site.Controlers
{
    class Application
    {

        public static Response Index(Request request)
        {
            Log.D("Count: "+User.Find<User>().Where().Count());
            if (User.IsLogged(request.Session))
            {
                return Response.Redirect("/dashboard/");
            }
            var response = new Response("index.html", Hash.FromAnonymousObject(new {  }));
            return response;
        }

        public static Response Dashboard(Request request)
        {
			return DashboardDisplay(request,null,"all");
        }

        public static Response DashboardWeek(Request request)
        {
            return null;
        }

        public static Response DashboardProject(Request request)
        {
            return null;
        }

        /// <summary>
        /// ToDo dorobić project display i reszte
        /// </summary>
        /// <param name="request"></param>
        /// <param name="projectId"></param>
        /// <param name="global"></param>
        /// <returns></returns>
        public static Response DashboardDisplay(Request request,int? projectId, String global)
        {

            User loggedUser = User.GetLoggedUser(request.Session);
            if (loggedUser == null)
                return Response.Redirect("/login/");
			List<ProjectDisplay> projects = Project.getUserProjectsDisplays (loggedUser);
			var response = new Response("dashboard.html", Hash.FromAnonymousObject(new { projects=projects,selectedProject=projectId,user=loggedUser,type=global }));
			return response;
		//return ok(dashboard.render(logged_user, projects, project_id, global, projects.size()>1));
	}

        public static Response Login(Request request)
        {
            return new Response("login.html", Hash.FromAnonymousObject(new { }));;
        }

        public static Response Authenticate(Request request)
        {
            var username = request.FormData["username"];
            var password = request.FormData["password"];
            var status = User.SignIn(username, password);
            if (status)
            {
                request.Session.SetProperty("isLogged", true.ToString());
                request.Session.SetProperty("username", request.FormData["username"]);
                return Response.Redirect("/dashboard/#message/success/Welcome " + username);
            }
            return new Response("login.html", Hash.FromAnonymousObject(new {hasError=true,errorMessage= "Wrong username or password"}));
        }

        public static Response Logout(Request request)
        {
            return null;
        }

        public static Response Register(Request request)
        {
            return new Response("register.html", Hash.FromAnonymousObject(new { })); ;
        }

        public static Response RegisterPost(Request request)
        {
            var username = request.FormData["username"];
            var password = request.FormData["password"];
            var rePassword = request.FormData["rePassword"];
            var email = request.FormData["email"];
            if (password != rePassword)
            {
                return new Response("register.html", Hash.FromAnonymousObject(new { hasError = true, errorMessage = "Passwords don't match." }));
            }
            var status = User.SignUp(username, password, email);
            if (status == 1)
            {
                return new Response("register.html", Hash.FromAnonymousObject(new { hasError = true, errorMessage = "Can't creat an acount. The username is not available." }));
            }
            if (status == 2)
            {
                return new Response("register.html", Hash.FromAnonymousObject(new { hasError = true, errorMessage = "Password is to short." }));
            }
            if (status == 3)
            {
                return new Response("register.html", Hash.FromAnonymousObject(new { hasError = true, errorMessage = "Invalid email." }));
            }
            if (status == 0)
            {
                return Response.Redirect("/login/");
            }
            return new Response("register.html", Hash.FromAnonymousObject(new { hasError = true, errorMessage = "Unexpected error." }));
        }

        public static Response UpdateUser(Request request)
        {
            return null;
        }

        public static Response Mobile(Request request)
        {
            return null;
        }

        public static Response Usage(Request request)
        {
            return null;
        }
    }
}
