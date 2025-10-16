using InvestigaIA.Model.Case;
using InvestigaIA.Model.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Game
{
    public class EndGameStats
    {


        public CaseFile CaseFile { get; set; }
        public Dictionary<string, bool> Objectives { get; set; }
        public Suspect AccusedSuspect { get; set; }

        public TimeSpan Time { get; set; }
        public string Message { get; set; }

        public bool JusticeServed { get; set; } = false;

        public bool IsCorrectSuspect
        {
            get
            {
                return AccusedSuspect != null && AccusedSuspect.Name == CaseFile.Culprit.Name;
            }
        }
    }
}