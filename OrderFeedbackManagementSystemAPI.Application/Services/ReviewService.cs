using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrderFeedbackManagementSystemAPI.Application.Interfaces;
using OrderFeedbackManagementSystemAPI.Domain.Entities;
using OrderFeedbackManagementSystemAPI.Domain.Enums;
using OrderFeedbackManagementSystemAPI.Domain.Interfaces;

namespace OrderFeedbackManagementSystemAPI.Application.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserRepository _userRepository;

        public ReviewService(
            IReviewRepository reviewRepository,
            IOrderRepository orderRepository,
            IUserRepository userRepository)
        {
            _reviewRepository = reviewRepository;
            _orderRepository = orderRepository;
            _userRepository = userRepository;
        }

        public async Task<Review> CreateReviewAsync(int userId, int orderId, int productId, int rating, string comment)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.UserId != userId)
                throw new UnauthorizedAccessException("User is not authorized to review this order");

            var orderItem = order.OrderItems.FirstOrDefault(item => item.ProductId == productId);
            if (orderItem == null)
                throw new ArgumentException("Product is not part of the specified order");

            var existingReview = order.Reviews?.FirstOrDefault(r => r.ProductId == productId);
            if (existingReview != null)
                throw new InvalidOperationException("A review for this product in the order already exists");

            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var review = new Review
            {
                UserId = userId,
                OrderId = orderId,
                ProductId = productId,
                Rating = rating,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            return await _reviewRepository.AddAsync(review);
        }

        public async Task<Review> UpdateReviewAsync(int reviewId, int rating, string comment)
        {
            var review = await _reviewRepository.GetByIdAsync(reviewId);
            if (review == null)
                throw new ArgumentException("Review not found");

            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            review.Rating = rating;
            review.Comment = comment;

            await _reviewRepository.UpdateAsync(review);
            return review;
        }

        public async Task<IEnumerable<Review>> GetOrderReviewsAsync(int orderId)
        {
            return await _reviewRepository.GetOrderReviewsAsync(orderId);
        }

        public async Task<IEnumerable<Review>> GetFilteredReviewsAsync(int rating)
        {
            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            return await _reviewRepository.GetFilteredReviewsAsync(rating);
        }

        public async Task<double> GetProductAverageRatingAsync(int productId)
        {
            return await _reviewRepository.GetProductAverageRatingAsync(productId);
        }

        public async Task<Review> HandleOrderStatusChangeAsync(int orderId, OrderStatus newStatus)
        {
            var review = await _reviewRepository.GetReviewByOrderIdAsync(orderId);
            if (review != null && newStatus != OrderStatus.Delivered)
            {
                await _reviewRepository.DeleteAsync(review);
                return null;
            }
            return review;
        }
        
        public async Task<bool> HasUserPurchasedProductAsync(int userId, int productId)
        {
            var orders = await _orderRepository.GetUserOrdersAsync(userId);
            return orders.Any(order =>
                order.Status == OrderStatus.Delivered &&
                order.OrderItems.Any(item => item.ProductId == productId)
            );
        }
        
        public async Task<IEnumerable<Review>> GetProductReviewsAsync(int productId)
        {
            // Validate productId (optional, depending on your requirements)
            if (productId <= 0)
            {
                throw new ArgumentException("Invalid productId");
            }

            // Fetch reviews for the specified product
            return await _reviewRepository.GetProductReviewsAsync(productId);
        }
    }
}
