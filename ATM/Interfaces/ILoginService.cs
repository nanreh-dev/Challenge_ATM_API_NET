using ATM.Models;
using ATM.Models.CustomModels;

namespace ATM.Interfaces
{
    public interface ILoginService
    {
        LoginResult CheckCard(LoginModel model);
    }
}
