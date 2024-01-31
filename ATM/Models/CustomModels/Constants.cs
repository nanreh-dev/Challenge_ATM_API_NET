namespace ATM.Models.CustomModels
{
    public static class Constants
    {
        public struct TransactionType
        {
            public const byte CASH_OUT = 1;
            public const byte CASH_IN = 2;
            public const string AVAILABLE_CASH = "Saldo";
        }
        public struct Errors
        {
            public const string NO_CARD = "Tarjeta inexistente";
            public const string INVALID_PIN = "PIN Inválido. Después de 4 intentos la tarjeta quedará anulada";
            public const string CARD_BLOQUED = "La tarjeta fué anulada, lero lero 🤭";
            public const string TRANSACTION_ERROR = "Error en la transacción. Intente nuevamente.";
            public const string NO_MONEY = "Saldo Insuficiente.";
            public const string NO_MOVEMENTS = "Cuenta sin movimientos.";
            public const string WRONG_PAGE = "Página inexistente.";
        }

    }
}
