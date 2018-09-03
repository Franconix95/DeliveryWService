using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DeliveryWService.Models;
using QuickGraph;
using QuickGraph.Algorithms;
using QuickGraph.Algorithms.Observers;
using QuickGraph.Algorithms.ShortestPath;
using DeliveryWService.Models;

namespace DeliveryWService.Services
{
    public class DeliveryRepository
    {
        private const string CacheKeyRoute = "RouteStore";
        private const string CacheKeyPoint = "PointStore";

        public DeliveryRepository()
        {
            var ctx = HttpContext.Current;
        }

        public Delivery GetDelivery(Delivery delivery)
        {
            var ctx = HttpContext.Current;

            if (ctx != null)
            {
                try
                {
                    List<Route> currentDataRoute = new RouteRepository().GetAllRoutes().ToList();
                    List<Point> currentDataPoint = new PointRepository().GetAllPoints().ToList();
                    currentDataRoute.RemoveAll(r => r.IdSource == delivery.IdSource && r.IdDestiny == delivery.IdDestiny);

                    #region Build Graph
                    int[,] graph = new int[currentDataPoint.Count(), currentDataPoint.Count()];
                    int x =0;
                    foreach (Point pointX in currentDataPoint)
                    {
                        int y = 0; 
                        foreach (Point pointY in currentDataPoint)
                        {
                            if (currentDataRoute.Exists(r => r.IdSource == pointX.Id && r.IdDestiny == pointY.Id))
                            {
                                switch(delivery.Type.ToUpper())
                                {
                                    case "COST":
                                        graph[x, y] = currentDataRoute.FirstOrDefault(r => r.IdSource == pointX.Id && r.IdDestiny == pointY.Id).RouteCost;
                                        break;
                                    case "TIME":
                                        graph[x, y] = currentDataRoute.FirstOrDefault(r => r.IdSource == pointX.Id && r.IdDestiny == pointY.Id).RouteTime;
                                        break;
                                }
                            }
                            else
                                graph[x, y] = 0;
                            y++;
                        }
                        x++;
                    }
                    #endregion Build Graph

                    #region Get Short Path
                    var path = Dijkstra.DijkstraAlgorithm(graph, 
                        currentDataPoint.FindIndex(r => r.Id == delivery.IdSource),
                        currentDataPoint.FindIndex(r => r.Id == delivery.IdDestiny)
                        );

                    string formattedPath = "";
                    if (path == null)
                    {
                        formattedPath = "No path";
                    }
                    else
                    {
                        int pathLength = 0;
                        for (int i = 0; i < path.Count - 1; i++)
                        {
                            pathLength += graph[path[i], path[i + 1]];
                        }
                        
                        for (int i = 0; i < path.Count; i++)
                        {
                            formattedPath += "(" + currentDataPoint[path[i]].Id + " - " + currentDataPoint[path[i]].Name + ")";
                            if (i < path.Count - 1)
                                formattedPath += " -> ";
                        }
                        formattedPath += " - Total cost in " + delivery.Type + ": " + pathLength;
                    }
                    #endregion
                    delivery.BestRoute = formattedPath;

                    return delivery;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return delivery;
                }
            }
            return delivery;
        }

    }
}