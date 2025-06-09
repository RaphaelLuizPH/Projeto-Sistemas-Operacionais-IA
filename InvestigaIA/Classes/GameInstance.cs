using Microsoft.Extensions.Configuration;
using InvestigaIA.API;
using InvestigaIA.Classes;
// Removed Spectre.Console
using TheInterrogatorAIDetective.Models;
using Microsoft.AspNetCore.SignalR;
using System.Text.Json;
using System.Text.Json.Serialization;



namespace InvestigaIA.Classes
{
    public class GameInstance
    {
        private bool _running = false;
        private readonly GeminiService GeminiClient;




        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly OpenAiService openAiService;

        public EndGameStats endGameStats { get; set; }
        private readonly Random random1 = new();
        private readonly Random random2 = new();
        public readonly Dictionary<string, List<string>> Evidences = new();
        private TimeSpan time = new(0, 0, 0);
        public List<Suspeito> suspects;
        public CaseFile _CaseFile;

        [JsonInclude]
        public Dictionary<string, bool> _objectives = new();

        public DateTime CreatedAt { get; } = DateTime.Now;

        private readonly IHubContext<GameHub> _hubContext;

        private string gameId;
        public GameInstance(GeminiService geminiClient)
        {
            GeminiClient = geminiClient ?? throw new ArgumentNullException(nameof(geminiClient));


        }


        public GameInstance(GeminiService geminiClient, string _gameId, IHubContext<GameHub> hubContext, OpenAiService _openAiService)
        {
            openAiService = _openAiService;
            GeminiClient = geminiClient;
            gameId = _gameId;
            _hubContext = hubContext;

        }






        private Thread? timerThread;
        private Thread? stresserThread;





        private string GeneratePrompt(PromptType type)
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



        public async Task<EndGameStats> EndGame(Suspeito acusado)

