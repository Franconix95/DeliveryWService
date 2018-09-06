using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DeliveryWService.Controllers;
using DeliveryWService.Services;
using DeliveryWService.Models;

namespace DeliveryWServiceUTest
{
    [TestClass]
    public class UTestDelivery
    {
        [TestMethod]
        public void GetPoints()
        {
            PointRepository pointRepository = new PointRepository();
            Point[] points = pointRepository.GetAllPoints();
            Assert.IsTrue(points != null);
        }

        [TestMethod]
        public void InsertPoint()
        {
            PointRepository pointRepository = new PointRepository();
            Point point = new Point();
            point.Id = -1;
            point.Name = "Point Test";
            Assert.IsTrue(pointRepository.SavePoint(point));
        }

        [TestMethod]
        public void UpdatePoint()
        {
            PointRepository pointRepository = new PointRepository();
            List<Point> points = pointRepository.GetAllPoints().ToList();
            Point point = new Point();
            if (points.Exists(p => p.Name == "Point Test"))
            {
                point.Id = points.FirstOrDefault(p => p.Name == "Point Test").Id;
                point.Name = "Point Test Updated";
                Assert.IsTrue(pointRepository.SavePoint(point));
            }
        }

        [TestMethod]
        public void DeletePoint()
        {
            PointRepository pointRepository = new PointRepository();
            List<Point> points = pointRepository.GetAllPoints().ToList();
            Point point = new Point();
            if (points.Exists(p => p.Name == "Point Test Updated"))
            {
                point.Id = points.FirstOrDefault(p => p.Name == "Point Test Updated").Id;
                point.Name = points.FirstOrDefault(p => p.Name == "Point Test Updated").Name;
                Assert.IsTrue(pointRepository.DeletePoint(point));
            }
        }

        [TestMethod]
        public void GetRoutes()
        {
            RouteRepository routeRepository = new RouteRepository();
            Route[] routes = routeRepository.GetAllRoutes();
            Assert.IsTrue(routes != null);  
        }

        [TestMethod]
        public void GetDelivery()
        {
            DeliveryRepository deliveryRepository = new DeliveryRepository();
            Delivery delivery = new Delivery();
            delivery.IdSource = 0;
            delivery.IdDestination = 0;
            delivery.Type = "TIME";
            Delivery deliveryRes = deliveryRepository.GetDelivery(delivery);
            Assert.IsTrue(deliveryRes != null);
        }
    }
}
