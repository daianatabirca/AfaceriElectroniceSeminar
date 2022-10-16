using Microsoft.AspNetCore.Http;
using ProiectMaster.Models.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProiectMaster.Models.DTOs.VM
{
    public class OrderVM
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(256)]
        public string CustomerName { get; set; }
        [Required]
        [MaxLength(10)]
        public string CustomerPhoneNumber { get; set; }
        [Required]
        [MaxLength(256)]
        public string CustomerEmail { get; set; }
        [Required]
        [MaxLength(2000)]
        public string CustomerAddress { get; set; }
        public DateTime OrderDate { get; set; }

    }
}
