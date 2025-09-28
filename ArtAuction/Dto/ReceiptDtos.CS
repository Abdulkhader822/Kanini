namespace ArtAuction.Dto
{
    public class ReceiptDtos
    {
        public int ReceiptId { get; set; }
        public int TransactionId { get; set; }
        public string PdfFilePath { get; set; } = string.Empty;
        public DateTime GeneratedOn { get; set; }
    }
}
