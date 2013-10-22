using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api
{
    class Error
    {
        public int Code { get; set; }
        public string Message { get; set; }
    }

    class Errors
    {
        public static Error NotLoggedIn = new Error() { Code = 1, Message = "Logged in required" };
        public static Error WrongFormData = new Error() { Code = 2, Message = "Wrong form data" };
        public static Error WrongConfirmationPassword = new Error() { Code = 3, Message = "Wrong confirmation password" };
        public static Error PaswordsAreNotTheSame = new Error() { Code = 4, Message = "Passwords are not the same" };
    }
}
