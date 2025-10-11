using InvestigaIA.Model.Case;
using InvestigaIA.Model.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Game
{
    public class GameInfo
    {
        public TimeSpan time = new(0, 10, 0);
        public List<Suspect> suspects;
        public CaseFile _CaseFile;
    }
}