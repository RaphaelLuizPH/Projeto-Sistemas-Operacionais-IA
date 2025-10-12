using InvestigaIA.API.Gemini;
using InvestigaIA.Model.Case;
using InvestigaIA.Model.Characters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InvestigaIA.Model.Game
{
    public class GameService(GeminiService geminiService)
    {

        private readonly GeminiService geminiService = geminiService;



     



        public async Task<List<Objective>> CreateObjectives(CaseFile caseFile)
        {
            try
            {
                var objectives = await geminiService.SendRequestAsync<List<ObjectiveDTO>>($@"Com base no seguinte enredo de assassinato, crie uma lista de objetivos de jogo que guiem o jogador a resolver o mistério.

    **Enredo do Jogo:**
    (Insira aqui o enredo detalhado que a IA gerou anteriormente)

    **Sua Tarefa:**
    Crie uma lista de objetivos que o jogador deve cumprir para identificar o assassino. A lista deve ser projetada para ser exibida na interface do jogo.

    **Regras para os Objetivos:**
    1.  **Formato:** Cada objetivo deve ser uma frase curta e direta, sem revelar a trama. Os objetivos devem ser alcançados através de interrogatórios.
    2.  **Mecanismo de Desbloqueio:** Os objetivos devem ser alcançados por meio de interrogatórios com os personagens. Eles devem ser interconectados, onde a conclusão de um objetivo leva ao desbloqueio de um ou mais novos objetivos.
    3.  **Evite Spoilers:** O texto do objetivo não pode revelar a identidade do assassino, o motivo do crime ou qualquer segredo central. Por exemplo, em vez de 'Descobrir que X é o assassino', use 'Reunir todas as evidências contra X'.
    4.  **Guia Subtil:** A lista de objetivos deve guiar o jogador para interrogar todos os suspeitos, questionar seus álibis e descobrir as pistas e contradições que você criou no enredo.
    5.  **Use IDs:** Atribua um ID único a cada objetivo. A lista de saída deve ser formatada como uma lista de IDs e descrições.");



                

                return objectives.Select(o => new Objective()
                {
                    
                    MainObjective = o.MainObjective,
                    Completed = false,

                }).ToList();
              
            }
            catch
            {
                throw;
            }
        }





        public async Task<string> CreateCaseStory(CaseFile caseFile, List<Suspect> suspects)
        {

            try
            {
                var story = await geminiService.SendRequestAsync($@"
    Você é um roteirista de jogos de mistério. Seu objetivo é criar um enredo para um jogo de assassinato.
    O cenário é a Mansão Blackwood, e o patriarca da família foi assassinado.

    **Personagens Suspeitos:**
    {{String.Join("";"", suspects.Select(s => new {{ s.Name, s.Description }}))}}

    **Sua Tarefa:**
    Crie um enredo de assassinato que possa ser resolvido por um jogador através de interrogatórios. O enredo deve ser lógico e conter todas as informações necessárias para a resolução.

    **Estrutura do Enredo (Responda APENAS com esta estrutura):**
    1.  **Vítima e Circunstâncias do Crime:** Descreva brevemente o patriarca da família (personalidade, segredos, etc.) e como o crime aconteceu (local, hora aproximada, etc.).
    2.  **Assassino e Motivo:** Escolha um dos personagens para ser o assassino. Crie um motivo forte e pessoal para o crime que se conecte diretamente com a vítima. O motivo deve ser um segredo que o assassino tenta esconder.
    3.  **Álibis dos Suspeitos:** Para cada personagem suspeito, incluindo o assassino, crie um álibi convincente para o momento do crime. Os álibis devem ter pequenos furos ou contradições que um investigador experiente notaria.
    5.  **Segredos dos Suspeitos (Não Relacionados ao Crime):** Para cada suspeito, crie um segredo que não seja o motivo do assassinato. Esses segredos devem ser interessantes e revelados durante o interrogatório, adicionando profundidade ao personagem.

    **Importante:** A resposta deve ser uma narrativa detalhada que conecta todos esses pontos, mas apresentada de forma clara e organizada, seguindo a estrutura acima. Não se limite a uma lista de tópicos; conte a história de como tudo se encaixa. O jogador deve ser capaz de descobrir a verdade com base nessas informações.
");


                return story;

            }
            catch (Exception ex)
            {
                throw;
            }



        }




    }
}
