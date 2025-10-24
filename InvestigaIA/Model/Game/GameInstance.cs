using InvestigaIA.API;
using InvestigaIA.API.Gemini;
using InvestigaIA.Model.Case;
using InvestigaIA.Model.Characters;
using InvestigaIA.Model.Utilities;
// Removed Spectre.Console
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json;
using System.Linq;



namespace InvestigaIA.Model.Game
{
    public class GameInstance : IDisposable
    {

        private readonly GeminiService _geminiService;

        private readonly GameService _gameService;

        private readonly IHubContext<GameHub> _hubContext;

        private bool disposedValue;


        public List<Suspect> Suspects { get; set; }

        public CaseFile CaseFile { get; set; }


        public Dictionary<int, List<ChatMessage>> Chats { get; set; } = new();


        public List<Objective> Objectives { get; set; } = [new Objective { MainObjective = "Encontrar o assassino", Completed = false, Id = Guid.NewGuid() }];

        public DateTime CreatedAt { get; } = DateTime.Now;

        public bool Public = true;

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





        public async Task<MessageAnswer> Ask(AskRequest request)
        {
            try
            {
                var suspect = Suspects.Find(s => s.Id.ToString() == request.SuspectID) ?? throw new Exception("Suspect not found");

                if (!Chats.TryGetValue(suspect.Id, out var chat))
                {
                    Chats.Add(suspect.Id, new List<ChatMessage>());

                    chat = Chats[suspect.Id];
                }

                chat = Chats[suspect.Id];



                
                var newChatMessage = new ChatMessage(request.Sender, request.SenderID, request.Message, MessageType.User);

                chat.Add(newChatMessage);

               await _hubContext.Clients.Group(request.ChatId).SendAsync("Send", newChatMessage);

                string systemPrompt = GenerateCharacterPrompt(suspect);

                var response = await _geminiService.SendPromptAsync<MessageAnswer>(request.Message, systemPrompt, suspect, Objectives);

                var newChatResponse = new ChatMessage(suspect.Name, suspect.Id.ToString(), response.Text ?? "", MessageType.Model);

                chat.Add(newChatResponse);

                await _hubContext.Clients.Group(request.ChatId).SendAsync("Send", newChatResponse);

                return response;

            }
            catch (Exception ex)
            {
                throw;
            }


        }




        private string GenerateCharacterPrompt(Suspect suspect) =>

            $@"
                Você é {suspect.Name}, um personagem em um jogo de mistério. Você é {suspect.Description}.
                {suspect.Personality}

                **Regras Essenciais do Jogo:**
                - Responda estritamente como o personagem {suspect.Name}. Mantenha a consistência da sua personalidade, emoções e motivações.
                - Você não pode quebrar a quarta parede. Nunca diga que você é um personagem de um jogo ou que está em um enredo.
                - O jogador é um detetive investigando o crime. Respeite a autoridade dele, mas sem ser submisso. Seu tom deve ser consistente com sua descrição (ex: defensivo, arrogante, assustado, etc.).
                - Se você é o assassino, **não admita o crime** em hipótese alguma. Mantenha-se evasivo e negue qualquer envolvimento, não importa a pergunta.
                - Se você não souber a resposta para a pergunta do jogador, responda que não sabe ou se recuse a responder. Não retorne um texto vazio.
                - Mantenha as respostas concisas, mas detalhadas o suficiente para parecerem naturais. Evite respostas excessivamente longas ou genéricas.

                **Contexto do Caso:**
                - Um assassinato acabou de acontecer. Leve isso em consideração em suas respostas.
                - O enredo do jogo é: {CaseFile.CrimeDetails}. Não revele detalhes do enredo que você não deveria saber.

                **Instruções de Saída (Formato):**
                - Inclui um campo 'Completed' para sua resposta. Se um objetivo for completo, adicionei seu Id no campo 'Completed'. Caso contrário, deixe-o vazio ou nulo.
                - Inclui um campo'NewObjectives' para quaisquer novos objetivos que o jogador desbloqueou com esta interação. Se nenhum, deixe vazio.
                - Não inclua nada além da resposta do personagem no campo Text. 
                - Caso sua resposta tenha uma informação relevante para o jogador, inclua-a no campo Notes.
                - Se o jogador apresentar evidências irrefutáveis do seu papel no crime, você foi pego e deve admitir e marcar a propriedade CaseClosed como true.
                - Use os IDs desta lista: {string.Join(",", Objectives)}.
            ";















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
