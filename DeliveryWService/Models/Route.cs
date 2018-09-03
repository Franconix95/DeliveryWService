using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DeliveryWService.Models
{
    public class Route
    {
        public int IdSource { get; set; }
        public int IdDestiny { get; set; }
        public int RouteCost { get; set; }
        public int RouteTime { get; set; }
    }
}