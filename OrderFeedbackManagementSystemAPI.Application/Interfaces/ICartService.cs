using OrderFeedbackManagementSystemAPI.Domain.Entities;

namespace OrderFeedbackManagementSystemAPI.Application.Interfaces;

public interface ICartService
{
    Task<Cart> GetCartAsync(int userId);
    Task AddToCartAsync(int userId, int productId, int quantity);
    Task RemoveFromCartAsync(int userId, int productId);
    Task<Cart> UpdateCartItemQuantityAsync(int userId, int productId, int quantity);
    Task ClearCartAsync(int userId);
}