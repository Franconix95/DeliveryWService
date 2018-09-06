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
        public HttpResponseMessage SavePoint(Point point)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, ModelState);
            else if (this.pointRepository.SavePoint(point))
                return Request.CreateResponse<Point>(System.Net.HttpStatusCode.Created, point);
            else
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "Error saving point!");
        }
        /// <summary>
        /// Delete a point - Delete all routes where source or destination point is the point selected for deletion
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("DeletePoint")]
        public HttpResponseMessage DeletePoint(Point point)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, ModelState);
            else if (this.pointRepository.DeletePoint(point))
                return Request.CreateResponse<Point>(System.Net.HttpStatusCode.OK, point);
            else
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "Error deleting point!");
        }
        /// <summary>
        /// Update a point
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("UpdatePoint")]
        public HttpResponseMessage UpdatePoint(Point point)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, ModelState);
            else if (this.pointRepository.UpdatePoint(point))
                return Request.CreateResponse<Point>(System.Net.HttpStatusCode.OK, point);
            else
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, "Error updating point!");
        }
    }
}
