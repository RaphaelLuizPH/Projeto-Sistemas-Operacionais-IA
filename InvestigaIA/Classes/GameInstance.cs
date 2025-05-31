using Microsoft.Extensions.Configuration;
using InvestigaIA.API;
using InvestigaIA.Classes;
// Removed Spectre.Console
using TheInterrogatorAIDetective.Models;

namespace InvestigaIA.Classes
{
    public class GameInstance
    {
        private bool _running = false;
        private readonly GeminiService GeminiClient;
        private readonly Random random1 = new();
        private readonly Random random2 = new();
        public readonly Dictionary<string, List<string>> Evidences = new();
        public TimeSpan time = new(0, 10, 0);
        public List<Suspeito> suspects;
        public CaseFile _CaseFile;

        public DateTime CreatedAt { get; } = DateTime.Now;



        public GameInstance(GeminiService geminiClient)
        {
            GeminiClient = geminiClient ?? throw new ArgumentNullException(nameof(geminiClient));

        }


        // Thread declarations at top level
        private Thread? timerThread;
        private Thread? stresserThread;

        // Method to start the timer thread

        public async Task StartGame()
        {
            _running = true;
            await CreateSuspects();


            timerThread = new Thread(TimerThreadMethod);
            timerThread.Start();
            stresserThread = new Thread(StressLevelBalanceThreadMethod);
            stresserThread.Start();

            while (_running)
            {

            }




        }


        private void Kill()
        {

            var correctCulprit = suspects[random1.Next(suspects.Count)];
            Suspeito victim;

            do
            {
                victim = suspects[random2.Next(suspects.Count)];
            } while (correctCulprit == victim);



            _CaseFile = new CaseFile(victim, correctCulprit, suspects);
        }

        private void TimerThreadMethod()
        {
            while (time.TotalSeconds > 0)
            {
                Thread.Sleep(1000); // Wait for 1 second
                time = time.Subtract(TimeSpan.FromSeconds(1));
            }
        }

        // Code to be run by the stress level balance thread
        private void StressLevelBalanceThreadMethod()
        {
            while (true)
            {
                Thread.Sleep(5000); // Wait for 5 seconds
                foreach (var suspect in suspects)
                {
                    if (suspect.StressLevel > 0.5)
                    {
                        suspect.StressLevel -= 0.1d;
                    }
                }
            }
        }


        public async Task CreateSuspects()
        {
            var init = new APIRequest(@"Crie 10 personagens para um jogo de detetive que passa num trem nos últimos anos do século 17. Cada personagem deve ter um nome, descrição e prompt de sistema.
 O prompt de sistema deve ser uma frase curta que descreve o papel do personagem no jogo. VocÊ não deve colocar o apelido de personagens entre aspas e nem utilizar aspas de modo que quebre o JSON.
 Os personagens devem ser únicos e interessantes, com diferentes origens e personalidades. Não defina o papel do personagem (como vítima, jogador, assassino). Você deve fornecer
 um valor para ImageCode, que deve ser o sexo do personagem + um numero de 1 a 10. (por exemplo: male1, female2, etc.).
 Retorne para mim um objeto desserializável com os personagens em formato JSON em texto puro sem bloco de código do markdown. Esta é a classe para referência:
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
            suspects = System.Text.Json.JsonSerializer.Deserialize<List<Suspeito>>(cleanRes);
            _CaseFile = new CaseFile(suspects[random1.Next(suspects.Count)], suspects[random2.Next(suspects.Count)], suspects);

        }


        public async Task<string> SendMessage(string question, Suspeito suspectSelected)
        {
            var answer = await GeminiClient.Ask(new APIRequest(
        @$"Você é um personagem chamado {suspectSelected.Name}. Você é {suspectSelected.Description} e {suspectSelected.SystemPrompt}. 
    Você deve responder como se fosse esse personagem. Você não pode quebrar a quarta parede e não pode dizer que é um personagem de um jogo. O assassino é {_CaseFile.CorrectCulpritName} 
    e você não pode dizer que ele é o assassino a menos que você de fato saiba disso. Se você não souber a resposta, você deve dizer que não sabe. Você pode escolher o silêncio. Se você for o assassino, você deve tentar enganar o jogador. E 
    só deve admitir ser o assassino se o jogador souber disso. Lembre-se que um assassinato acabou de acontecer no trem em que vocês estão e você não deve ignorar isso. O jogador é um investigador e sua autoridade deve ser respeitada. Esta é a pergunta do jogador: {question}"
    ), suspectSelected).ContinueWith(task =>
    {
        var res = task.Result;
        return res.candidates[0].content.parts[0].text;
    });


            return answer;


        }




    }
}
