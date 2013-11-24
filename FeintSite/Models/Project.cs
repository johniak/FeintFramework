using Feint.FeintORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DotLiquid;

namespace Site.Models
{
	public class Project:DBModel, ILiquidizable
	{
		[DBProperty]
		public String Name { get; set; }

		public DBForeignKey<User> Owner { get; set; }

		public static List<Project> getUserProjects (User u)
		{
			return Find<Project> ().Where ().Eq ("Owner", u).Execute ();
		}

		public static List<ProjectDisplay> getUserProjectsDisplays (User u)
		{
			var projects = Find<Project> ().Where ().Eq ("Owner", u).Execute ();
			var projectsDisplays = new List<ProjectDisplay> ();
			foreach (var p in projects)
				projectsDisplays.Add (p);
			return projectsDisplays;
		}

		public bool isOwnerOfProject (User u)
		{
			if (Owner.Value.Id == u.Id) 
			{
				return true;
			}
			return false;
		}

		public object ToLiquid ()
		{
			return this;
		}
        public override string ToString()
        {
            return Name+" "+Id;
        }
	}

	public class ProjectDisplay: ILiquidizable
	{
		public long Id { get; set; }

		public String Name { get; set; }

		public long Count { get; set; }

		public ProjectDisplay (long id, String name, long count)
		{
			this.Id = id;
			this.Name = name;
			this.Count = count;
		}

		public static implicit operator ProjectDisplay (Project p)
		{
			return new ProjectDisplay (p.Id, p.Name, Task.Find<Task> ().Where ().Eq ("ProjectToTask.Id", p.Id).Execute ().Count);
		}

		public object ToLiquid ()
		{
			return new {Id=this.Id, Name=this.Name,Count=this.Count};
		}
	}
}
