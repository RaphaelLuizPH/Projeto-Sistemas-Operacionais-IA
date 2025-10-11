using InvestigaIA.API;
using InvestigaIA.API.Gemini;
using InvestigaIA.Model.Case;
using InvestigaIA.Model.Characters;
// Removed Spectre.Console
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using System.Linq;
using Newtonsoft.Json;



namespace InvestigaIA.Model.Game
{
    public class GameInstance
    {

        private readonly GeminiService geminiService;


        private readonly OpenAiService openAiService;

      
        public EndGameStats EndGameStats { get; set; }

        public  Dictionary<string, List<string>> Evidences { get; set; } = new();
        public List<Suspect> Suspects { get => suspects; set => suspects = value; }

        public CaseFile CaseFile { get; set; }

  
        public List<Objective> Objectives { get; set; }

        public DateTime CreatedAt { get; } = DateTime.Now;

        private readonly IHubContext<GameHub> _hubContext;

        [JsonIgnore]
        private List<Suspect> suspects = [];

        private string GameId { get; set; }


        public GameInstance(GeminiService service, string _gameId, IHubContext<GameHub> hubContext)
        {
       

            geminiService = service;

            GameId = _gameId;

            _hubContext = hubContext;


            suspects = CharacterSet.suspects;


            CaseFile = new CaseFile(ref suspects);

            CaseFile.CrimeDetails = CreateCaseStory().Result;

            Objectives = CreateObjectives().Result;

        }



        public async Task<List<Suspect>> CreateSuspects()
        {

            try
            {
                var suspectsDTOs = await geminiService.SendRequestAsync<List<SuspectDTO>>($@"Crie 8 personagens para um jogo de detetive que passa numa mansão (a mansão Blackwood). Cada personagem deve ter um nome, descrição e prompt de sistema.
           O prompt de sistema deve ser uma frase curta que descreve o papel do personagem no jogo. VocÊ não deve colocar o apelido de personagens entre aspas e nem utilizar aspas de modo que quebre o JSON.
           Os personagens devem ser únicos e interessantes, com diferentes origens e personalidades. Não defina o papel do personagem (como vítima, jogador, assassino). Você deve fornecer
           um valor para ImageCode, que deve ser o sexo do personagem + um numero de 1 a 8. (por exemplo: male1, female2, etc. em ordem aleatória, não linear).");


                Suspects = [.. suspectsDTOs.Select(s => new Suspect()
                {
                    Name = s.Name,
                    Description = s.Description,
                    SystemPrompt = s.SystemPrompt,
                    ImageCode = s.ImageCode,
                    StressLevel = 0.0

                })];

                return Suspects;
            }
            catch
            {
                throw;
            }


        }



        public async Task<List<Objective>> CreateObjectives()
        {
            try
            {
                var objectives = await geminiService.SendRequestAsync<List<Objective>>($@"Baseado nos personagens criados, crie uma lista de objetivos que o jogador deve cumprir para resolver o mistério do assassinato. Todos os objetivos devem ser
                       alcançados através de interrogatórios com os personagens e guiar o jogador ao culpado, este é o roteiro: {CaseFile.CrimeDetails}. Os objetivos não devem revelar partes da trama.");


                return objectives;
            }
            catch
            {
                throw;
            }
        }


        public async Task<CaseFile> CreateCaseFile()
        {

            try
            {
               var caseFile = await geminiService.SendRequestAsync<CaseFile>($@"Na Mansão Blackwood, 
                O patriarca da mansão foi assinado, com base nos seguintes personagens 
                crie um enredo sobre como ocorreu o crime de forma que o jogador seja capaz de desventar o crime através de interrogatórios. 
                {String.Join(";", suspects.Select(s => new { s.Name, s.Description }))  }. ");


                return caseFile;

            } catch(Exception ex)
            {
                throw;
            }



        }


        public async Task<string> CreateCaseStory()
        {

            try
            {
                var caseFile = await geminiService.SendRequestAsync($@"
                {CaseFile.ToString()}
                O patriarca da mansão foi assassinado, com base nos seguintes personagens 
                preencha crie o O Enredo do caso de forma que o jogador seja capaz de desventar o 
                crime através de interrogatórios. 
                Não precisa explicar o que é o jogo nem repetir a descrição de nenhum personagem. 
                Apenas crie a história do crime do inicio ao fim seguindo a descrição curta. 
                Exemplo: O personagem X estava na biblioteca quando ouviu um tiro, etc, etc.
                { JsonConvert.SerializeObject(suspects.Select(s => new { s.Name, Description = s.Description[..100] }))  }. ");


                return caseFile;

            }
            catch (Exception ex)
            {
                throw;
            }



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
