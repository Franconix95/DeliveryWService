using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace DeliveryWService.Models
{
    [BsonIgnoreExtraElements]
    public class Route
    {
        /// <summary>
        /// Source id point - must exists in points repository.
        /// </summary>
        [BsonElement("IdSource")]
        public int IdSource { get; set; }
        /// <summary>
        /// Destination id point - must exists in points repository.
        /// </summary>
        [BsonElement("IdDestination")]
        public int IdDestination { get; set; }
        /// <summary>
        /// Cost value for route.
        /// </summary>
        [BsonElement("RouteCost")]
        public int RouteCost { get; set; }
        /// <summary>
        /// Time value for route.
        /// </summary>
        [BsonElement("RouteTime")]
        public int RouteTime { get; set; }
    }
}