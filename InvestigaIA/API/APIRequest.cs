using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestigaIA.Classes
{

    public class Content
    {
        public string role { get; set; } = "user"; 

        public List<Part> parts { get; set; }
    }

    public class Part
    {
        public string text { get; set; }
    }

    public class APIRequest
    {

        public APIRequest(string content)
        {
            contents = new List<Content>()
            {
                new Content
                {
                    parts = new List<Part>
                    {
                        new Part { text = content }
                    }
                }
            };
        }

        public List<Content> contents { get; set; }
    }


    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Candidate
    {
        public Content content { get; set; }
        public string finishReason { get; set; }
        public double avgLogprobs { get; set; }
    }

    public class CandidatesTokensDetail
    {
        public string modality { get; set; }
        public int tokenCount { get; set; }
    }

 

    public class PromptTokensDetail
    {
        public string modality { get; set; }
        public int tokenCount { get; set; }
    }

    public class APIResponse
    {
        public List<Candidate> candidates { get; set; }
        public UsageMetadata usageMetadata { get; set; }
        public string modelVersion { get; set; }
        public string responseId { get; set; }
    }

    public class UsageMetadata
    {
        public int promptTokenCount { get; set; }
        public int candidatesTokenCount { get; set; }
        public int totalTokenCount { get; set; }
        public List<PromptTokensDetail> promptTokensDetails { get; set; }
        public List<CandidatesTokensDetail> candidatesTokensDetails { get; set; }
    }








}