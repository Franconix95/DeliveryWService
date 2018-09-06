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
    public class DeliveryController : ApiController
    {
        private DeliveryRepository deliveryRepository;

        public DeliveryController()
        {
            this.deliveryRepository = new DeliveryRepository();
        }
        /// <summary>
        /// Get bets delivery route - Default type is by cost
        /// </summary>
        [System.Web.Http.HttpPost]
        [ActionName("GetDelivery")]
        public HttpResponseMessage GetDelivery(Delivery delivery)
        {
            Delivery deliveryRes = this.deliveryRepository.GetDelivery(delivery);

            var response = Request.CreateResponse<Delivery>(System.Net.HttpStatusCode.Created, deliveryRes);
            return response;
        }

    }
}
