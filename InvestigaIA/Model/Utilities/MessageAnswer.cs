using InvestigaIA.Model.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Utilities
{
    public class MessageAnswer
    {

        public string? Text { get; set; }


        public string[] Completed { get; set; } 

        public string[] Notes { get; set; }

        public ObjectiveDTO[] NewObjectives { get; set; }

        bool CaseClosed { get; set; } = false;




    }
}