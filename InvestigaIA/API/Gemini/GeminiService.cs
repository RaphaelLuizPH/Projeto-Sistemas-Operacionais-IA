using GenerativeAI;
using GenerativeAI.Types;
using InvestigaIA.Classes;
using InvestigaIA.Model.Case;
using InvestigaIA.Model.Characters;
using InvestigaIA.Model.Game;
using InvestigaIA.Model.Utilities;
using Sprache;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.X86;
using System.Text.Json;
using System.Threading.Tasks;
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
        private List<Content> _conversationHistory = new List<Content>();

        public GeminiService(string APIKey)
        {
            _APIKey = APIKey ?? throw new ArgumentNullException(nameof(APIKey));

            var googleAI = new GoogleAi(apiKey: APIKey);
            _model = googleAI.CreateGeminiModel("gemini-2.0-flash");



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

                var content = new Content(prompt, "user");



                var chat =  _model.StartChat(contents);

                  

                var genContentRequest = new GenerateContentRequest([content], generationConfig: genConfig);
              
                var response = await chat.GenerateContentAsync(genContentRequest); ;



                    return response.ToObject<T>();


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



        public async Task<MessageAnswer> SendRequestAsync(string prompt, Suspect suspect) 
        {
            try
            {


                GenerationConfig genConfig = new GenerationConfig()
                {
                    ResponseMimeType = "application/json",
                    ResponseSchema = GoogleSchemaHelper.ConvertToSchema<MessageAnswer>(),
                };



                string promptWithContext = @$"Você é um personagem chamado {suspect.Name}. Você é {suspect.Description} e suas descrição é: {suspect.SystemPrompt}.
                Você deve responder como se fosse esse personagem, não exagere no texto, responda de tamanho adequado para a pergunta.
                Você não pode quebrar a quarta parede e não pode dizer que é um personagem de um jogo. 
                Se você não souber a resposta, você deve dizer que não sabe.Você pode escolher o silêncio.Você NÃO É O ASSASSINO, 
                não admita independente do que o jogador perguntar. Lembre - se que um assassinato acabou de acontecer e você não deve ignorar isso. 
                O jogador é um investigador e sua autoridade deve ser respeitada. Essa é a mensagem do jogador: {prompt} ";
                
                
                var content = new Content(promptWithContext, "user");



                var chat = _model.StartChat(contents);



                var genContentRequest = new GenerateContentRequest([content], generationConfig: genConfig);

                var response = await chat.GenerateContentAsync(genContentRequest); ;



                return response.ToObject<MessageAnswer>();


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









    }
}