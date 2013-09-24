using FeintSDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel
{
    class LogInForm : Form
    {
        [CharField(MinLenght = 3)]
        public String username { get; set; }
        [CharField(MinLenght = 3)]
        public String password { get; set; }
    }
}
