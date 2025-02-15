using OrderFeedbackManagementSystemAPI.Domain.Entities;

namespace OrderFeedbackManagementSystemAPI.Domain.Interfaces;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task<Cart> GetUserCartAsync(int userId);
    Task<Cart> GetCartWithItemsAsync(int cartId);
    Task ClearCartAsync(int cartId);
    Task<Cart> GetByUserIdAsync(int userId);
}