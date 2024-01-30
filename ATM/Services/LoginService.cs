using ATM.Interfaces;
using ATM.Models;
using ATM.Models.CustomModels;
using Microsoft.EntityFrameworkCore;

namespace ATM.Services
{
    public class LoginService : ILoginService
    {
        private readonly IUserService _userService;

        public LoginService(IUserService userService)
        {
            _userService = userService;
        }

        public LoginResult CheckCard(LoginModel model)
        {
            //TO DO model.card == null -> badRequest
            var user =  _userService.Get(model.Card);

            if (user == null)
            {
                return new LoginResult() { Success = false, Error = Constants.Errors.NO_CARD };
            }
            else if (!user.Enabled)
            {
                return new LoginResult() { Success = false, Error = Constants.Errors.CARD_BLOQUED };
            }
            else if (model.Pin != user.Pin)
            {
                if(user.WrongPin >= 4)
                {
                    _userService.BlockCard(user.Id);

                    return new LoginResult() { Success = false, Error = Constants.Errors.CARD_BLOQUED };
                }
                else
                {
                    _userService.UpdateWrongPin(user.Id);

                    return new LoginResult() { Success = false, Error = Constants.Errors.INVALID_PIN };
                }
            }
            
            return new LoginResult() { Success = true };
        }
    }
}
