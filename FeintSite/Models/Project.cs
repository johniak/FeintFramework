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
}
