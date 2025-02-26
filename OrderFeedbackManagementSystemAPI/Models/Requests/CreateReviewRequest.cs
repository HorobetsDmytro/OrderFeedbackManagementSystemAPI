using System.ComponentModel.DataAnnotations;

namespace OrderFeedbackManagementSystemAPI.Models.Requests
{
    public class CreateReviewRequest
    {
        [Required]
        public int OrderId { get; set; }
        
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; }
    }
}
