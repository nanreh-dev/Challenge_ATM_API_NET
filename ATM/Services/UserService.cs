using ATM.Interfaces;
using ATM.Models;
using Microsoft.EntityFrameworkCore;

namespace ATM.Services
{
    public class UserService : IUserService
    {
        private readonly ATMContext _context;

        public UserService(ATMContext context)
        {
            _context = context;
        }

        public User Get(string cardNumber)
        {
            try
            {
                var user =  _context.Users.Where(x => x.CardNumber == cardNumber).FirstOrDefault();
                return user ?? new User();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: " + ex.Message);
            }
        }

        public void UpdateWrongPin(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if(user != null)
            {
                user.WrongPin = (byte?)(user.WrongPin + 1);

                _context.Entry(user).State = EntityState.Modified;
                var state = _context.SaveChanges();

                if (state == 0)
                {
                    throw new Exception("Error al actualizar PIN.");
                }
            }
        }

        public void BlockCard(int id)
        {
            var user = _context.Users.FirstOrDefault(x => x.Id == id);
            if(user != null)
            {
                user.Enabled = false;

                _context.Entry(user).State = EntityState.Modified;
                var state = _context.SaveChanges();
                if (state == 0)
                {
                    throw new Exception("Error al bloquear la tarjeta.");
                }
            }
        }
    }
}
