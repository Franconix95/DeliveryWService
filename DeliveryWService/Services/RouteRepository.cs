using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DeliveryWService.Models;
using MySql.Data.MySqlClient;

namespace DeliveryWService.Services
{
    public class RouteRepository
    {
         private const string CacheKey = "RouteStore";

         public RouteRepository()
        {
            var ctx = HttpContext.Current;

            if (ctx != null)
            {
                if (ctx.Cache[CacheKey] == null)
                {
                    var routes = new Route[]
                    {
                        new Route
                        {
                            IdSource = 1, IdDestiny = 3, RouteCost = 20, RouteTime = 1
                        },
                        new Route
                        {
                            IdSource = 1, IdDestiny = 5, RouteCost = 5, RouteTime = 30
                        },
                        new Route
                        {
                            IdSource = 1, IdDestiny = 8, RouteCost = 1, RouteTime = 10
                        },
                        new Route
                        {
                            IdSource = 3, IdDestiny = 2, RouteCost = 12, RouteTime = 1
                        },
                        new Route
                        {
                            IdSource = 4, IdDestiny = 6, RouteCost = 50, RouteTime = 4
                        },
                        new Route
                        {
                            IdSource = 5, IdDestiny = 4, RouteCost = 5, RouteTime = 3
                        },
                        new Route
                        {
                            IdSource = 6, IdDestiny = 9, RouteCost = 50, RouteTime = 45
                        },
                        new Route
                        {
                            IdSource = 6, IdDestiny = 7, RouteCost = 50, RouteTime = 40
                        },
                        new Route
                        {
                            IdSource = 7, IdDestiny = 2, RouteCost = 73, RouteTime = 64
                        },
                        new Route
                        {
                            IdSource = 8, IdDestiny = 5, RouteCost = 1, RouteTime = 30
                        },
                        new Route
                        {
                            IdSource = 9, IdDestiny = 2, RouteCost = 5, RouteTime = 65
                        }
                    };

                    ctx.Cache[CacheKey] = routes;
                }
            }
        }

