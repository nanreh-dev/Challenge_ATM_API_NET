using ATM.Interfaces;
using ATM.Models.CustomModels;

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
            if(model.Card == null)
                return new LoginResult() { Success = false, Error = Constants.Errors.NO_CARD };

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
