using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FeintSDK;
using Feint.FeintORM;
using DotLiquid;

namespace Site
{
    class Message : DBModel, ILiquidizable
    {
        [DBForeignKey]
        public User From { get; set; }
        [DBForeignKey]
        public User To { get; set; }
        [DBProperty]
        public String Title { get; set; }
        [DBProperty]
        public String Text { get; set; }
        public static bool SendMessage(Session session, User to, string title, string text)
        {
            User user = User.Find<User>().Where().Eq("Username", session.GetProperty("userId")).Execute()[0];// User.getOne<User>(u => u.Username == session.GetProperty("userId"));
            Message mes = new Message() { From = user, To = to, Title = title, Text = text };
            mes.Save();
            return true;
        }
        public static List<Message> GetReciveBox(Session session)
        {
            User user = User.Find<User>().Where().Eq("Username", session.GetProperty("userId")).Execute()[0];
            var messages = Message.Find<Message>().Where().Eq("To", user).Execute();
            return messages;
        }
        public static List<Message> GetSentBox(Session session)
        {
            User user = User.getOne<User>(u => u.Username == session.GetProperty("userId"));
            var messages = Message.get<Message>(m => m.From.Id == user.Id);
            return messages;
        }


        public object ToLiquid()
        {
            return new {Id,From,To,Title,Text};
        }
    }
}
