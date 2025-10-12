using InvestigaIA.Model.Case;
using InvestigaIA.Model.Characters;
using InvestigaIA.Model.Game;

namespace InvestigaIA
{
    public interface IGameService
    {
        Task<CaseFile> CreateCaseFile(List<Suspect> suspects);
        Task<string> CreateCaseStory(CaseFile caseFile, List<Suspect> suspects);
        Task<List<Objective>> CreateObjectives(CaseFile caseFile);
    }
}