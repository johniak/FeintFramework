using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeintSDK;
using Api.Controlers;


namespace Api
{
    class Api
    {
        public static void Main()
        {
            Settings.Urls.Add(new Url(@"^/user/$", Users.UpdateUser, RequestMethod.PUT));

            Settings.Urls.Add(new Url(@"^/tasks/$", Tasks.GetAllTasks, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/all/tasks/$", Tasks.GetAllTasks, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/all/tasks/$", Tasks.AddAll, RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/projects/week/tasks/$", Tasks.GetAllTasks, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/week/tasks/$", Tasks.AddAll, RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/projects/(?<project>[0-9]+)/tasks/$", Tasks.GetProjectTasks, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/(?<project>[0-9]+)/tasks/$", Tasks.Add, RequestMethod.POST));


            Settings.Urls.Add(new Url(@"^/projects/all/tasks/(?<task>[0-9]+)/$", Tasks.UpdateAllTask, RequestMethod.PUT));
            Settings.Urls.Add(new Url(@"^/projects/all/tasks/(?<task>[0-9]+)/$", Tasks.DeleteTask, RequestMethod.DELETE));
            Settings.Urls.Add(new Url(@"^/projects/week/tasks/(?<task>[0-9]+)/$", Tasks.UpdateAllTask, RequestMethod.PUT));
            Settings.Urls.Add(new Url(@"^/projects/week/tasks/(?<task>[0-9]+)/$", Tasks.DeleteTask, RequestMethod.DELETE));
            Settings.Urls.Add(new Url(@"^/projects/(?<project>[0-9]+)/tasks/(?<task>[0-9]+)/$", Tasks.UpdateTask, RequestMethod.PUT));
            Settings.Urls.Add(new Url(@"^/projects/(?<project>[0-9]+)/tasks/(?<task>[0-9]+)/$", Tasks.DeleteTask, RequestMethod.DELETE));

            Settings.Urls.Add(new Url(@"^/projects/$", Projects.AddProject, RequestMethod.POST));
            Settings.Urls.Add(new Url(@"^/projects/$", Projects.GetAllProjects, RequestMethod.GET));
            Settings.Urls.Add(new Url(@"^/projects/(?<project>[0-9]+)/$", Projects.DeleteProject, RequestMethod.DELETE));
        }
    }
}
