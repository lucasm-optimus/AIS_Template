namespace Tilray.Integrations.Core.Application.Adapters.Repositories;

public interface ISnowflakeRepository
{
    Task<string> GetAcctCodeAsync(string segment0, string segment1);
    Task<IEnumerable<GrpoDetails>> GetGrpoDetailsAsync(
        string baseDocNum, string baseEntry, string docEntry, string lineNum);
}
