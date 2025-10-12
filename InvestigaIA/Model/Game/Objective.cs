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

        public Guid Id { get; set; } = Guid.NewGuid();

        public bool Completed { get; set; } = false;

 
        public override string ToString()
        {
            return MainObjective + "\t" + Id;
        }

    }


    public class ObjectiveDTO
    {

        public string MainObjective { get; set; }




    }
}
