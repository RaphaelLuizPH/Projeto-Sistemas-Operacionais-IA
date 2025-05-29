using Microsoft.Extensions.Configuration;
using InvestigaIA.API;
using InvestigaIA.Classes;
using Spectre.Console;
using TheInterrogatorAIDetective.Models;

namespace InvestigaIA.Classes
{
    public class GameManager
    {
        private readonly IConfiguration config;
        private readonly GeminiService GeminiClient;
        private readonly Random random1 = new();
        private readonly Random random2 = new();
        private readonly Dictionary<string, List<string>> Evidences = new();
        private TimeSpan time = new TimeSpan(0, 0, 10, 0, 0);
        private List<Suspeito> suspects;
        private CaseFile _CaseFile;
        private Layout layout;
        private bool isQuestioning = false;

        public GameManager()
        {
            config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string apiKey = config["APIKey"];
            string apiUrl = config["APIUrl"];

            GeminiClient = new GeminiService(
                APIKey: apiKey,
                APIUrl: apiUrl
            );
        }

        public async Task RunAsync()
        {
            var init = new APIRequest(@"Crie 10 personagens para um jogo de detetive que passa num trem nos últimos anos do século 17. Cada personagem deve ter um nome, descrição e prompt de sistema.
 O prompt de sistema deve ser uma frase curta que descreve o papel do personagem no jogo. VocÊ não deve colocar o apelido de personagens entre aspas e nem utilizar aspas de modo que quebre o JSON.
 Os personagens devem ser únicos e interessantes, com diferentes origens e personalidades. Não defina o papel do personagem (como vítima, jogador, assassino).
 Retorne para mim um objeto desserializável com os personagens em formato JSON. Esta é a classe para referência:
     public string Name { get; set; }
     public string Description { get; set; }
     public string SystemPrompt { get; set; }
 ");

            var res = await GeminiClient.SendRequestAsync(init).ContinueWith(task =>
            {
                var response = task.Result;
                return response.candidates[0].content.parts[0].text;
            });

            var cleanRes = ResponseCleanUpUtility.CleanUpResponse(res);
            suspects = System.Text.Json.JsonSerializer.Deserialize<List<Suspeito>>(cleanRes);
            _CaseFile = new CaseFile(suspects[random1.Next(suspects.Count)], suspects[random2.Next(suspects.Count)], suspects);

            _CaseFile.Display();
            layout = new Layout("Root")
                .SplitColumns(
                    new Layout("Left"),
                    new Layout("Right")
                        .SplitRows(
                            new Layout("Top"),
                            new Layout("Bottom")));

            var suspectNames = suspects.Select(s => s.Name).ToList();
            string nameAcc = string.Empty;
            foreach (var name in suspectNames)
            {
                nameAcc = $"{nameAcc} [yellow]{name}[/],";
                layout["Right"]["Top"].Update(
                    new Panel(
                        Align.Left(
                            new Markup($"{nameAcc}"),
                            VerticalAlignment.Middle))
                        .Expand());
            }
            layout["Right"]["Bottom"].Update(
                       new Panel(
                           Align.Center(
                               new Markup($"[bold red]Tempo restante: {time.Minutes:00}:{time.Seconds:00}[/]"),
                               VerticalAlignment.Middle))
                           .Expand());

            Thread timer = new Thread(() =>
            {
                while (time.TotalSeconds > 0)
                {
                    Thread.Sleep(1000);
                    time = time.Subtract(TimeSpan.FromSeconds(1));
                    layout["Right"]["Bottom"].Update(
                        new Panel(
                            Align.Center(
                                new Markup($"[bold red]Tempo restante: {time.Minutes:00}:{time.Seconds:00}[/]"),
                                VerticalAlignment.Middle))
                            .Expand());
                }
            });

            Thread stresser = new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(5000);
                    foreach (var suspect in suspects)
                    {
                        if (suspect.StressLevel > 0.8)
                        {
                            suspect.StressLevel -= 0.1d;
                        }
                    }
                    layout["Left"].Update(
                new Panel(
                    Align.Center(
                        new BarChart().Width(60)
                .Label("[green bold underline]Nivel de estresse[/]")
                .CenterLabel()
                .AddItems(suspects),
                        VerticalAlignment.Middle))
                    .Expand());
                }
            });

            stresser.Start();
            timer.Start();

