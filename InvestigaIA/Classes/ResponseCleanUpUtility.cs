using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InvestigaIA.Classes
{
    public static class ResponseCleanUpUtility
    {


        public static string CleanUpResponse(string response)
        {
            

            string cleanedResponse = response
                .Replace("\n", " ")
                .Replace("\r", " ")
                .Replace("\t", " ")
                .Replace("```", " ")
                .Replace("csharp", " ")
                .Replace("json", " ")
                .Replace("  ", " ")
                .Trim()
                .Trim('`', ',', '.');
            if (cleanedResponse.StartsWith("json"))
            {
                cleanedResponse = cleanedResponse[4..].TrimStart('\n', '\r', ' ');
            }
           

            return cleanedResponse;
        }



    }
}