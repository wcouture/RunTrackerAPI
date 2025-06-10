using Microsoft.EntityFrameworkCore;
using RunTrackerAPI.Models;

namespace RunTrackerAPI.Services;

/// <summary>
/// Service class for managing FriendInvite entities in the database.
/// Provides CRUD operations and additional functionality for FriendInvite records.
/// Implements the IInviteService interface for dependency injection and testing.
/// </summary>
public class InviteService : IInviteService {
    private RunDb _db {get; set;} = null!;

    /// <summary>
    /// Initializes a new instance of the InviteService class.
    /// </summary>
    /// <param name="db">The database context for FriendInvite entities.</param>
    public InviteService(RunDb db) {
        _db = db;
    }

    /// <summary>
    /// Creates a new FriendInvite record in the database.
    /// </summary>
    /// <param name="invite">The FriendInvite object to create.</param>
    /// <returns>The created FriendInvite object.</returns>
    public async Task<IResult> CreateInvite(FriendInvite invite) {
        _db.FriendInvites.Add(invite);
        await _db.SaveChangesAsync();
        return Results.Ok(invite);
    }

    /// <summary>
    /// Retrieves all FriendInvite records for a given user ID.
    /// </summary>
    /// <param name="userId">The ID of the user to retrieve FriendInvite records for.</param>
    /// <returns>A list of FriendInvite objects.</returns>
    public async Task<List<FriendInvite>> GetInvitesByUserId(int userId) {
        return await _db.FriendInvites.Where(i => i.ReceiverId == userId && i.Status == FriendInviteStatus.Pending).ToListAsync();
    }

    /// <summary>
    /// Accepts a FriendInvite record for a given user ID.
    /// </summary>
    /// <param name="receiverId">The ID of the user accepting the FriendInvite.</param>
    /// <param name="id">The ID of the FriendInvite to accept.</param>
    /// <returns>An IResult object indicating the success or failure of the operation.</returns>
    public async Task<IResult> AcceptInvite(int receiverId, int id) {
        var invite = await _db.FriendInvites.FirstOrDefaultAsync(i => i.Id == id && i.ReceiverId == receiverId);
        if (invite == null) {
            return Results.NotFound();
        }
        invite.Status = FriendInviteStatus.Accepted;
        var sender = await _db.AccountData.FirstOrDefaultAsync(a => a.Id == invite.SenderId);
        if (sender == null) {
            return Results.NotFound();
        }
        sender.Friends.Add(receiverId);
        var receiver = await _db.AccountData.FirstOrDefaultAsync(a => a.Id == invite.ReceiverId);
        if (receiver == null) {
            return Results.NotFound();
        }
        receiver.Friends.Add(sender.Id);
        await _db.SaveChangesAsync();
        return Results.Ok(invite);
    }

    /// <summary>
    /// Rejects a FriendInvite record for a given user ID.
    /// </summary>
    /// <param name="receiverId">The ID of the user rejecting the FriendInvite.</param>
    /// <param name="id">The ID of the FriendInvite to reject.</param>
    /// <returns>An IResult object indicating the success or failure of the operation.</returns>
    public async Task<IResult> RejectInvite(int receiverId, int id) {
        var invite = await _db.FriendInvites.FirstOrDefaultAsync(i => i.Id == id && i.ReceiverId == receiverId);
        if (invite == null) {
            return Results.NotFound();
        }
        invite.Status = FriendInviteStatus.Rejected;
        await _db.SaveChangesAsync();
        return Results.Ok(invite);
    }

    /// <summary>
    /// Deletes a FriendInvite record for a given user ID.
    /// </summary>
    /// <param name="senderId">The ID of the user deleting the FriendInvite.</param>
    /// <param name="id">The ID of the FriendInvite to delete.</param>
    /// <returns>An IResult object indicating the success or failure of the operation.</returns>
    public async Task<IResult> DeleteInvite(int senderId, int id) {
        var invite = await _db.FriendInvites.FirstOrDefaultAsync(i => i.Id == id && i.SenderId == senderId);
        if (invite == null) {
            return Results.NotFound();
        }
        _db.FriendInvites.Remove(invite);
        await _db.SaveChangesAsync();
        return Results.Ok(invite);
    }
}