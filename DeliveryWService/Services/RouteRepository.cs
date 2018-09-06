using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using DeliveryWService.Models;
using MongoDB.Bson;
using MongoDB.Driver;
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
                            IdSource = 1, IdDestination = 3, RouteCost = 20, RouteTime = 1
                        },
                        new Route
                        {
                            IdSource = 1, IdDestination = 5, RouteCost = 5, RouteTime = 30
                        },
                        new Route
                        {
                            IdSource = 1, IdDestination = 8, RouteCost = 1, RouteTime = 10
                        },
                        new Route
                        {
                            IdSource = 3, IdDestination = 2, RouteCost = 12, RouteTime = 1
                        },
                        new Route
                        {
                            IdSource = 4, IdDestination = 6, RouteCost = 50, RouteTime = 4
                        },
                        new Route
                        {
                            IdSource = 5, IdDestination = 4, RouteCost = 5, RouteTime = 3
                        },
                        new Route
                        {
                            IdSource = 6, IdDestination = 9, RouteCost = 50, RouteTime = 45
                        },
                        new Route
                        {
                            IdSource = 6, IdDestination = 7, RouteCost = 50, RouteTime = 40
                        },
                        new Route
                        {
                            IdSource = 7, IdDestination = 2, RouteCost = 73, RouteTime = 64
                        },
                        new Route
                        {
                            IdSource = 8, IdDestination = 5, RouteCost = 1, RouteTime = 30
                        },
                        new Route
                        {
                            IdSource = 9, IdDestination = 2, RouteCost = 5, RouteTime = 65
                        }
                    };

                    ctx.Cache[CacheKey] = routes;
                }
            }
        }

        public Route[] GetAllRoutes()
         {
            List<Route> allRoutes = new List<Route>();
            try
            {
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
                            IdSource = 0, IdDestination = 0, RouteCost = 0, RouteTime = 0
                        }
                    };
                }
                #endregion
                #region DB Storage MySQl
                else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MYSQL")
                {
                    var dbCon = DBConnection.Instance();
                    if (dbCon.IsConnect())
                    {
                        //suppose col0 and col1 are defined as VARCHAR in the DB
                        string query = "SELECT IdSource,IdDestination,RouteCost,RouteTime Name FROM ROUTES";
                        var cmd = new MySqlCommand(query, dbCon.Connection);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Route route = new Route();
                            route.IdSource = reader.GetInt32(0);
                            route.IdDestination = reader.GetInt32(1);
                            route.RouteCost = reader.GetInt32(2);
                            route.RouteTime = reader.GetInt32(3);
                            allRoutes.Add(route);
                        }
                        dbCon.Close();
                    }
                }
                #endregion DB Storage MySQl
                #region DB Storage Mongo
                else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MONGO")
                {
                    var dbCon = DBConnectionMongo.Instance();
                    MongoDB.Driver.MongoClient client = dbCon.GetClient();
                    client.StartSession();
                    var collection = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]).GetCollection<Route>("ROUTES");
                    allRoutes = collection.Find(_ => true).ToList();
                }
                #endregion DB Storage Mongo
            }
            catch (Exception ex)
            {
                new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() +" - " +ex.Message);
            }
            return allRoutes.ToArray();
        }

        public bool SaveRoute(Route route)
        {
            try
            {
                #region Validations
                List<Point> points = new PointRepository().GetAllPoints().ToList();
                if (!points.Exists(p=>p.Id == route.IdSource))
                    return false;
                else if (!points.Exists(p=>p.Id == route.IdDestination))
                    return false;
                List<Route> routes = new RouteRepository().GetAllRoutes().ToList();
                if (routes.Exists(r => r.IdSource == route.IdSource && r.IdDestination == route.IdDestination))
                    return false;
                #endregion Validations

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
                            new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() + " - " + ex.Message);
                            return false;
                        }
                    }
                }
                #endregion
                #region DB Storage MySQl
                else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MYSQL")
                {
                    try
                    {
                        var dbCon = DBConnection.Instance();
                        if (dbCon.IsConnect())
                        {
                            //suppose col0 and col1 are defined as VARCHAR in the DB
                            string query = "INSERT INTO `ROUTES`(`IdSource`, `IdDestination`, `RouteCost`, `RouteTime`) VALUES  (" + route.IdSource + "," + route.IdDestination + "," + route.RouteCost + "," + route.RouteTime + ")";
                            var cmd = new MySqlCommand(query, dbCon.Connection);
                            var reader = cmd.ExecuteNonQuery();
                            dbCon.Close();
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() + " - " + ex.Message);
                        return false;
                    }
                }
                #endregion DB Storage MySQl
                #region  DB Storage Mongo
                else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MONGO")
                {
                    var dbCon = DBConnectionMongo.Instance();
                    MongoDB.Driver.MongoClient client = dbCon.GetClient();
                    client.StartSession();
                    var collection = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]).GetCollection<Route>("ROUTES");
                    collection.InsertOne(route);
                    return true;
                }
                #endregion  DB Storage Mongo
            }
            catch (Exception ex)
            {
                new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() +" - " +ex.Message);
            }
            return false;
        }

        public bool DeleteRoute(Route route)
        {
            try
            {
                #region Validations
                List<Route> routes = new RouteRepository().GetAllRoutes().ToList();
                if (!routes.Exists(r => r.IdSource == route.IdSource && r.IdDestination == route.IdDestination))
                    return false;
                #endregion Validations

                #region Cache Storage
                if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "CACHE")
                {
                    var ctx = HttpContext.Current;
                    if (ctx != null)
                    {
                        try
                        {
                            var currentData = ((Route[])ctx.Cache[CacheKey]).ToList();
                            if (currentData.Exists(p => p.IdSource == route.IdSource && p.IdDestination == route.IdDestination))
                            {
                                currentData.RemoveAt(currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestination == route.IdDestination));
                            }
                            ctx.Cache[CacheKey] = currentData.ToArray();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() + " - " + ex.Message);
                            return false;
                        }
                    }
                }
                #endregion
                #region DB Storage MySQl
                else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MYSQL")
                {
                    try
                    {
                        var dbCon = DBConnection.Instance();
                        if (dbCon.IsConnect())
                        {
                            //suppose col0 and col1 are defined as VARCHAR in the DB
                            string query = "DELETE FROM `ROUTES` WHERE IdSource = " + route.IdSource + " AND IdDestiny = " + route.IdDestination;
                            var cmd = new MySqlCommand(query, dbCon.Connection);
                            var reader = cmd.ExecuteNonQuery();
                            dbCon.Close();
                            return true;
                        }
                    }
                    catch (Exception ex)
                    {
                        new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() + " - " + ex.Message);
                        return false;
                    }
                }
                #endregion DB Storage MySQl
                #region DB Storage Mongo
                else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MONGO")
                {
                    var dbCon = DBConnectionMongo.Instance();
                    MongoDB.Driver.MongoClient client = dbCon.GetClient();
                    client.StartSession();
                    var collection = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]).GetCollection<Route>("ROUTES");
                    var Deleteone = collection.DeleteOneAsync(Builders<Route>.Filter.And(
                                      Builders<Route>.Filter.Eq("IdSource", route.IdSource),
                                      Builders<Route>.Filter.Eq("IdDestination", route.IdDestination)));
                    return true;
                }
                #endregion DB Storage Mongo
            }
            catch (Exception ex)
            {
                new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() +" - " +ex.Message);
            }
            return false;
        }

        public bool UpdateRoute(Route route)
        {
            try
            {
                #region Validations
                List<Route> routes = new RouteRepository().GetAllRoutes().ToList();
                if (!routes.Exists(r => r.IdSource == route.IdSource && r.IdDestination == route.IdDestination))
                    return false;
                #endregion Validations

                #region Cache Storage
                if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "CACHE")
                {
                    var ctx = HttpContext.Current;
                    if (ctx != null)
                    {
                        try
                        {
                            var currentData = ((Route[])ctx.Cache[CacheKey]).ToList();
                            if (currentData.Exists(p => p.IdSource == route.IdSource && p.IdDestination == route.IdDestination))
                            {
                                if (currentData[currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestination == route.IdDestination)].RouteCost != route.RouteCost && route.RouteCost > 0)
                                    currentData[currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestination == route.IdDestination)].RouteCost = route.RouteCost;
                                if (currentData[currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestination == route.IdDestination)].RouteTime != route.RouteTime && route.RouteTime > 0)
                                    currentData[currentData.FindIndex(p => p.IdSource == route.IdSource && p.IdDestination == route.IdDestination)].RouteTime = route.RouteTime;
                            }
                            ctx.Cache[CacheKey] = currentData.ToArray();

                            return true;
                        }
                        catch (Exception ex)
                        {
                            new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() + " - " + ex.Message);
                            return false;
                        }
                    }
                }
                #endregion
                #region DB Storage MySQl
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
                                    string query = "UPDATE `ROUTES` SET `RouteCost`=" + route.RouteCost + "ss WHERE IdSource=" + route.IdSource + " AND IdDestiny = " + route.IdDestination;
                                    var cmd = new MySqlCommand(query, dbCon.Connection);
                                    var reader = cmd.ExecuteNonQuery();
                                }
                            }
                            if (!string.IsNullOrEmpty(route.RouteTime.ToString()))
                            {
                                if (route.RouteTime > 0)
                                {
                                    string query = "UPDATE `ROUTES` SET `RouteTime`=" + route.RouteTime + " WHERE IdSource=" + route.IdSource + " AND IdDestiny = " + route.IdDestination;
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
                        new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() + " - " + ex.Message);
                        return false;
                    }
                }
                #endregion DB Storage MySQl
                #region DB Storage Mongo
                else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MONGO")
                {
                    var dbCon = DBConnectionMongo.Instance();
                    MongoDB.Driver.MongoClient client = dbCon.GetClient();
                    client.StartSession();
                    var collection = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]).GetCollection<Route>("ROUTES");

                    if (!string.IsNullOrEmpty(route.RouteCost.ToString()))
                    {
                        if (route.RouteCost > 0)
                        {
                            var updoneresult = collection.UpdateOneAsync(Builders<Route>.Filter.And(
                                      Builders<Route>.Filter.Eq("IdSource", route.IdSource),
                                      Builders<Route>.Filter.Eq("IdDestination", route.IdDestination)), Builders<Route>.Update.Set("RouteCost", route.RouteCost));
                        }
                    }
                    if (!string.IsNullOrEmpty(route.RouteTime.ToString()))
                    {
                        if (route.RouteTime > 0)
                        {
                            var updoneresult = collection.UpdateOneAsync(Builders<Route>.Filter.And(
                                   Builders<Route>.Filter.Eq("IdSource", route.IdSource),
                                   Builders<Route>.Filter.Eq("IdDestination", route.IdDestination)), Builders<Route>.Update.Set("RouteTime", route.RouteTime));
                        }
                    }
                    return true;
                }
                #endregion DB Storage Mongo
            }
            catch (Exception ex)
            {
                new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() +" - " +ex.Message);
            }
            return false;
        }
    }
}