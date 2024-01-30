﻿using ATM.Interfaces;
using ATM.Models;
using ATM.Models.CustomModels;

namespace ATM.Services
{
    public class TransactionServices : ITransactionService
    {
        private readonly ATMContext _context;
        private readonly IUserService _userService;

        public TransactionServices(ATMContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public Transaction? Get(int userId)
        {
            return _context.Transactions.Where(y => y.UserId == userId).OrderByDescending(x => x.IdTransaction).FirstOrDefault();
        }

        public TransactionResult GetSaldo(string cardNumber)
        {
            var user = _userService.Get(cardNumber);

            //obtengo la ultima transaccion
            var lastTransaction = Get(user.Id);

            if (lastTransaction == null)
            {
                return new TransactionResult() { Success = false, Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"), Type = Constants.TransactionType.AVAILABLE_CASH, Message = Constants.Errors.NO_MOVEMENTS };
            }

            return new TransactionResult() { Success = true, Name = user.Name, CardNumber = cardNumber, Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"), Type = Constants.TransactionType.AVAILABLE_CASH, Amount = lastTransaction.Amount };
        }

        public TransactionResult Transactions(string cardNumber, byte type, decimal amount)
        {
            var user = _userService.Get(cardNumber);
            decimal newAmount;
            int state;

            //obtengo la ultima transaccion
            var lastTransaction = Get(user.Id);
            
            Transaction newTransaction = new();

            //determino si es deposito o extraccion
            if (type == Constants.TransactionType.CASH_IN)
            {
                if (lastTransaction == null)
                {
                    newTransaction.UserId = user.Id;
                    newTransaction.Date = DateTime.Now;
                    newTransaction.Type = type;
                    newTransaction.Amount = amount;

                    _context.Transactions.Add(newTransaction);
                    state = _context.SaveChanges();
                    if (state == 0)
                    {
                        throw new Exception("Error en la transación.");
                    }

                    return new TransactionResult() { Success = true, Name = user.Name, CardNumber = cardNumber, Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"), Type = GetTransactionDescription(type), Amount = amount };
                }

                //nuevo saldo
                newAmount = lastTransaction.Amount + amount;

                newTransaction.UserId = user.Id;
                newTransaction.Date = DateTime.Now;
                newTransaction.Type = type;
                newTransaction.Amount = newAmount;

                _context.Transactions.Add(newTransaction);
                state = _context.SaveChanges();
                if (state == 0)
                {
                    throw new Exception("Error en la transación.");
                }

                return new TransactionResult() { Success = true, Name = user.Name, CardNumber = cardNumber, Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"), Type = GetTransactionDescription(type), Amount = newAmount };
            }
            if(lastTransaction == null || lastTransaction.Amount < amount)
            {
                return new TransactionResult() { Success = false,Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"), Message = Constants.Errors.NO_MONEY };
            }

            newAmount = lastTransaction.Amount - amount;

            newTransaction.UserId = user.Id;
            newTransaction.Date = DateTime.Now;
            newTransaction.Type = type;
            newTransaction.Amount = newAmount;

            _context.Transactions.Add(newTransaction);
            state = _context.SaveChanges();
            if (state == 0)
            {
                throw new Exception("Error en la transación.");
            }

            return new TransactionResult() { Success = true, Name = user.Name, CardNumber = cardNumber, Date = DateTime.Now.ToString("dd/MM/yyyy hh:mm tt"), Type = GetTransactionDescription(type), Amount = newAmount };

        }

        string GetTransactionDescription(byte type)
        {
            return _context.TransactionTypes.Where(x => x.IdType == type).FirstOrDefault().Description.Trim();
        }
    }
}