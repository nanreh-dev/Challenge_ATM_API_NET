namespace ATM.Models.CustomModels
{
    public record TransactionResult
    {
        public bool Success { get; set; }
        public string? Name { get; set; }
        public string? CardNumber { get; set; }    
        public string? Date { get; set; }
        public string? Type { get; set; }
        public decimal Amount { get; set; }
        public string? Message { get; set; }
    }
}
