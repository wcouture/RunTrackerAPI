using RunTrackerAPI.Models;

namespace RunTrackerAPI.Services;

public interface IAccountService
{
    Task<List<UserAccount>> GetAllAccounts();
    Task<IResult> Authenticate(UserAccount user);
    Task<IResult> CreateAccount(UserAccount account);
    Task<IResult> UpdateAccount(UserAccount account, int id);
    Task<IResult> DeleteAccount(int id);
    Task<IResult> UserInfo(int id);
    Task<IResult> GetAccountFriends(int id);
}