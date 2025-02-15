namespace OrderFeedbackManagementSystemAPI.Domain.Entities;

public class Cart
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public ICollection<CartItem> Items { get; set; }
    public DateTime LastModified { get; set; }
    
    public Cart()
    {
        Items = new List<CartItem>();
    }
}