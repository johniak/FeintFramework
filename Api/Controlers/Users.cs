using FeintSDK;
using Newtonsoft.Json;
using Site.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Api.Controlers
{
    class Users
    {
        [ApiAuth]
        public static Response updateUser(Request request)
        {

            UpdateUserForm form = Form.FromFormData<UpdateUserForm>(request.FormData);
            if (!form.IsValid)
                return new Response(JsonConvert.SerializeObject(Errors.WrongFormData)) {Status=400};
            User user = User.GetLoggedUser(request.Session);
            if(user.Password!=User.MD5Hash(form.oldPassword))
                return new Response(JsonConvert.SerializeObject(Errors.WrongConfirmationPassword)) { Status = 403 };
            if (form.password != form.retypePassword)
                return new Response(JsonConvert.SerializeObject(Errors.PaswordsAreNotTheSame)) { Status = 400 };

            user.Mail = form.email;
            user.Password = User.MD5Hash(form.password);
            user.Save();
            return new Response(JsonConvert.SerializeObject(user)); 
        }
    }
}
