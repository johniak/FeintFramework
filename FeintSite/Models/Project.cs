using Feint.FeintORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Models
{
    class Project:DBModel
    {
        [DBProperty]
        public String Name { get; set; }

        public DBForeignKey<User> Owner { get; set; }

        public List<Project> getUserProjects(User u)
        {
            return Find<Project>().Where().Eq("Owner", u).Execute();
        }
    }
    public class ProjectDisplay
    {
        public long Id { get; set; }
        public String Name { get; set; }
        public long Count { get; set; }

        public ProjectDisplay(long id, String name, long count)
        {
            this.Id = id;
            this.Name = name;
            this.Count = count;
        }
        //public static implicit operator ProjectDisplay(Project p)
        //{
        //    return new ProjectDisplay(p.Id,p.Name,p
        //}

    }
}
