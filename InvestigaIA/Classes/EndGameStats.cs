using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheInterrogatorAIDetective.Models;

namespace InvestigaIA.Classes
{
    public class EndGameStats
    {


        public CaseFile CaseFile { get; set; }
        public Dictionary<string, bool> Objetivos { get; set; }
        public Suspeito Acusado { get; set; }

        public TimeSpan Time { get; set; }
        public string Message { get; set; }

        public bool JusticeServed { get; set; } = false;

        public bool IsCorrectSuspect
        {
            get
            {
                return Acusado != null && Acusado.Name == CaseFile.Culpado.Name;
            }
        }
    }
}