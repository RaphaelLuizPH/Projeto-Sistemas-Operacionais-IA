using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Characters
{
    public class Culprit(string nome, string description, string systemPrompt) : Suspect(nome, description, systemPrompt)
    {
        [JsonInclude]
        public bool Caught = false;
    }
}