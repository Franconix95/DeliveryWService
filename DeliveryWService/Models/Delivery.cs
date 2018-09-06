using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryWService.Models
{
    public class Delivery
    {
        /// <summary>
        /// Id source point.
        /// </summary>
        public int IdSource { get; set; }
        /// <summary>
        /// Id destination point.
        /// </summary>
        public int IdDestination { get; set; }
        /// <summary>
        /// Default type is by cost
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Get's the best route and total cost or time
        /// </summary>
        public string BestRoute { get; set; }
    }
}