namespace OrderFeedbackManagementSystemAPI.Domain.Entities;

public class CartItem
{
    public int Id { get; private set; }
    public int CartId { get; private set; }
    public int ProductId { get; set; }
    public Product Product { get; private set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public static CartItem Create(int productId, int quantity, decimal price)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        return new CartItem
        {
            ProductId = productId,
            Quantity = quantity,
            Price = price
        };
    }

    public void UpdateQuantity(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        Quantity = quantity;
    }
}