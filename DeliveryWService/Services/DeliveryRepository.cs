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
using DeliveryWService.Services;

namespace DeliveryWService.Services
{
    public class DeliveryRepository
    {
        private const string CacheKeyRoute = "RouteStore";
        private const string CacheKeyPoint = "PointStore";

        public Delivery GetDelivery(Delivery delivery)
        {
           
            try
            {
                List<Route> currentDataRoute = new RouteRepository().GetAllRoutes().ToList();
                List<Point> currentDataPoint = new PointRepository().GetAllPoints().ToList();
                
                #region Validations
                if (!currentDataPoint.Exists(p => p.Id == delivery.IdSource))
                {
                    delivery.BestRoute = "No source point found in points repository.";
                    return delivery;
                }
                else if (!currentDataPoint.Exists(p => p.Id == delivery.IdDestination))
                {
                    delivery.BestRoute = "No destimatiom point found in points repository.";
                    return delivery;
                }
                else if (!currentDataPoint.Exists(p => p.Id == delivery.IdDestination))
                {
                    delivery.BestRoute = "No destimatiom point found in points repository.";
                    return delivery;
                }

                if (string.IsNullOrEmpty(delivery.Type))
                    delivery.Type = ConfigurationManager.AppSettings["dbDefaultSearchType"].ToUpper();
                else if (!ConfigurationManager.AppSettings["dbSearchTypes"].Split(';').ToList().Exists(a => a.ToUpper() == delivery.Type.ToUpper()))
                    delivery.Type = ConfigurationManager.AppSettings["dbDefaultSearchType"].ToUpper();
                #endregion Validations

                #region Build Graph
                currentDataRoute.RemoveAll(r => r.IdSource == delivery.IdSource && r.IdDestination == delivery.IdDestination);

                int[,] graph = new int[currentDataPoint.Count(), currentDataPoint.Count()];
                int x =0;
                foreach (Point pointX in currentDataPoint)
                {
                    int y = 0; 
                    foreach (Point pointY in currentDataPoint)
                    {
                        if (currentDataRoute.Exists(r => r.IdSource == pointX.Id && r.IdDestination == pointY.Id))
                        {
                            switch(delivery.Type.ToUpper())
                            {
                                case "COST":
                                    graph[x, y] = currentDataRoute.FirstOrDefault(r => r.IdSource == pointX.Id && r.IdDestination == pointY.Id).RouteCost;
                                    break;
                                case "TIME":
                                    graph[x, y] = currentDataRoute.FirstOrDefault(r => r.IdSource == pointX.Id && r.IdDestination == pointY.Id).RouteTime;
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
                    currentDataPoint.FindIndex(r => r.Id == delivery.IdDestination)
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
                new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() +" - " +ex.Message);
                return null;
            }
            return delivery;
        }

    }
}