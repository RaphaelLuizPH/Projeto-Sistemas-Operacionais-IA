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
            // Remove any unwanted characters or formatting

            string cleanedResponse = response.Replace("\n", " ").Trim();
            if (cleanedResponse.StartsWith("json"))
            {
                cleanedResponse = cleanedResponse[4..].TrimStart('\n', '\r', ' ');
            }
            cleanedResponse = cleanedResponse.Replace("\r", " ").Trim();
            cleanedResponse = cleanedResponse.Replace("json", " ").Trim();
            cleanedResponse = cleanedResponse.Replace("\t", " ").Trim();
            cleanedResponse = cleanedResponse.Replace("  ", " ").Trim();
            cleanedResponse = cleanedResponse.Replace("```", " ").Trim();

            cleanedResponse.Trim('`').Trim();


            // Optionally, you can add more complex cleaning logic here
            // For example, removing specific patterns or HTML tags

            return cleanedResponse;
        }



    }
}