using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeintSDK;
using DotLiquid;
using Feint.FeintORM;
using Newtonsoft.Json;
using Site.Models;

namespace Site
{
    class Views
    {
        static dynamic cos()
        {
            return 2;
        }
        public static Response Index(Request request)
        {
            User.SignUp("johniak", "test");
            User.SignUp("ania", "test");
            var user = User.Find<User>().Where().execute();
            foreach (var u in user)
            {
               
                Console.WriteLine(u.Id + "\t" + u.Username + "\t" + u.Password);
            }
            var model = SampleModel.Find<SampleModel>().Where().execute();
            var mess = Message.Find<Message>().Where().Eq("To", user[1]).And().Ge("From.model", model[1]).execute();
            var tasks = ToDoTask.getAll<ToDoTask>();
            var response = new Response("index.html", Hash.FromAnonymousObject(new { message = "Hello World!",isLogged=User.isLogged(request.Session)}));
            return response;
        }
        public static Response Register(Request request)
        {
            return new Response("register.html", Hash.FromAnonymousObject(new {  }));
        }
        public static Response SignIn(Request request)
        {
            if (request.Method != "POST")
                return ErrorPages.ExpectedPostMethod(request);

            if (!User.SignIn(request.POST["username"], request.POST["password"]))
                return new Response("index.html", Hash.FromAnonymousObject(new { loginMessage = "Bad username or password." }));
            User user= User.getOne<User>(u => u.Username == request.POST["username"]);
            request.Session.SetProperty("isLogged", true.ToString());
            request.Session.SetProperty("userId", request.POST["username"]);
            return Response.Redirect("/");

        }
        public static Response SignUp(Request request)
        {
            if (User.isLogged(request.Session))
                return Response.Redirect("/");
            if (request.Method != "POST")
                return ErrorPages.ExpectedPostMethod(request);
            if (!User.SignUp(request.POST["username"], request.POST["password"]))
                return new Response("register.html", Hash.FromAnonymousObject(new { registerMassage="Wrong input data." }));
            return Response.Redirect("/");
        }
        public static Response LogOut(Request request)
        {
            if (User.isLogged(request.Session))
            {
                request.Session.UnsetProperty("isLogged");
                request.Session.UnsetProperty("userId");
            }
            return Response.Redirect("/");
        }
        public static Response Messages(Request request)
        {
            var messages = Message.GetReciveBox(request.Session);
            return new Response("messages.html", Hash.FromAnonymousObject(new {messages=messages  }));
        }

        public static Response SendMessage(Request request)
        {
            if (!User.isLogged(request.Session))
                return Response.Redirect("/");
            if (request.Method != "POST")
                return ErrorPages.ExpectedPostMethod(request);
            User user= User.getOne<User>(u=> u.Username== request.POST["to"]);
            if (user==null)
             return new Response(JsonConvert.SerializeObject(false));
           return new Response(JsonConvert.SerializeObject( Message.SendMessage(request.Session, user, request.POST["title"], request.POST["text"])));
        }
        public static Response GetReciveBoxJson(Request request)
        {
            if (!User.isLogged(request.Session))
                return Response.Redirect("/");
            var messages = Message.getAll<Message>();
            var lis = new List<object>();
            foreach(var m in messages)
            {
                lis.Add(new { m.Id, m.Title, m.Text, From = m.From.Username, To = m.To.Username });
            }
            return new Response(JsonConvert.SerializeObject(lis));
        }
    }
}
