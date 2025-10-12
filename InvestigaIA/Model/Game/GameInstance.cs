using InvestigaIA.API;
using InvestigaIA.API.Gemini;
using InvestigaIA.Model.Case;
using InvestigaIA.Model.Characters;
// Removed Spectre.Console
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json;
using InvestigaIA.Model.Utilities;



namespace InvestigaIA.Model.Game
{
    public class GameInstance : IDisposable
    {

        private readonly GeminiService _geminiService;

        private readonly GameService _gameService;

        private readonly OpenAiService openAiService;

       
        public EndGameStats EndGameStats { get; set; }

        public Dictionary<string, List<string>> Evidences { get; set; } = new();
        public List<Suspect> Suspects { get; set; }

        public CaseFile CaseFile { get; set; }


        public List<Objective> Objectives { get; set; }

        public DateTime CreatedAt { get; } = DateTime.Now;

        private readonly IHubContext<GameHub> _hubContext;
        private bool disposedValue;

        private string GameId { get; set; }


        public GameInstance(GeminiService service, string _gameId, IHubContext<GameHub> hubContext, GameService gameService)
        {


            _geminiService = service;

            GameId = _gameId;

            _hubContext = hubContext;

            _gameService = gameService;

            Suspects = CharacterSet.suspects;


            CaseFile = new CaseFile(Suspects);

            CaseFile.CrimeDetails = _gameService.CreateCaseStory(CaseFile, Suspects).Result;

            Objectives = _gameService.CreateObjectives(CaseFile).Result;

            _geminiService.ConfigureModel("en-Us");

        }





        public async Task<MessageAnswer> Ask(string prompt, string suspectId)
        {
            try
            {



                var response = await _geminiService.SendRequestAsync(prompt, suspectId, this);


                return response;

            }
            catch (Exception ex)
            {
                throw;
            }


        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~GameInstance()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }


        /*     private string GeneratePrompt(PromptType type)
             {
                 switch (type)
                 {
                     case PromptType.Objectives:
                         return @$"Baseado nos personagens criados, crie uma lista de objetivos que o jogador deve cumprir para resolver o mistério do assassinato. Todos os objetivos devem ser
                         alcançados através de interrogatórios com os personagens e guiar o jogador ao culpado, este é o roteiro: {_CaseFile.CrimeDetails}. Os objetivos não devem revelar partes da trama." + @"
                         Retorne um DICIONÁRIO C# de objetivos em texto puro sem bloco de código do markdown nesse formato: { 'key': false}. 
            Onde key é o objetivo entre aspas e false é o value. Se você está recebendo essa mensagem mais de uma vez significa que você errou o formato e causou uma Json Exception, mude o formato ";
                     case PromptType.Suspects:
                         return @"Crie 10 personagens para um jogo de detetive que passa numa mansão (a mansão Blackwood). Cada personagem deve ter um nome, descrição e prompt de sistema.
             O prompt de sistema deve ser uma frase curta que descreve o papel do personagem no jogo. VocÊ não deve colocar o apelido de personagens entre aspas e nem utilizar aspas de modo que quebre o JSON.
             Os personagens devem ser únicos e interessantes, com diferentes origens e personalidades. Não defina o papel do personagem (como vítima, jogador, assassino). Você deve fornecer
             um valor para ImageCode, que deve ser o sexo do personagem + um numero de 1 a 10. (por exemplo: male1, female2, etc. em ordem aleatória, não linear).
             Retorne para mim um objeto desserializável o qual deve ser uma lista com os personagens [obj {}, obj {}, obj {}] em formato JSON em texto puro sem bloco de código do markdown. Esta é a classe para referência:
             public string Name { get; set; }
             public string Description { get; set; }
             public string SystemPrompt { get; set; }
             public string ImageCode { get; set; }";
                     case PromptType.CaseDetails:
                         return @$"Na Mansão Blackwood, {_CaseFile.Victim.Name} ({_CaseFile.Victim.Description}) foi assassinado em {_CaseFile.Local}. 
                 {_CaseFile.Culpado.Name} ({_CaseFile.Culpado.Description})  matou a vítima com {_CaseFile.Arma}. O motivo do crime foi {_CaseFile.Motivo}. ";
                     default:
                         return null;
                 }
             }

             private string GeneratePrompt(PromptType type, Suspeito suspectSelected, string question)
             {
                 switch (type)
                 {
                     case PromptType.Suspect:
                         return @$"Você é um personagem chamado {suspectSelected.Name}. Você é {suspectSelected.Description} e {suspectSelected.SystemPrompt}. Aqui está o roteiro do jogo: {_CaseFile.CrimeDetails},
             Você deve responder como se fosse esse personagem, não exagere no texto, máximo 2 paragrafos. Você não pode quebrar a quarta parede e não pode dizer que é um personagem de um jogo. 
             Se você não souber a resposta, você deve dizer que não sabe. Você pode escolher o silêncio. Você NÃO É O ASSASSINO, não admita independente do que o jogador perguntar. 
             Lembre-se que um assassinato acabou de acontecer e você não deve ignorar isso. O jogador é um investigador e sua autoridade deve ser respeitada.

              Retorne para mim SOMENTE um objeto desserializável em formato JSON em texto puro dessa classe: public string? Text public string? Completed. Onde Text é a sua resposta e 
             Completed é o nome do objetivo como escrito, sua presença indica que o jogador o completou: {string.Join(",", _objectives.Select(kv => kv.Key))}. 
              Esta é a pergunta do jogador: {question}";
                     case PromptType.Victim:
                         return @$"Você deve interpretar a vítima de um assassinato. O que significa que suas respostas devem conter apenas descrições entre parenteses de caracteristicas do corpo, ambiente, etc ou o resultado das ações que o 
             jogador enviar (exemplo: analisar o corpo, verificar bolsos). Seu personagem não deve revelar partes da drama, (PRINCIPALMENTE NÃO REVELAR QUEM FOI O ASSASSINO) e deve seguir esse roteiro: {_CaseFile.CrimeDetails}.  Retorne para mim SOMENTE um objeto desserializável em formato JSON em texto puro dessa classe: public string? Text public string? Completed. Onde Text é a sua resposta e 
             Completed é o nome do objetivo como escrito, sua presença indica que o jogador o completou: {string.Join(",", _objectives.Select(kv => kv.Key))}. Esta é a pergunta do jogador: {question}";
                     case PromptType.Culprit:
                         return @$"Você é um personagem chamado {suspectSelected.Name}. Você é {suspectSelected.Description} e {suspectSelected.SystemPrompt}. 
             Você deve responder como se fosse esse personagem, não exagere no texto, máximo 2 paragrafos. Você não pode quebrar a quarta parede e não pode dizer que é um personagem de um jogo. 
             Se você não souber a resposta, você deve dizer que não sabe. Você pode escolher o silêncio. Você É O ASSASSINO, seu objetivo é não admitir o crime facilmente, aqui está o roteiro: {_CaseFile.CrimeDetails}. 
             Retorne para mim SOMENTE um objeto desserializável em formato JSON em texto puro dessa classe: public string? Text public string? Completed. Onde Text é a sua resposta e 
             Completed é o nome do objetivo como escrito, sua presença indica que o jogador o completou: {string.Join(",", _objectives.Select(kv => kv.Key))}. Se o jogador apresentar evidências 
             irrefutáveis do seu papel no crime e esse número {suspectSelected.StressLevel} for maior que 0.5, você foi pego e deve admitir e marcar como completo o objetivo relevante a confissão. 
            Esta é a pergunta do jogador: {question} .
            ";
                     default:
                         return null;
                 }
             }


          */













    }

    }