        public Route[] GetAllRoutes()
        {
            List<Route> allRoutes = new List<Route>();
            #region Cache Storage
            if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "CACHE")
            {
                var ctx = HttpContext.Current;

                if (ctx != null)
                {
                    return (Route[])ctx.Cache[CacheKey];
                }

                return new Route[]
                {
                    new Route
                    {
                        IdSource = 0, IdDestiny = 0, RouteCost = 0, RouteTime = 0
                    }
                };
            }
            #endregion
            #region DB Storage
            else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MYSQL")
            {
                var dbCon = DBConnection.Instance();
                if (dbCon.IsConnect())
                {
                    //suppose col0 and col1 are defined as VARCHAR in the DB
                    string query = "SELECT IdSource,IdDestiny,RouteCost,RouteTime Name FROM ROUTES";
                    var cmd = new MySqlCommand(query, dbCon.Connection);
                    var reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        Route route = new Route();
                        route.IdSource = reader.GetInt32(0);
                        route.IdDestiny = reader.GetInt32(1);
                        route.RouteCost = reader.GetInt32(2);
                        route.RouteTime = reader.GetInt32(3);
                        allRoutes.Add(route);
                    }
                    dbCon.Close();
                }
            }
            #endregion
            return allRoutes.ToArray();
        }

        public bool SaveRoute(Route route)
        {
            #region Cache Storage
            if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "CACHE")
            {
                var ctx = HttpContext.Current;
                if (ctx != null)
                {
                    try
                    {
                        var currentData = ((Route[])ctx.Cache[CacheKey]).ToList();
                        currentData.Add(route);
                        ctx.Cache[CacheKey] = currentData.ToArray();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return false;
                    }
                }
            }
            #endregion
            #region DB Storage
            else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MYSQL")
            {
                try
                {
                    var dbCon = DBConnection.Instance();
                    if (dbCon.IsConnect())
                    {
                        //suppose col0 and col1 are defined as VARCHAR in the DB
                        string query = "INSERT INTO `ROUTES`(`IdSource`, `IdDestiny`, `RouteCost`, `RouteTime`) VALUES  (" + route.IdSource + "," + route.IdDestiny  + "," + route.RouteCost + "," + route.RouteTime + ")";
                        var cmd = new MySqlCommand(query, dbCon.Connection);
                        var reader = cmd.ExecuteNonQuery();
                        dbCon.Close();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            #endregion
            return false;
        }

        public bool DeleteRoute(Route route)
        {
            #region Cache Storage
            if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "CACHE")
            {
                var ctx = HttpContext.Current;
                if (ctx != null)
                {
                    try
                    {
                        var currentData = ((Route[])ctx.Cache[CacheKey]).ToList();
                        if (currentData.Exists(p => p.IdSource == route.IdSource && p.IdDestiny == route.IdDestiny))
                        {
                            currentData.RemoveAt(currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestiny == route.IdDestiny));
                        }
                        ctx.Cache[CacheKey] = currentData.ToArray();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return false;
                    }
                }
            }
            #endregion
            #region DB Storage
            else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MYSQL")
            {
                try
                {
                    var dbCon = DBConnection.Instance();
                    if (dbCon.IsConnect())
                    {
                        //suppose col0 and col1 are defined as VARCHAR in the DB
                        string query = "DELETE FROM `ROUTES` WHERE IdSource = " + route.IdSource + " AND IdDestiny = " + route.IdDestiny;
                        var cmd = new MySqlCommand(query, dbCon.Connection);
                        var reader = cmd.ExecuteNonQuery();
                        dbCon.Close();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            #endregion

            return false;
        }

        public bool UpdateRoute(Route route)
        {
            #region Cache Storage
            if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "CACHE")
            {
                var ctx = HttpContext.Current;
                if (ctx != null)
                {
                    try
                    {
                        var currentData = ((Route[])ctx.Cache[CacheKey]).ToList();
                        if (currentData.Exists(p => p.IdSource == route.IdSource && p.IdDestiny == route.IdDestiny))
                        {
                            if (currentData[currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestiny == route.IdDestiny)].RouteCost != route.RouteCost && route.RouteCost > 0)
                                currentData[currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestiny == route.IdDestiny)].RouteCost = route.RouteCost;
                            if (currentData[currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestiny == route.IdDestiny)].RouteTime != route.RouteTime && route.RouteTime > 0)
                                currentData[currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestiny == route.IdDestiny)].RouteTime = route.RouteTime;
                        }
                        ctx.Cache[CacheKey] = currentData.ToArray();

                        return true;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.ToString());
                        return false;
                    }
                }
            }
            #endregion
            #region DB Storage
            else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MYSQL")
            {
                try
                {
                    var dbCon = DBConnection.Instance();
                    if (dbCon.IsConnect())
                    {
                        if (!string.IsNullOrEmpty(route.RouteCost.ToString()))
                        {
                            if(route.RouteCost > 0)
                            { 
                                string query = "UPDATE `ROUTES` SET `RouteCost`=" + route.RouteCost + "ss WHERE IdSource=" + route.IdSource + " AND IdDestiny = " + route.IdDestiny;
                                var cmd = new MySqlCommand(query, dbCon.Connection);
                                var reader = cmd.ExecuteNonQuery();
                            }
                        }
                        if (!string.IsNullOrEmpty(route.RouteTime.ToString()))
                        {
                            if (route.RouteTime > 0)
                            {
                                string query = "UPDATE `ROUTES` SET `RouteTime`=" + route.RouteTime + " WHERE IdSource=" + route.IdSource + " AND IdDestiny = " + route.IdDestiny;
                                var cmd = new MySqlCommand(query, dbCon.Connection);
                                var reader = cmd.ExecuteNonQuery();
                            }
                        }
                        dbCon.Close();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }
            #endregion

            return false;
        }
    }
}