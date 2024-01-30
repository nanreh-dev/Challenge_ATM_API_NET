namespace ATM.Models.CustomModels
{
    public record LoginResult
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public string? Error { get; set; }
    }
}
