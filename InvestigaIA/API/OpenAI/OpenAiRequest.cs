using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestigaIA.API.OpenAI
{




    public class OpenAiRequest
    {
        public string model { get; set; } = "gpt-4o-mini";
        public bool store { get; set; } = true;
        public List<Message> messages { get; set; }




    }








}