        {
            if (endGameStats != null)
                return endGameStats;



            try
            {
                await _semaphore.WaitAsync();

                var newStats = new EndGameStats
                {
                    Time = time,
                    CaseFile = _CaseFile,
                    Acusado = acusado,
                    Objetivos = _objectives.Where(kv => kv.Value == true).ToDictionary(kv => kv.Key, kv => kv.Value),
                };

                _running = false;
                if (acusado.Name == _CaseFile.Victim.Name)
                {
                    var response = await GeminiClient.SendRequestAsync(new APIRequest($@"O investigador fez um palpite incorreto, esse é o fim do jogo. Ele chegou a conclusão de que o caso foi um suícidio, 
                deixando assim o assassino se safar. Você deve agora escrever as consequências dessa ação. Lembra que esses são os personagens da história: {string.Join(",", suspects.Select(s => s.Name))} e 
                esse é o roteiro {_CaseFile.CrimeDetails}")).ContinueWith(task =>
                     {
                         var result = task.Result;
                         return result.candidates[0].content.parts[0].text;
                     });
                    newStats.Message = response;

                    endGameStats = newStats;
                    return endGameStats;
                }


                if (acusado.Name == _CaseFile.Culpado.Name)
                {


                    if (_objectives.Select(kv => kv.Value).Where(x => x == false).Count() > _objectives.Count / 2)
                    {
                        var response = await GeminiClient.SendRequestAsync(new APIRequest($@"O investigador fez um palpite correto, esse é o fim do jogo, mas não conseguiu completar os objetivos necessários para resolver o caso. 
                    O culpado é {_CaseFile.Culpado.Name} e a vítima é {_CaseFile.Victim.Name}. Você deve agora escrever as consequências dessa ação. Lembra que esses são os personagens da história: {string.Join(",", suspects.Select(s => s.Name))} e 
                    esse é o roteiro {_CaseFile.CrimeDetails}, retorne em texto simples, sem negrito ou italico, paragrafos separados por ponto final.")).ContinueWith(task =>
                         {
                             var result = task.Result;
                             return result.candidates[0].content.parts[0].text;
                         });
                        newStats.Message = response;

                        endGameStats = newStats;
                        return endGameStats;
                    }
                    else
                    {
                        if (_objectives["Conseguir uma confissão do assassino"])
                            _CaseFile.Culpado.Caught = true;


                        var response = await GeminiClient.SendRequestAsync(new APIRequest($@"O investigador fez um palpite correto, esse é o fim do jogo, e conseguiu completar os objetivos necessários para resolver o caso. 
                    O culpado é {_CaseFile.Culpado.Name} e a vítima é {_CaseFile.Victim.Name}. Você deve agora escrever as consequências dessa ação. Lembra que esses são os personagens da história: {string.Join(",", suspects.Select(s => s.Name))} e 
                    esse é o roteiro {_CaseFile.CrimeDetails}, retorne em texto simples, sem negrito ou italico, paragrafos separados por ponto final.")).ContinueWith(task =>
                    {
                        var result = task.Result;
                        return result.candidates[0].content.parts[0].text;
                    });
                        newStats.Message = response;
                        newStats.JusticeServed = true;
                        endGameStats = newStats;
                        return endGameStats;
                    }

                }

                else
                {
                    var response = await GeminiClient.SendRequestAsync(new APIRequest($@"O jogador fez um palpite incorreto. Ele chegou a conclusão de que o culpado é {acusado.Name}, 
                mas na verdade o culpado é {_CaseFile.Culpado.Name}. Você deve agora escrever as consequências dessa ação. Lembra que esses são os personagens da história: {string.Join(",", suspects.Select(s => s.Name))} e 
                esse é o roteiro {_CaseFile.CrimeDetails}, retorne em texto simples, sem negrito ou italico, paragrafos separados por ponto final.")).ContinueWith(task =>
                     {
                         var result = task.Result;
                         return result.candidates[0].content.parts[0].text;
                     });
                    newStats.Message = response;


                    endGameStats = newStats;
                    return endGameStats;
                }

            }
            finally
            {
                _semaphore.Release();
            }




        }


        public async Task StartGame()
        {

            _running = true;
            await CreateSuspects();
            Kill();


            _CaseFile.CrimeDetails = GeneratePrompt(PromptType.CaseDetails);
            _objectives = await CreateObjectives();
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

            var culprit = new Culprit(correctCulprit.Name, correctCulprit.Description, correctCulprit.SystemPrompt);


            _CaseFile = new CaseFile(victim, culprit, suspects);
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
                    if (suspect.StressLevel >= 0.5)
                    {
                        suspect.StressLevel -= 0.1d;
                        await _hubContext.Clients.Group(gameId).SendAsync("ReceiveMessage", $"Stress levels balanced for suspects.");
                    }
                }



            }
        }



        private async Task<Dictionary<string, bool>> CreateObjectives()

        {
            var objectivesRequest = new APIRequest(GeneratePrompt(PromptType.Objectives));

            var response = await GeminiClient.SendRequestAsync(objectivesRequest).ContinueWith(task =>
            {
                var result = task.Result;
                return result.candidates[0].content.parts[0].text;
            });

            var cleanedResponse = ResponseCleanUpUtility.CleanUpResponse(response);


            try
            {



                var objectives = JsonSerializer.Deserialize<Dictionary<string, bool>>(cleanedResponse);
                if (objectives == null)
                {
                    throw new Exception("Failed to deserialize objectives from the response.");
                }

                objectives.Add("Conseguir uma confissão do assassino", false);
                return objectives;
            }
            catch
            {
                throw new Exception(cleanedResponse);
            }



        }

        private async Task CreateSuspects()
        {
            var init = new APIRequest(GeneratePrompt(PromptType.Suspects));


            var res = await GeminiClient.SendRequestAsync(init).ContinueWith(task =>
            {
                var response = task.Result;
                return response.candidates[0].content.parts[0].text;
            });

            var cleanRes = ResponseCleanUpUtility.CleanUpResponse(res);

            try
            {
                await _semaphore.WaitAsync();

                var deserializedSuspects = JsonSerializer.Deserialize<List<Suspeito>>(cleanRes);
                if (deserializedSuspects == null || !deserializedSuspects.Any())
                {
                    throw new Exception("Failed to deserialize suspects or the list is empty.");
                }

                suspects = deserializedSuspects;

            }
            catch (Exception ex)
            {
                throw new Exception($"{ex.Message} - {res}, {cleanRes}");
            }
            finally
            {
                _semaphore.Release();
            }
        }


        public async Task<List<Content>> SendMessageGPT(string question, Suspeito suspectSelected)
        {
            string message;


            if (suspectSelected.Name == _CaseFile.Culpado.Name)
            {
                message = GeneratePrompt(PromptType.Culprit, suspectSelected, question);


            }
            else if (suspectSelected.Name == _CaseFile.Victim.Name)
            {
                message = GeneratePrompt(PromptType.Victim, suspectSelected, question);


            }
            else
            {
                message = GeneratePrompt(PromptType.Suspect, suspectSelected, question);


            }


            var res = await openAiService.Ask(message, suspectSelected);
            res = ResponseCleanUpUtility.CleanUpResponse(res);

            try
            {
                await _semaphore.WaitAsync();
                var resObj = JsonSerializer.Deserialize<MessageAnswer>(res);

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
                },
                new Content
                {
                role = "assistant",
                parts =
                [
                new Part { text = resObj.Text }
                ]
                }
                }
                );
                await _hubContext.Clients.Group(gameId).SendAsync("Response", resObj);
                if (!string.IsNullOrEmpty(resObj.Completed))
                {


                    if (_objectives.TryGetValue(resObj.Completed.Trim(), out bool value))
                    {
                        _objectives[resObj.Completed] = true;
                        await _hubContext.Clients.Group(gameId).SendAsync("Objectives", _objectives);

                        if (_objectives["Conseguir uma confissão do assassino"])
                            _CaseFile.Culpado.Caught = true;

                    }


                }



                suspectSelected.StressLevel += 0.1d;
                await _hubContext.Clients.Group(gameId).SendAsync("Conversation", new { suspectSelected._conversationHistory, suspectSelected.StressLevel });
                return suspectSelected._conversationHistory;
            }
            catch (JsonException ex)
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
                },
                new Content
                {
                role = "assistant",
                parts =
                [
                new Part { text = "..." }
                ]
                }
              }
              );
                await _hubContext.Clients.Group(gameId).SendAsync("Conversation", new { suspectSelected._conversationHistory, suspectSelected.StressLevel });
                return suspectSelected._conversationHistory;
            }
            finally
            {

                _semaphore.Release();
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

            try
            {

                await _semaphore.WaitAsync();
                APIRequest request;

                if (suspectSelected.Name == _CaseFile.Culpado.Name)
                {
                    var prompt = GeneratePrompt(PromptType.Culprit, suspectSelected, question);
                    request = new APIRequest(prompt);
                }
                else if (suspectSelected.Name == _CaseFile.Victim.Name)
                {
                    var prompt = GeneratePrompt(PromptType.Victim, suspectSelected, question);
                    request = new APIRequest(prompt);
                }
                else
                {
                    var prompt = GeneratePrompt(PromptType.Suspect, suspectSelected, question);
                    request = new APIRequest(prompt);
                }

                var answer = await GeminiClient.Ask(request, suspectSelected);

                var objetive = _objectives.TryGetValue(answer.Completed, out bool isCompleted);

                if (objetive)
                {
                    _objectives[answer.Completed] = true;
                    await _hubContext.Clients.Group(gameId).SendAsync("Objectives", _objectives);
                }

                suspectSelected.StressLevel += 0.1d;

                return suspectSelected._conversationHistory;



            }
            catch (JsonException ex)
            {
                suspectSelected._conversationHistory.Add(new Content()
                {
                    role = "model",
                    parts = { new Part() { text = "..." } }
                });


                return suspectSelected._conversationHistory;
            }
            finally
            {
                _semaphore.Release();
            }
        }





    }
}
