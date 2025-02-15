using System.ComponentModel.DataAnnotations;

namespace OrderFeedbackManagementSystemAPI.Models.Requests;

public class UpdateCartItemQuantityRequest
{
    private int _quantity;

    [Range(1, int.MaxValue, ErrorMessage = "Кількість товару повинна бути більше 0")]
    public int Quantity
    {
        get => _quantity;
        set => _quantity = value;
    }
}