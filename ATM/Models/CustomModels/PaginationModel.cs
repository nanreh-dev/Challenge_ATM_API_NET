namespace ATM.Models.CustomModels
{
    public record PaginationModel
    {
        public bool Success { get; set; }
        public string? Name { get; set; }
        public string? CardNumber { get; set; }
        public int TotalPages { get; set; }
        public int RequestedPage { get; set; }
        public string? Message { get; set; }
        public List<TransactionsResults>? Movements { get; set; }
    }
}
