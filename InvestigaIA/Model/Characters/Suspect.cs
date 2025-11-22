using GenerativeAI.Types;
using InvestigaIA.Model.Game;
using Newtonsoft.Json;
using System.Text.Json.Serialization;



namespace InvestigaIA.Model.Characters
{
    // SuspectProfile.cs

    public class Suspect 
    {
        public Suspect(string name, string description, string personality, string imageCode) 
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            Personality = personality ?? throw new ArgumentNullException(nameof(personality));
            ImageCode = imageCode ?? throw new ArgumentNullException(nameof(imageCode));
        }

        [Newtonsoft.Json.JsonConstructor]
        [System.Text.Json.Serialization.JsonConstructor]
        public Suspect()
        {
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string Personality { get; set; }

        public string Id { get; set; } 
        public string ImageCode { get; set; }

        [Newtonsoft.Json.JsonIgnore]
        [JsonProperty("conversationHistory")]
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