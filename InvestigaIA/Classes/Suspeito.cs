using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;



namespace InvestigaIA.Classes
{
    // SuspectProfile.cs
    public class Suspeito
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SystemPrompt { get; set; }

        public string ImageCode { get; set; }
        public List<Content> _conversationHistory = new List<Content>();
        public double StressLevel { get; set; } = 0.0d;




        public Suspeito(string name, string description, string systemPrompt)
        {
            Name = name;
            Description = description;
            SystemPrompt = systemPrompt;
        }
    }
}