using FeintSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controlers
{
    class UpdateUserForm : Form
    {
        [EmailField(MaxLenght = 100, Requierd = true)]
        public String email { get; set; }
        [CharField(MaxLenght = 100, Requierd = true)]
        public String password { get; set; }
        [CharField(MaxLenght = 100, Requierd = true)]
        public String retypePassword { get; set; }
        [CharField(MaxLenght = 100, Requierd = true)]
        public String oldPassword { get; set; }
    }

    class ProjectForm : Form
    {
        [CharField(MaxLenght = 100, Requierd = true)]
        public String name { get; set; }
    }

    class TaskForm : Form
    {
        [IntegerField(Requierd=true,MinValue=(int)Site.Models.Task.TaskPriority.Low,MaxValue=(int)Site.Models.Task.TaskPriority.High)]
        public int priority { get; set; }
        [CharField(MaxLenght=512)]
        public String message { get; set; }
        [IntegerField(Requierd = true, MinValue = (int)Site.Models.Task.TaskStatus.Waiting, MaxValue = (int)Site.Models.Task.TaskStatus.Done)]
        public int status { get; set; }
        [DateTimeField(DataTimeFormatString="dd/MM/yyyy")]
        public DateTime deadline { get; set; }
        [IntegerField(Requierd=false)]
        public int project { get; set; }
    }
}
