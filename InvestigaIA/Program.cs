using System.Net.Http;
using InvestigaIA.API;
using InvestigaIA.API.Gemini;
using InvestigaIA.Model.Characters;

Console.WriteLine("Starting InvestigaIA...");


var gemini = new GeminiService("AIzaSyApFBCT0lICs3nk5UC6IAZj5k_KASpjrRc");



var res = await gemini.SendPromptAsync <List<SuspectDTO>>(@"Crie 10 personagens para um jogo de detetive que passa numa mansão (a mansão Blackwood). Cada personagem deve ter um nome, descrição e prompt de sistema.
           O prompt de sistema deve ser uma frase curta que descreve o papel do personagem no jogo. VocÊ não deve colocar o apelido de personagens entre aspas e nem utilizar aspas de modo que quebre o JSON.
           Os personagens devem ser únicos e interessantes, com diferentes origens e personalidades. Não defina o papel do personagem (como vítima, jogador, assassino). Você deve fornecer
           um valor para ImageCode, que deve ser o sexo do personagem + um numero de 1 a 10. (por exemplo: male1, female2, etc. em ordem aleatória, não linear).");


var Suspects = res.Select(s => new Suspect()
{
    Name = s.Name,
    Description = s.Description,
    Personality = s.SystemPrompt,
    ImageCode = s.ImageCode,
}).ToList();


//var message = await gemini.SendRequestAsync("Olá, como se chama?", Suspects.First());


