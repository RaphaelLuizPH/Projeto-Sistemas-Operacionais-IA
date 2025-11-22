using InvestigaIA.Model.Case;
using InvestigaIA.Model.Characters;
using InvestigaIA.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigaIA.Model.DTOs
{
    public class GameInstanceJSON
    {
        public List<Suspect> Suspects { get; set; }

        public CaseFile CaseFile { get; set; }

        public Dictionary<string, List<ChatMessage>> Chats { get; set; }

        public List<Objective> Objectives { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool Public { get; set; }

        public string GameId { get; set; }

        public string ApiKey { get ; set;}
    }
}
