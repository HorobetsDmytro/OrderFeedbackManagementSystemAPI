﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Enums;
using OrderFeedbackManagementSystemAPI.Domain.Interfaces;
using OrderFeedbackManagementSystemAPI.Infrastructure.Data;

namespace OrderFeedbackManagementSystemAPI.Application.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IReviewService _reviewService;
        private readonly ApplicationDbContext _dbContext;

        public OrderService(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IReviewService reviewService, 
            ApplicationDbContext dbContext)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _reviewService = reviewService;
            _dbContext = dbContext;
        }

        public async Task<Order> CreateOrderAsync(int userId, List<OrderItem> items)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");


            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                    throw new ArgumentException($"Product with ID {item.ProductId} not found");

                if (product.Stock < item.Quantity)
                    throw new InvalidOperationException($"Insufficient stock for product {product.Name}");

                item.UnitPrice = product.Price;
            }

            var order = new Order
            {
                UserId = userId,
                OrderNumber = GenerateOrderNumber(),
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                OrderItems = items,
                TotalAmount = await CalculateOrderTotalAsync(items)
            };

            foreach (var item in items)
            {
                await _productRepository.UpdateStockAsync(item.ProductId, item.Quantity);
            }

            return await _orderRepository.AddAsync(order);
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            return await _orderRepository.GetOrderWithDetailsAsync(orderId);
        }

        public async Task<IEnumerable<Order>> GetUserOrdersAsync(int userId)
        {
            return await _orderRepository.GetUserOrdersAsync(userId);
        }

        public async Task<Order> UpdateOrderStatusAsync(int id, OrderStatus newStatus)
        {
            var order = await _dbContext.Orders.FindAsync(id);
            if (order == null)
            {
                throw new ArgumentException("Order not found");
            }

            order.Status = newStatus;
            await _dbContext.SaveChangesAsync();
            return order;
        }

        public async Task<decimal> CalculateOrderTotalAsync(List<OrderItem> items)
        {
            decimal total = 0;
            foreach (var item in items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                total += product.Price * item.Quantity;
            }
            return total;
        }

        private string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}";
        }
        
        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _dbContext.Orders
                .Include(o => o.User)
                .ToListAsync();
        }
    }
}
