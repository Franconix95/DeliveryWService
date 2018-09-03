using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DeliveryWService.Models;
using DeliveryWService.Services;

namespace DeliveryWService.Controllers
{
    public class PointController : ApiController
    {
        private PointRepository pointRepository;

        public PointController()
        {
            this.pointRepository = new PointRepository();
        }
        /// <summary>
        /// Get all points.
        /// </summary>
        [ActionName("GetPoints")]
        public Point[] GetAllPoints()
        {
            return pointRepository.GetAllPoints();
        }
        /// <summary>
        /// Save a point.
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("SavePoint")]
        public Point[] SavePoint(Point point)
        {
            this.pointRepository.SavePoint(point);
            return pointRepository.GetAllPoints();
        }
        /// <summary>
        /// Delete a point.
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("DeletePoint")]
        public Point[] DeletePoint(Point point)
        {
            this.pointRepository.DeletePoint(point);
            return pointRepository.GetAllPoints();
        }
        /// <summary>
        /// Update a point.
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("UpdatePoint")]
        public Point[] UpdatePoint(Point point)
        {
            this.pointRepository.UpdatePoint(point);
            return pointRepository.GetAllPoints();
        }
    }
}
