using System.ComponentModel.DataAnnotations;

namespace RunTrackerAPI.Models;

public enum FriendInviteStatus {
    Pending,
    Accepted,
    Rejected
}

public class FriendInvite {
    [Key]
    public int Id {get; set;}
    public int SenderId {get; set;}
    public int ReceiverId {get; set;}
    public FriendInviteStatus Status {get; set;} = FriendInviteStatus.Pending;
    public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
}