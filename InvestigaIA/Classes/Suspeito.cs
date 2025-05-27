using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Spectre.Console;



namespace InvestigaIA.Classes
{
    // SuspectProfile.cs
    public class Suspeito : IBarChartItem
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SystemPrompt { get; set; }

      
        public List<Content> _conversationHistory = new List<Content>();
        public double StressLevel { get; set; } = 0.0d;



        public string Label => Name;

        public double Value => StressLevel;

        public Color? Color => StressLevel switch
        {
            < 0.2 => Spectre.Console.Color.Green,
            < 0.4 => Spectre.Console.Color.White,
            < 0.6 => Spectre.Console.Color.Yellow,
            < 0.8 => Spectre.Console.Color.Red,
            _ => Spectre.Console.Color.Red
        };

        public Suspeito(string name, string description, string systemPrompt)
        {
            Name = name;
            Description = description;
            SystemPrompt = systemPrompt;
        }
    }
}