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

        public async Task<Review> CreateReviewAsync(int userId, int orderId, int rating, string comment)
        {
            var order = await _orderRepository.GetOrderWithDetailsAsync(orderId);
            if (order == null)
                throw new ArgumentException("Order not found");

            if (order.UserId != userId)
                throw new UnauthorizedAccessException("User is not authorized to review this order");

            if (order.Status != OrderStatus.Delivered)
                throw new InvalidOperationException("Can only review delivered orders");

            if (order.Review != null)
                throw new InvalidOperationException("Order already has a review");

            if (rating < 1 || rating > 5)
                throw new ArgumentException("Rating must be between 1 and 5");

            var review = new Review
            {
                UserId = userId,
                OrderId = orderId,
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
    }
}
