using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Utilities
{
    public class AskRequest
    {
        
        public required string GameId { get; set; } = string.Empty;
        public required string SuspectID { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public required string SenderID { get; set; } = string.Empty;

        public required string Sender { get; set; }

        public AskRequest() { }

        public AskRequest(string gameId, string suspectId, string message, string senderID, string sender)
        {
            GameId = gameId ?? string.Empty;
            SuspectID = suspectId ?? string.Empty;
            Message = message ?? string.Empty;
            SenderID = senderID ?? string.Empty;
            Sender = sender ?? string.Empty;
        }
    }
}
