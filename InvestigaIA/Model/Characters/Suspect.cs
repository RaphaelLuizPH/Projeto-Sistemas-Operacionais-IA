using GenerativeAI.Types;
using System.Text.Json.Serialization;



namespace InvestigaIA.Model.Characters
{
    // SuspectProfile.cs

    public class Suspect 
    {
        public Suspect(string name, string description, string systemPrompt) 
        {

        }


        public Suspect()
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string SystemPrompt { get; set; }

        public string ImageCode { get; set; }

        [JsonInclude]
        [JsonPropertyName("conversationHistory")]
        public List<Content> conversationHistory { get; set; } = [];
        public double StressLevel { get; set; } = 0.0d;




      
    }




    public class SuspectDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string SystemPrompt { get; set; }

        public string ImageCode { get; set; }

        public SuspectDTO(string name, string description, string systemPrompt)
        {
            Name = name;
            Description = description;
            SystemPrompt = systemPrompt;
        }
    }
}