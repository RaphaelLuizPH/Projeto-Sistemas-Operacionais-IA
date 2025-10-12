using GenerativeAI;
using GenerativeAI.Types;
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
                "mas detalhadas o suficiente para parecerem naturais. Evite respostas excessivamente longas ou genéricas.";
                

                _model.UseJsonMode = true; 

        }

        public async Task<T> SendRequestAsync<T>(string prompt) where T : class
        {

          

            try
            {

               





                GenerationConfig genConfig = new GenerationConfig()
                {
                    ResponseMimeType = "application/json",
                    ResponseSchema = GoogleSchemaHelper.ConvertToSchema<T>(),
                };

                var content = new Content(prompt, "system");



                var chat = _model.StartChat(contents);



                var genContentRequest = new GenerateContentRequest([content], generationConfig: genConfig);

                var response = await chat.GenerateContentAsync(genContentRequest); ;


                var result = response.ToObject<T>();

                if (result == null)
                {

                    result = JsonConvert.DeserializeObject<T>(response.Text);

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





        public async Task<string> SendRequestAsync(string prompt)
        {

           

            try
            {
                var content = new Content(prompt, "system");
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



        public async Task<MessageAnswer> SendRequestAsync(string prompt, string suspectId, GameInstance gameInstance)
        {
            try
            {

                var Suspects = gameInstance.Suspects;
                var caseFile = gameInstance.CaseFile;
                var objectives = gameInstance.Objectives;

                var suspect = Suspects.Find(s => s.Id.ToString() == suspectId);

                if (suspect == null)
                {
                    throw new Exception("Suspect not found");
                }
                GenerationConfig genConfig = new GenerationConfig()
                {
                    ResponseMimeType = "application/json",
                    ResponseSchema = GoogleSchemaHelper.ConvertToSchema<MessageAnswer>(),
                };



                        string promptWithContext = $@"
                Você é {suspect.Name}, um personagem em um jogo de mistério. Você é {suspect.Description}.
                {suspect.SystemPrompt}

                **Regras Essenciais do Jogo:**
                - Responda estritamente como o personagem {suspect.Name}. Mantenha a consistência da sua personalidade, emoções e motivações.
                - Você não pode quebrar a quarta parede. Nunca diga que você é um personagem de um jogo ou que está em um enredo.
                - O jogador é um detetive investigando o crime. Respeite a autoridade dele, mas sem ser submisso. Seu tom deve ser consistente com sua descrição (ex: defensivo, arrogante, assustado, etc.).
                - Se você é o assassino, **não admita o crime** em hipótese alguma. Mantenha-se evasivo e negue qualquer envolvimento, não importa a pergunta.
                - Se você não souber a resposta para a pergunta do jogador, responda que não sabe ou se recuse a responder. Não retorne um texto vazio.
                - Mantenha as respostas concisas, mas detalhadas o suficiente para parecerem naturais. Evite respostas excessivamente longas ou genéricas.

                **Contexto do Caso:**
                - Um assassinato acabou de acontecer. Leve isso em consideração em suas respostas.
                - O enredo do jogo é: {caseFile.CrimeDetails}. Não revele detalhes do enredo que você não deveria saber.

                **Instruções de Saída (Formato):**
                - Inclui um campo 'Completed' para sua resposta. Se um objetivo for completo, adicionei seu Id no campo 'Completed'. Caso contrário, deixe-o vazio ou nulo.
                - Inclui um campo'NewObjectives' para quaisquer novos objetivos que o jogador desbloqueou com esta interação. Se nenhum, deixe vazio.
                - Não inclua nada além da resposta do personagem no campo Text.
                - Use os IDs desta lista: {string.Join(",", objectives)}.
            ";






                var sysContent = new Content(promptWithContext, "system");
                var content = new Content(prompt, "user");




                var chat = _model.StartChat(suspect.ConversationHistory, systemInstruction: promptWithContext);
                


                var genContentRequest = new GenerateContentRequest([sysContent, content], generationConfig: genConfig);

                var response = await chat.GenerateContentAsync(genContentRequest); ;



                return response.ToObject<MessageAnswer>();


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