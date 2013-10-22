using Feint.FeintORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Site.Models
{
    public class Task : DBModel
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

        public enum TaskPriority
        {
            Low=0,Normal=1,High=2
        }
        public enum TaskStatus
        {
            Done=1, Waiting=0
        }
        public static List<Task> getUserTask(User u){
            return Find<Task>().Where().Eq("Owner", u).Execute();
        }
    }
	public  class TaskSafe 
	{

		public long id { get; set; }

		public int priority{ get; set; }

		public String message{ get; set; }

		public int status{ get; set; }

		public String deadline{ get; set; }

		public long project{ get; set; }

		public String project_name{ get; set; }

		
		public TaskSafe(long id, long project, int priority, String message, int status, String deadline, String projectName) {
			this.id = id;
			this.project = project;
			this.priority = priority;
			this.message = message;
			this.status = status;
			this.deadline = deadline;
			this.project_name = projectName;
		}
	}
}
