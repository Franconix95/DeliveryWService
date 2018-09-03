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
            this.routeRepository.SaveRoute(route);

            var response = Request.CreateResponse<Route>(System.Net.HttpStatusCode.Created, route);

            return response;
        }
        /// <summary>
        /// Delete route.
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("DeleteRoute")]
        public Route[] DeleteRoute(Route route)
        {
            this.routeRepository.DeleteRoute(route);
            return routeRepository.GetAllRoutes();
        }
        /// <summary>
        /// Update route.
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("UpdateRoute")]
        public Route[] UpdateRoute(Route route)
        {
            this.routeRepository.UpdateRoute(route);
            return routeRepository.GetAllRoutes();
        }
    }
}
