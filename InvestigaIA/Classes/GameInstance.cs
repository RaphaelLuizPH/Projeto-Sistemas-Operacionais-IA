using Microsoft.Extensions.Configuration;
using InvestigaIA.API;
using InvestigaIA.Classes;
// Removed Spectre.Console
using TheInterrogatorAIDetective.Models;
using Microsoft.AspNetCore.SignalR;

namespace InvestigaIA.Classes
{
    public class GameInstance
    {
        private bool _running = false;
        private readonly GeminiService GeminiClient;
        private readonly Random random1 = new();
        private readonly Random random2 = new();
        public readonly Dictionary<string, List<string>> Evidences = new();
        public TimeSpan time = new(0, 0, 0);
        public List<Suspeito> suspects;
        public CaseFile _CaseFile;

        public DateTime CreatedAt { get; } = DateTime.Now;

        private readonly IHubContext<GameHub> _hubContext;

        private string gameId;
        public GameInstance(GeminiService geminiClient)
        {
            GeminiClient = geminiClient ?? throw new ArgumentNullException(nameof(geminiClient));

        }


        public GameInstance(GeminiService geminiClient, string _gameId, IHubContext<GameHub> hubContext)
        {
            GeminiClient = geminiClient;
            gameId = _gameId;
            _hubContext = hubContext;
        }

        
        private Thread? timerThread;
        private Thread? stresserThread;

        

        public async Task StartGame()
        {

            _running = true;
            await CreateSuspects();
            Kill();


            _CaseFile.CrimeDetails = @$"Na Mansão Blackwood, {_CaseFile.Victim} ({suspects.First(s => s.Name == _CaseFile.Victim).Description}) foi assassinado na biblioteca. Aproveitando uma distração, {_CaseFile.CorrectCulpritName} ({suspects.First(s => s.Name == _CaseFile.CorrectCulpritName).Description}) surpreendeu e matou a vítima.
            {suspects[0].Name} dedilhava as teclas do piano, {suspects[1].Name} debatia acaloradamente um jogo, {suspects[5].Name} polia a prataria reluzente, 
            {suspects[6].Name} revisava anotações antigas, {suspects[7].Name} cantava ao som do piano, {suspects[3].Name} fumava à janela, 
            {suspects[8].Name} inspecionava a adega e {suspects[2].Name} estudava um livro.
            ";

            timerThread = new Thread(TimerThreadMethod);
            timerThread.IsBackground = true;
            timerThread.Start();

            stresserThread = new Thread(StressLevelBalanceThreadMethod);
            stresserThread.IsBackground = true;
            stresserThread.Start();






        }


        private void Kill()
        {

            var correctCulprit = suspects[random1.Next(suspects.Count)];
            Suspeito victim;

            do
            {
                victim = suspects[random2.Next(suspects.Count)];
            } while (correctCulprit.Name == victim.Name);




            _CaseFile = new CaseFile(victim, correctCulprit, suspects);
        }

        private async void TimerThreadMethod()
        {
            while (true)
            {
                Thread.Sleep(1000); // Wait for 1 second
                time = time.Add(TimeSpan.FromSeconds(1));

                await _hubContext.Clients.Group(gameId).SendAsync("ReceiveMessage", new { time = time.ToString(@"mm\:ss") });
            }
        }

        private async void StressLevelBalanceThreadMethod()
        {
            while (true)
            {
                Thread.Sleep(5000); // Wait for 5 seconds
                foreach (var suspect in suspects)
                {
                    if (suspect.StressLevel > 0.5)
                    {
                        suspect.StressLevel -= 0.1d;
                        await _hubContext.Clients.Group(gameId).SendAsync("ReceiveMessage", $"Stress levels balanced for suspects.");
                    }
                }



            }
        }


