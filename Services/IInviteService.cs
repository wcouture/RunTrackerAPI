using RunTrackerAPI.Models;

namespace RunTrackerAPI.Services;

public interface IInviteService {
    Task<List<FriendInvite>> AllInvites();
    Task<IResult> CreateInvite(FriendInvite invite);
    Task<List<FriendInvite>> GetInvitesByUserId(int userId);
    Task<IResult> AcceptInvite(int receiverId, int id);
    Task<IResult> RejectInvite(int receiverId, int id);
    Task<IResult> DeleteInvite(int senderId, int id);
}