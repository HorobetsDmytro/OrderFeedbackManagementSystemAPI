namespace OrderFeedbackManagementSystemAPI.Models.Requests;

public class AddToCartRequest
{
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}