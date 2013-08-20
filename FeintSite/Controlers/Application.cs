using DotLiquid;
using FeintSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Controlers
{
    class Application
    {

        public static Response Index(Request request)
        {
            if (User.IsLogged(request.Session))
            {
                return Response.Redirect("/dashboard/");
            }
            var response = new Response("index.html", Hash.FromAnonymousObject(new {  }));
            return response;
        }

        public static Response Dashboard(Request request)
        {
            return null;
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
        public static Response dashboardDisplay(Request request,int projectId, String global)
        {

            User loggedUser = User.GetLoggedUser(request.Session);
            if (loggedUser == null)
                return Response.Redirect("/login/");
	//	List<ProjectDisplay> projects = Projects.getUserProjects();
            return null;
		//return ok(dashboard.render(logged_user, projects, project_id, global, projects.size()>1));
	}

        public static Response Login(Request request)
        {
            return new Response("login.html", Hash.FromAnonymousObject(new { }));;
        }

        public static Response Authenticate(Request request)
        {
            var username = request.POST["username"];
            var password = request.POST["password"];
            var status = User.SignIn(username, password);
            if (status)
            {
                request.Session.SetProperty("isLogged", true.ToString());
                request.Session.SetProperty("username", request.POST["username"]);
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
            var username = request.POST["username"];
            var password = request.POST["password"];
            var rePassword = request.POST["rePassword"];
            var email = request.POST["email"];
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
