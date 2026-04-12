namespace OrderService.Models
{
    public class OrderCreatedEvent
    {
        public int OrderId { get; set; }
        public string Product { get; set; }
        public decimal Total { get; set; }
    }
}