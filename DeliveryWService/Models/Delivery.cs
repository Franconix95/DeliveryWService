using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryWService.Models
{
    public class Delivery
    {
        public int IdSource { get; set; }
        public int IdDestiny { get; set; }
        public string Type { get; set; }
        public string BestRoute { get; set; }
    }
}