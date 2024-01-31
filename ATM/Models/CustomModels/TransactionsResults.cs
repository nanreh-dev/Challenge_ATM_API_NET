namespace ATM.Models.CustomModels
{
    public record TransactionsResults
    {
        public string? Date { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
    }
}
