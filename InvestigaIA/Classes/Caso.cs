// Models/CaseFile.cs
using InvestigaIA.Classes;
using Spectre.Console;
using System.Collections.Generic;
using System.Linq; // Required for LINQ operations like Select

namespace TheInterrogatorAIDetective.Models
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

        /// <summary>
        /// Gets or sets the name or description of the victim of the crime.
        /// </summary>
        public string Victim { get; set; }

        /// <summary>
        /// Gets or sets the name of the suspect who is the correct culprit for this case.
        /// This is used to verify the player's accusation.
        /// </summary>
        public string CorrectCulpritName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CaseFile"/> class.
        /// </summary>

        /// <param name="victim">The victim of the crime.</param>
        /// <param name="correctCulpritName">The name of the actual culprit.</param>
        public CaseFile(Suspeito victim, Suspeito correctCulpritName, List<Suspeito> suspects)
        {

            Victim = victim.Name;
            Title = $"O caso de {victim.Name}";
            CorrectCulpritName = correctCulpritName.Name;
            Kill(correctCulpritName, suspects);
        }


        private void Kill(Suspeito suspeito, List<Suspeito> suspects)
        {
            suspects.Remove(suspeito);
        }


        /// <summary>
        /// Displays the case file details in an attractive format using Spectre.Console.Panel.
        /// </summary>
      
    }
}
