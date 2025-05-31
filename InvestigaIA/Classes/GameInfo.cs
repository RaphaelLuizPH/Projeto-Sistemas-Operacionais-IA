using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TheInterrogatorAIDetective.Models;

namespace InvestigaIA.Classes
{
    public class GameInfo
    {
        public TimeSpan time = new(0, 10, 0);
        public List<Suspeito> suspects;
        public CaseFile _CaseFile;
    }
}