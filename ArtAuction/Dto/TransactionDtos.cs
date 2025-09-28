namespace ArtAuction.Dto
{
    public class TransactionDtos
    {
        public int TransactionId { get; set; }
        public int ArtworkId { get; set; }
        public int BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;   

        public decimal FinalPrice { get; set; }
        public DateTime TransactionDate { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = "Pending"; // Success / Failed
        public string ReceiptFilePath { get; set; } = string.Empty;
    }

    public class TransactionCreateDto
    {
        public int ArtworkId { get; set; }
        public int BuyerId { get; set; }
        public string PaymentMethod { get; set; } = "UPI";
    }
}
