using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;
using System.Web.ModelBinding;

namespace AmaZone.Models
{
    public class Order
    {
        [Key]
        public string idString { get; set; }
        public string client { get; set; }
        [Display(Name = "City:")]
        [Required(ErrorMessage = "Required field")]
        [DataType(DataType.Text)]

        public string destination { get; set; }

        [Display(Name = "Address:")]
        public string address { get; set; }

        public DateTime shippingDate = DateTime.Now;

        [Display(Name = "Recipient Name:")]
        public string recipientName { get; set; }
        [Display(Name = "Freight:")]
        [Required(ErrorMessage = "Required Field")]
        public double freight { get; set; }
        [Display(Name = "ETA:")]
        [DataType(DataType.Date)]
        public DateTime arrivalDate { get; set; }
        public string state { get; set; }
    }
}