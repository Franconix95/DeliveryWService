using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MongoDB.Bson.Serialization.Attributes;

namespace DeliveryWService.Models
{
    [BsonIgnoreExtraElements]
    public class Point
    {
        /// <summary>
        /// Id point - auto increment.
        /// </summary>
        [BsonElement("Id")]
        public int Id { get; set; }
        /// <summary>
        /// Name point - must be unique.
        /// </summary>
        [BsonElement("Name")]
        public string Name { get; set; }
    }
}