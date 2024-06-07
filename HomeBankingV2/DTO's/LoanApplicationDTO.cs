namespace HomeBankingV2.DTO_s
{
    public class LoanApplicationDTO
    { 
        public long LoanId { get; set; }
        public double Amount {  get; set; } 
        public string Payments { get; set; }
        public string ToAccountNumber { get; set; }
    }
}
