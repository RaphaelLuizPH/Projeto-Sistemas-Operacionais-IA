using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Game
{
    public class Objective
    {

        public string MainObjective { get; set; }


        public bool Completed { get; set; } = false;

        public bool Hidden { get; set; } = true;

    }
}
