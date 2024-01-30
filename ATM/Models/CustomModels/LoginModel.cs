namespace ATM.Models.CustomModels
{
    public record LoginModel
    {
        public string? Card { get; set; } 
        public string? Pin { get; set; }
    }
}
