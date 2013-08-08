using Feint.FeintORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Models
{
    class Task :DBModel
    {
        public DBForeignKey<User> Owner { get; set; }
        public DBForeignKey<Project> ProjectToTask { get; set; }

        [DBProperty]
        public int Priority { get; set; }

        [DBProperty]
        public String Message { get; set; }

        [DBProperty]
        public int Status { get; set; }

        [DBProperty]
        public DateTime Updated { get; set; }

        [DBProperty]
        public DateTime Created { get; set; }

        [DBProperty]
        public DateTime Deadline { get; set; }

        enum TaskPriority
        {
            Low=0,Normal=1,High=2
        }
        enum TaskStatus
        {
            Done=1, Waiting=0
        }
        public static List<Lazy<Task>> getUserTask(User u){
            return Find<Task>().Where().Eq("Owner", u).Execute();
        }
    }
}