            layout["Left"].Update(
                new Panel(
                    Align.Center(
                        new BarChart().Width(60)
                .Label("[green bold underline]Nivel de estresse[/]")
                .CenterLabel()
                .AddItems(suspects),
                        VerticalAlignment.Middle))
                    .Expand());

            var suspect = string.Empty;
            while (true)
            {
                layout["Left"].Update(
                new Panel(
                    Align.Center(
                        new BarChart().Width(60)
                .Label("[green bold underline]Nivel de estresse[/]")
                .CenterLabel()
                .AddItems(suspects),
                        VerticalAlignment.Middle))
                    .Expand());
                AnsiConsole.Write(layout);
                if (Console.IsInputRedirected)
                {
                    suspect = suspectNames[random1.Next(0, suspectNames.Count)];
                }
                else
                {
                    AnsiConsole.MarkupLine("\n\n");
                    suspect = AnsiConsole.Prompt(
                       new SelectionPrompt<string>().Title("Qual personagem você quer interrogar?")
                           .AddChoices(suspectNames).PageSize(10));
                    AnsiConsole.MarkupLine("\n\n");
                }
                var suspectSelected = suspects.FirstOrDefault(s => s.Name.Contains(suspect));
                isQuestioning = true;
                AnsiConsole.Clear();
                while (isQuestioning)
                {
                    var choice = AnsiConsole.Prompt(
                       new SelectionPrompt<string>().Title("O que você quer fazer?")
                           .AddChoices(new[] { "Voltar", "Investigar", "Perguntar" }).PageSize(3));
                    if (choice == "Investigar")
                    {
                        var suspectClues = AnsiConsole.Prompt(
                       new SelectionPrompt<string>().Title("Escolha um suspeito: ")
                           .AddChoices(Evidences.Select(s => s.Key)).PageSize(10));
                        var clue = AnsiConsole.Prompt(
                       new SelectionPrompt<string>().Title("Evidências")
                           .AddChoices(Evidences[suspectClues]).PageSize(10));
                    }
                    if (choice == "Voltar")
                    {
                        isQuestioning = false;
                    }
                    if (choice == "Perguntar")
                    {
                        AnsiConsole.Markup($"Você está interrogando [aqua]{suspectSelected.Name}[/]\n");
                        AnsiConsole.Markup($"[green]Descrição: {suspectSelected.Description}[/]");
                        AnsiConsole.MarkupLine("");
                        var question = AnsiConsole.Prompt(new TextPrompt<string>("Escreva sua pergunta ou [red]/SAIR[/] para voltar: ")
                    .AllowEmpty()
                    .PromptStyle("green")
                    .Validate(s =>
                    {
                        if (s.ToUpper() == "/SAIR")
                        {
                            isQuestioning = false;
                            return ValidationResult.Success();
                        }
                        if (suspectSelected.StressLevel > 0.9)
                        {
                            return ValidationResult.Error($"[red]{suspectSelected.Name} está muito estressado e não vai responder perguntas[/]");
                        }
                        else if (string.IsNullOrWhiteSpace(s))
                        {
                            return ValidationResult.Error("[red]Você não pode fazer perguntas vazias![/]");
                        }
                        else
                        {
                            return ValidationResult.Success();
                        }
                    })
                    );
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
                        suspectSelected.StressLevel += 0.1d;
                        AnsiConsole.Markup($"[turquoise2] {suspectSelected.Name}: [/] {answer}");
                        AnsiConsole.MarkupLine("");
                        var action = AnsiConsole.Prompt(
                       new SelectionPrompt<string>().Title("Evidências")
                           .AddChoices(new[] { "Anotar evidência", "Voltar", "Perguntar novamente" }).PageSize(3));
                        if (action == "Voltar")
                        {
                            isQuestioning = false;
                        }
                        if (action == "Perguntar novamente")
                        {
                            continue;
                        }
                        if (action == "Anotar evidência")
                        {
                            var evidence = AnsiConsole.Prompt(
                    new SelectionPrompt<string>().Title("Evidências")
                   .AddChoices(answer.Trim().Replace("\n", " ").Split(".").Where(s => !string.IsNullOrWhiteSpace(s))).PageSize(10));
                            if (!Evidences.ContainsKey(suspectSelected.Name))
                            {
                                Evidences.Add(suspectSelected.Name, new List<string>() { evidence });
                            }
                            else
                            {
                                Evidences[suspectSelected.Name].Add(evidence);
                            }
                        }
                    }
                }
                AnsiConsole.Clear();
            }
        }
    }
}
