using System.ComponentModel.DataAnnotations;

namespace OrderFeedbackManagementSystemAPI.Models.Requests
{
    public class StockUpdateRequest
    {
        [Required]
        public int Quantity { get; set; }
    }
}
