using GenerativeAI.Types;
using System.Text.Json.Serialization;



namespace InvestigaIA.Model.Characters
{
    // SuspectProfile.cs

    public class Suspect 
    {
        public Suspect(string name, string description, string systemPrompt, string imageCode) 
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            SystemPrompt = systemPrompt ?? throw new ArgumentNullException(nameof(systemPrompt));
            ImageCode = imageCode ?? throw new ArgumentNullException(nameof(imageCode));
        }


        public Suspect()
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string SystemPrompt { get; set; }

        public string ImageCode { get; set; }

        [JsonIgnore]
        [JsonPropertyName("conversationHistory")]
        public List<Content> ConversationHistory { get; set; } = [];
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