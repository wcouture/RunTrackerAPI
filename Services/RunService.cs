using Microsoft.EntityFrameworkCore;
using RunTrackerAPI.Models;

namespace RunTrackerAPI.Services;

/// <summary>
/// Service class for managing Run entities in the database.
/// Provides CRUD operations and additional functionality for Run records.
/// </summary>
public class RunService : IRunService
{
    private RunDb _db {get; set;} = null!;

    /// <summary>
    /// Initializes a new instance of the RunService class.
    /// </summary>
    /// <param name="db">The database context for Run entities.</param>
    public RunService(RunDb db)
    {
        _db = db;
    }

    /// <summary>
    /// Retrieves all Run records from the database, including their associated Duration data.
    /// </summary>
    /// <returns>A Task containing a List of Run records.</returns>
    public async Task<List<Run>> GetAllRuns()
    {
        return await _db.RunData.Include(r => r.Duration).ToListAsync();
    }

    /// <summary>
    /// Retrieves a specific Run record by its unique identifier, including its associated Duration data.
    /// </summary>
    /// <param name="id">The unique identifier of the Run record to retrieve.</param>
    /// <returns>A Task containing the Run record if found.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the Run record does not exist.</exception>
    public async Task<Run> GetRunById(int id)
    {
        return await _db.RunData.Include(r => r.Duration).Where(r => r.Id == id).FirstAsync();
    }

    /// <summary>
    /// Retrieves all Run records associated with a specific user, including their associated Duration data.
    /// </summary>
    /// <param name="userId">The unique identifier of the user whose Run records are to be retrieved.</param>
    /// <returns>A Task containing a List of Run records associated with the specified user.</returns>
    public async Task<List<Run>> GetRunsByUserId(int userId)
    {
        return await _db.RunData.Include(r => r.Duration).Where(r => r.UserId == userId).ToListAsync();
    }

    /// <summary>
    /// Updates an existing Run record with new data.
    /// </summary>
    /// <param name="updatedRun">The updated Run object containing the new data.</param>
    /// <param name="id">The unique identifier of the Run record to update.</param>
    /// <returns>A Task containing an IResult indicating the result of the update operation.</returns>
    /// <remarks>
    /// Returns Results.NoContent() if the update is successful.
    /// Returns Results.NotFound() if the Run record does not exist.
    /// </remarks>
    public async Task<IResult> UpdateRun(Run updatedRun, int id)
    {
        var run = await _db.RunData.FindAsync(id);
        if (run is null) return Results.NotFound();

        run.Label = updatedRun.Label;
        if (run.Duration is not null)
        {
            run.Duration.Hours = updatedRun.Duration.Hours;
            run.Duration.Minutes = updatedRun.Duration.Minutes;
            run.Duration.Seconds = updatedRun.Duration.Seconds;
        }
        else
        {
            run.Duration = new Duration() { Hours = updatedRun.Duration.Hours, Minutes = updatedRun.Duration.Minutes, Seconds = updatedRun.Duration.Seconds };
        }
        run.Mileage = updatedRun.Mileage;

        await _db.SaveChangesAsync();
        return Results.NoContent();
    }

    /// <summary>
    /// Creates a new Run record in the database.
    /// </summary>
    /// <param name="run">The Run object to be added to the database.</param>
    /// <returns>A Task containing the newly created Run record.</returns>
    public async Task<Run> CreateRun(Run run)
    {
        await _db.AddAsync(run);
        await _db.SaveChangesAsync();
        return run;
    }

    /// <summary>
    /// Deletes a specific Run record from the database.
    /// </summary>
    /// <param name="id">The unique identifier of the Run record to delete.</param>
    /// <returns>A Task containing an IResult indicating the result of the delete operation.</returns>
    /// <remarks>
    /// Returns Results.Ok() if the deletion is successful.
    /// Returns Results.NotFound() if the Run record does not exist.
    /// </remarks>
    public async Task<IResult> DeleteRun(int id)
    {
        var run = await _db.RunData.FindAsync(id);
        if (run is null) return Results.NotFound();

        _db.Remove(run);
        await _db.SaveChangesAsync();
        return Results.Ok();
    }
}

