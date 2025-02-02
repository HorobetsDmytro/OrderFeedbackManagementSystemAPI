using System.ComponentModel.DataAnnotations;

namespace OrderFeedbackManagementSystemAPI.Models.Requests
{
    public class UpdateReviewRequest
    {
        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [Required]
        [StringLength(1000)]
        public string Comment { get; set; }
    }
}
