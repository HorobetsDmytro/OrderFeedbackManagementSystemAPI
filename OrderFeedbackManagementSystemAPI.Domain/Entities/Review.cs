﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace OrderFeedbackManagementSystemAPI.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }

        public int OrderId { get; set; }
        [JsonIgnore]
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Rating { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