        private async Task CreateSuspects()
        {
            var init = new APIRequest(@"Crie 10 personagens para um jogo de detetive que passa numa mansão. Cada personagem deve ter um nome, descrição e prompt de sistema.
 O prompt de sistema deve ser uma frase curta que descreve o papel do personagem no jogo. VocÊ não deve colocar o apelido de personagens entre aspas e nem utilizar aspas de modo que quebre o JSON.
 Os personagens devem ser únicos e interessantes, com diferentes origens e personalidades. Não defina o papel do personagem (como vítima, jogador, assassino). Você deve fornecer
 um valor para ImageCode, que deve ser o sexo do personagem + um numero de 1 a 10. (por exemplo: male1, female2, etc. em ordem aleatória, não linear).
 Retorne para mim um objeto desserializável o qual deve ser uma lista com os personagens [obj {}, obj {}, obj {}] em formato JSON em texto puro sem bloco de código do markdown. Esta é a classe para referência:
     public string Name { get; set; }
     public string Description { get; set; }
     public string SystemPrompt { get; set; }
     public string ImageCode { get; set; }
 ");

            var res = await GeminiClient.SendRequestAsync(init).ContinueWith(task =>
            {
                var response = task.Result;
                return response.candidates[0].content.parts[0].text;
            });
            var cleanRes = ResponseCleanUpUtility.CleanUpResponse(res);
            try
            {


                suspects = System.Text.Json.JsonSerializer.Deserialize<List<Suspeito>>(cleanRes);
                _CaseFile = new CaseFile(suspects[random1.Next(suspects.Count)], suspects[random2.Next(suspects.Count)], suspects);

            }
            catch (Exception ex)
            {

                throw new Exception($"{ex.Message} - {res}, {cleanRes}");
            }


        }


        public async Task<List<Content>> SendMessage(string question, Suspeito suspectSelected)
        {
            suspectSelected._conversationHistory.AddRange(
                new List<Content>
                {
                    new Content
                    {
                        role = "user",
                        parts =
                    [
                        new Part { text = question }
                    ]
                    }
                }
            );

            APIRequest request;

            if (suspectSelected.Name == _CaseFile.CorrectCulpritName)
            {
                request = new APIRequest(
    @$"Você é um personagem chamado {suspectSelected.Name}. Você é {suspectSelected.Description} e {suspectSelected.SystemPrompt}. 
    Você deve responder como se fosse esse personagem, não exagere no texto, máximo 2 paragrafos. Você não pode quebrar a quarta parede e não pode dizer que é um personagem de um jogo. 
    Se você não souber a resposta, você deve dizer que não sabe. Você pode escolher o silêncio. Você É O ASSASSINO, seu objetivo é não ser pego, aqui está o roteiro: {_CaseFile.CrimeDetails}. 
    Esta é a pergunta do jogador: {question}"
);
            }
            else if (suspectSelected.Name == _CaseFile.Victim)
            {
                request = new APIRequest(
    @$"Você é um personagem chamado {suspectSelected.Name}. Você é {suspectSelected.Description} e {suspectSelected.SystemPrompt}. 
    Você deve interpretar a vítima de um assassinato. O que significa que suas respostas devem conter apenas descrições de caracteristicas do corpo, ambiente, etc. Seu personagem 
    não deve falar e deve seguir esse roteiro: {_CaseFile.CrimeDetails}. Esta é a pergunta do jogador: {question}"
);
            }
            else
            {
                request = new APIRequest(
    @$"Você é um personagem chamado {suspectSelected.Name}. Você é {suspectSelected.Description} e {suspectSelected.SystemPrompt}. Aqui está o roteiro do jogo: {_CaseFile.CrimeDetails},
    Você deve responder como se fosse esse personagem, não exagere no texto, máximo 2 paragrafos. Você não pode quebrar a quarta parede e não pode dizer que é um personagem de um jogo. 
    Se você não souber a resposta, você deve dizer que não sabe. Você pode escolher o silêncio. Você NÃO É O ASSASSINO, não admita independente do que o jogador perguntar. 
    Lembre-se que um assassinato acabou de acontecer e você não deve ignorar isso. O jogador é um investigador e sua autoridade deve ser respeitada. Esta é a pergunta do jogador: {question}"
);
            }




            var answer = await GeminiClient.Ask(request, suspectSelected).ContinueWith(task =>
    {
        var res = task.Result;
        return res.candidates[0].content;
    });


            suspectSelected.StressLevel += 0.1d;

            return suspectSelected._conversationHistory;


        }




    }
}
