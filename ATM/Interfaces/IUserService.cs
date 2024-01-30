using ATM.Models;

namespace ATM.Interfaces
{
    public interface IUserService
    {
        User Get(string cardNumber);

        void UpdateWrongPin(int id);

        void BlockCard(int id);
    }
}
