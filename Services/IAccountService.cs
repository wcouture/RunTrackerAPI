using RunTrackerAPI.Models;

namespace RunTrackerAPI.Services;

public interface IAccountService
{
    Task<List<UserAccount>> GetAllAccounts();
    Task<IResult> GetAccountById(int id);
    Task<IResult> CreateAccount(UserAccount account);
    Task<IResult> UpdateAccount(UserAccount account, int id);
    Task<IResult> DeleteAccount(int id);
}