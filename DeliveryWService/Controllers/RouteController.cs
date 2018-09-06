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
    public class RouteController : ApiController
    {
        private RouteRepository routeRepository;

        public RouteController()
        {
            this.routeRepository = new RouteRepository();
        }
        /// <summary>
        /// Get all routes.
        /// </summary>
        [ActionName("GetRoutes")]
        public Route[] Get()
        {
            return routeRepository.GetAllRoutes();
        }
        /// <summary>
        /// Save route.
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("SaveRoute")]
        public HttpResponseMessage SaveRoute(Route route)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, ModelState);
            else if (this.routeRepository.SaveRoute(route))
                return Request.CreateResponse<Route>(System.Net.HttpStatusCode.Created, route);
            else
                return Request.CreateResponse<Route>(System.Net.HttpStatusCode.BadRequest, route);
        }
        /// <summary>
        /// Delete route.
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("DeleteRoute")]
        public HttpResponseMessage DeleteRoute(Route route)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, ModelState); 
            else if (this.routeRepository.DeleteRoute(route))
                return Request.CreateResponse<Route>(System.Net.HttpStatusCode.Created, route);
            else
                return Request.CreateResponse<Route>(System.Net.HttpStatusCode.BadRequest, route);
        }
        /// <summary>
        /// Update route - Updates only cost or time
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("UpdateRoute")]
        public HttpResponseMessage UpdateRoute(Route route)
        {
            if (!ModelState.IsValid)
                return Request.CreateErrorResponse(System.Net.HttpStatusCode.BadRequest, ModelState); 
            else if (this.routeRepository.UpdateRoute(route))
                return Request.CreateResponse<Route>(System.Net.HttpStatusCode.Created, route);
            else
                return Request.CreateResponse<Route>(System.Net.HttpStatusCode.BadRequest, route);
        }
    }
}
