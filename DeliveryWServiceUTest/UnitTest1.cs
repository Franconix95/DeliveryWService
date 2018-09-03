using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeliveryWService.Controllers;
using DeliveryWService.Services;
using DeliveryWService.Models;

namespace DeliveryWServiceUTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetRoutes()
        {
            RouteRepository routeRepository = new RouteRepository();
            Route[] routes = routeRepository.GetAllRoutes();
            Assert.IsTrue(routes != null);  
        }

        [TestMethod]
        public void GetPoints()
        {
            PointRepository pointRepository = new PointRepository();
            Point[] points = pointRepository.GetAllPoints();
            Assert.IsTrue(points != null);
        }

        [TestMethod]
        public void GetDelivery()
        {
            DeliveryRepository deliveryRepository = new DeliveryRepository();
            Delivery delivery = new Delivery();
            delivery.IdSource = 0;
            delivery.IdDestiny = 0;
            delivery.Type = "TIME";
            Delivery deliveryRes = deliveryRepository.GetDelivery(delivery);
            Assert.IsTrue(deliveryRes != null);
        }
    }
}
