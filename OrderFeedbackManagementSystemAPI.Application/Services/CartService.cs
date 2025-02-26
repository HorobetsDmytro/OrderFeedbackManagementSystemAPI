using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Interfaces;

namespace OrderFeedbackManagementSystemAPI.Application.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;
    private ICartService _cartServiceImplementation;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<Cart> GetCartAsync(int userId)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            await _cartRepository.AddAsync(cart);
        }
        return cart;
    }

    public async Task AddToCartAsync(int userId, int productId, int quantity)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException("Товар не знайдено");

        if (product.Stock < quantity)
            throw new InvalidOperationException("Недостатньо товару на складі");

        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
        {
            cart = new Cart { UserId = userId };
            await _cartRepository.AddAsync(cart);
        }

        var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (existingItem != null)
        {
            existingItem.Quantity += quantity;
        }
        else
        {
            cart.Items.Add(new CartItem
            {
                ProductId = productId,
                Quantity = quantity,
                Price = product.Price
            });
        }

        await _cartRepository.UpdateAsync(cart);
    }


    public async Task RemoveFromCartAsync(int userId, int productId)
    {
        var cart = await GetCartAsync(userId);
        var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (item != null)
        {
            cart.Items.Remove(item);
            await _cartRepository.UpdateAsync(cart);
        }
    }

    public async Task<Cart> UpdateCartItemQuantityAsync(int userId, int productId, int quantity)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId);
        if (cart == null)
            throw new InvalidOperationException("Корзина не знайдена");

        var cartItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
        if (cartItem == null)
            throw new InvalidOperationException("Товар не знайдений в корзині");

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            throw new InvalidOperationException("Товар не знайдений");

        if (product.Stock < quantity)
            throw new InvalidOperationException("Недостатньо товару на складі");

        cartItem.Quantity = quantity;
        await _cartRepository.UpdateAsync(cart);

        return cart;
    }

    public async Task ClearCartAsync(int userId)
    {
        await _cartRepository.ClearCartAsync(userId);
    }
}