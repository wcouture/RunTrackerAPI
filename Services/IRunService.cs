using RunTrackerAPI.Models;

namespace RunTrackerAPI.Services;

public interface IRunService
{
    Task<Run> CreateRun(Run run);
    Task<List<Run>> GetAllRuns();
    Task<Run> GetRunById(int id);
    Task<List<Run>> GetRunsByUserId(int userId);
    Task<IResult> UpdateRun(Run updatedRun, int id);
}