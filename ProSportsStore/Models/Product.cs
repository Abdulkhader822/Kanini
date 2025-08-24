namespace ProSportsStore.Models
{
    public class Product
    {
        public int ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }

        // IMPORTANT: was `object` before -> caused 500. Make it string?.
        public string? Description { get; set; }
    }
}
