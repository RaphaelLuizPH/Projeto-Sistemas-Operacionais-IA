using GenerativeAI;
using GenerativeAI.Types;
using InvestigaIA.Model.Characters;
using InvestigaIA.Model.Game;
using InvestigaIA.Model.Utilities;
using Newtonsoft.Json;
using Sprache;
using Content = GenerativeAI.Types.Content;

namespace InvestigaIA.API.Gemini
{
    /// <summary>
    /// Provides methods to interact with the Gemini API for AI responses.
    /// </summary>
    public class GeminiService
    {
        private readonly string _APIKey;
        private readonly GeminiModel _model;
        private readonly List<Content> contents = [];
   


        public GeminiService(string APIKey)
        {
            _APIKey = APIKey ?? throw new ArgumentNullException(nameof(APIKey));

            var googleAI = new GoogleAi(apiKey: APIKey);
            _model = googleAI.CreateGeminiModel("gemini-2.0-flash");

            _model.SystemInstruction = "Você é um agente de IA dentro de um jogo de mistério. " +
                "Quando for dito que você é um personagem, responda como um personagem do jogo, mantendo a consistência da personalidade, " +
                "emoções e motivações. Nunca admita ser uma IA ou que está em um jogo. Mantenha as respostas concisas, " +
                "mas detalhadas o suficiente para parecerem naturais. Evite respostas excessivamente longas ou genéricas." + "Quando você não for especificado um personagem" +
                "Siga o prompt normalmente, sem atuação";
                

                _model.UseJsonMode = true; 

        }

        public async Task<T> SendPromptAsync<T>(string prompt) where T : class
        {

          

            try
            {

               





                GenerationConfig genConfig = new GenerationConfig()
                {
                    ResponseMimeType = "application/json",
                    ResponseSchema = GoogleSchemaHelper.ConvertToSchema<T>(),
                };

                var content = new Content(prompt, "user");



                var chat = _model.StartChat(contents);



                var genContentRequest = new GenerateContentRequest([content], generationConfig: genConfig);

                var response = await chat.GenerateContentAsync(genContentRequest); ;


                var result = response.ToObject<T>();

                if (result == null)
                {

                    result = JsonConvert.DeserializeObject<T>(response.Text);

                }

                if (result == null)
                {
                    throw new Exception("Could not deserialize LLM response");
                }


                return result;


            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {


                throw;
            }
            catch (Exception ex)
            {
                throw;

            }

        }





        public async Task<string> SendPromptAsync(string prompt)
        {

           

            try
            {
                var content = new Content(prompt, "user");
                var chat = _model.StartChat(contents);
                var response = await chat.GenerateContentAsync(new GenerateContentRequest([content]));
                return response.Text;
            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }

        }



        public async Task<MessageAnswer> SendPromptAsync<T>(string prompt, string promptWithContext, Suspect suspect, List<Objective> objectives) where T : MessageAnswer
        {
            try
            {

             
        

           
                GenerationConfig genConfig = new GenerationConfig()
                {
                    ResponseMimeType = "application/json",
                    ResponseSchema = GoogleSchemaHelper.ConvertToSchema<MessageAnswer>(),
                };




                var sysContent = new Content(promptWithContext, "user");
                var content = new Content(prompt, "user");




                var chat = _model.StartChat(suspect.ConversationHistory, systemInstruction: promptWithContext);
                


                var genContentRequest = new GenerateContentRequest([sysContent, content], generationConfig: genConfig);

                var response = await chat.GenerateContentAsync(genContentRequest); ;


                var messageAnswer = response.ToObject<MessageAnswer>();

                if(messageAnswer is null)
                {
                    messageAnswer = JsonConvert.DeserializeObject<MessageAnswer>(response.Text);
                } 

                if(messageAnswer is null)
                {
                    throw new Exception("Could not deserialize LLM response");
                }


                if (messageAnswer?.NewObjectives?.Length > 0)
                {
                    foreach(var obj in messageAnswer.NewObjectives)
                    {
                      


                        if (!objectives.Select(o => o.MainObjective).Contains(obj.MainObjective))
                        {
                           
                            
                            objectives.Add(new Objective() { MainObjective = obj.MainObjective});
                        }
                    }
                }


                if(messageAnswer?.Completed?.Length > 0)
                {
                    foreach(var id in messageAnswer.Completed)
                    {
                        var objective = objectives.FirstOrDefault(o => o.Id.ToString() == id);
                        if(objective != null)
                        {
                            objective.Completed = true;
                        }
                    }
                }


                return messageAnswer;


            }
            catch (HttpRequestException httpEx) when (httpEx.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable)
            {


                throw new Exception(httpEx.Message + ": Serviço Gemini indisponível ou esgotado " + httpEx.StatusCode);
            }
            catch (Exception ex)
            {
                throw;

            }

        }



       public void ConfigureModel(string? language)
        {

            if(!string.IsNullOrEmpty(language))
            {
                _model.SystemInstruction += $"Answer in {language}";
            }

        }






    }
}