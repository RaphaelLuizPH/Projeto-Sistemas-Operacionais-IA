using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Characters
{
    public class Culprit(string name, string description, string personality, string imageCode) : Suspect(name, description, personality, imageCode)
    {
        [JsonInclude]
        public bool Caught = false;
    }
}