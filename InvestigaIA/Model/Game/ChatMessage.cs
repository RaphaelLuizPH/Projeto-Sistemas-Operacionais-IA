using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Game
{
    public class ChatMessage(string sender, string senderId, string message)
    {

        public string Message { get; set; } = message;

        public string MessageID { get; set; } = Guid.NewGuid().ToString();

        public string Sender { get; set; } = sender;

        public string SenderID { get; set; } = senderId;

        public DateTime time { get; set; } = DateTime.Now;
    }





}
