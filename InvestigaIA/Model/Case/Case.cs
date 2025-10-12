// Models/CaseFile.cs
using InvestigaIA.Model.Characters;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization; // Required for LINQ operations like Select

namespace InvestigaIA.Model.Case
{
    /// <summary>
    /// Represents the details of the crime case that the player needs to solve.
    /// Contains information about the crime, victim, evidence, and the actual culprit.
    /// </summary>
    public class CaseFile
    {
        /// <summary>
        /// Gets or sets the title of the case.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a detailed description of the crime that occurred.
        /// </summary>
        public string CrimeDetails { get; set; }

        /// <summary>
        /// Gets or sets a list of pieces of evidence found related to the crime.
        /// </summary>
        public List<string> Evidence { get; set; }

 
        [JsonInclude]
        public Culprit Culprit { get; set; }





        public string Motive { get; set; }
        public string Weapon { get; set; }
        public string Location { get; set; }


        public CaseFile(List<Suspect> suspects)
        {

            var random = new Random(); 
            

            var correctCulprit = suspects[random.Next(suspects.Count)];


            Weapon = CrimeWeapons[random.Next(CrimeWeapons.Count)];

    

            Motive = crimeMotives[random.Next(crimeMotives.Count)];

  

            Location = Locations[random.Next(Locations.Count)];


         
            Title = $"O caso de Thomas Blackwood";
            Culprit = new Culprit(correctCulprit.Name, correctCulprit.Description, correctCulprit.SystemPrompt, correctCulprit.ImageCode) { Id = correctCulprit.Id };
            Motive = crimeMotives[new Random().Next(crimeMotives.Count)];
            Weapon = CrimeWeapons[new Random().Next(CrimeWeapons.Count)];
            Location = Locations[new Random().Next(Locations.Count)];


            


        }


        public override string ToString()
        {
            return $"Na Mansão Blackwood, O Thomas S. Blackwood (Bilionário e patriarca da família) foi assassinado em {Location}. " +
                $"{Culprit.Name} ({Culprit.Description})  matou a vítima com {Weapon}. O motivo do crime foi {Motive}. "; ;
        }

        readonly List<string> CrimeWeapons =
[
"Faca de cozinha",
    "Revolver",
    "Chave inglesa",
    "Tesoura",
    "Veneno",
    "Cano de ferro",
    "Estátua de mármore",
    "Martelo",
    "Corda",
    "Arame",
    "Taco de beisebol",
    "Seringa",
    "Travesseiro",
    "Pé de cabra",
    "Chave de fenda",
    "Espada antiga",
    "Frasco de ácido",
    "Extintor de incêndio",
    "Garfo de churrasco",
    "Soco inglês",
    "Livro pesado",
    "Vara de pesca",
    "Abajur",
    "Cinto",
    "Bengala",
    "Garrafa quebrada"
];


        readonly List<string> crimeMotives = new()
        {
    "Herança disputada",
    "Ciúmes amoroso",
    "Vingança por uma traição antiga",
    "Segredo de família revelado",
    "Chantagem financeira",
    "Relacionamento extraconjugal descoberto",
    "Ambição por poder e controle",
    "Inveja do estilo de vida da vítima",
    "Rivalidade profissional",
    "Desprezo por humilhações passadas",
    "Desejo de se livrar de um obstáculo",
    "Briga por uma joia valiosa",
    "Conflito sobre a venda da mansão",
    "Silenciar um informante",
    "Rejeição de um pedido de casamento",
    "Manipulação por parte de um terceiro",
    "Descoberta de um testamento secreto",
    "Crime acidental durante uma discussão",
    "Traição no jogo de cartas",
    "Revanche por um caso antigo não resolvido"
};


        readonly List<string> Locations = new()
        {
"Sala de estar",
"Biblioteca",
"Escritório",
"Sala de jantar",
"Cozinha",
"Quarto principal",
"Quarto de hóspedes",
"Banheiro",
"Jardim de inverno",
"Porão",
"Sótão",
"Garagem",
"Hall de entrada",
"Varanda",
"Despensa",
"Salão de festas",
"Corredor",
"Terraço",
"Piscina coberta",
"Vestiário",
"Casa de máquinas",
"Capela",
"Estufa",
"Salão de jogos",
"Cinema particular"
};


        /// <summary>
        /// Displays the case file details in an attractive format using Spectre.Console.Panel.
        /// </summary>

    }
}
