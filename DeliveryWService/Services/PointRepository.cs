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
    public class PointRepository
    {
        private const string CacheKey = "PointStore";

        public PointRepository()
        {
            var ctx = HttpContext.Current;

            if (ctx != null)
            {
                if (ctx.Cache[CacheKey] == null)
                {
                    var points = new Point[]
                    {
                        new Point
                        {
                            Id = 1, Name = "A"
                        },
                        new Point
                        {
                            Id = 2, Name = "B"
                        },
                        new Point
                        {
                            Id = 3, Name = "C"
                        },
                        new Point
                        {
                            Id = 4, Name = "D"
                        },
                        new Point
                        {
                            Id = 5, Name = "E"
                        },
                        new Point
                        {
                            Id = 6, Name = "F"
                        },
                        new Point
                        {
                            Id = 7, Name = "G"
                        },
                        new Point
                        {
                            Id = 8, Name = "H"
                        },
                        new Point
                        {
                            Id = 9, Name = "I"
                        }
                    };

                    ctx.Cache[CacheKey] = points;
                }
            }
        }

        public Point[] GetAllPoints()
        {
            List<Point> allPoints = new List<Point>();
            try
            {
                #region Cache Storage
                if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "CACHE")
                {
                    var ctx = HttpContext.Current;
                    if (ctx != null)
                    {
                        return (Point[])ctx.Cache[CacheKey];
                    }

                    return new Point[]
                {
                    new Point
                    {
                        Id = 0,
                        Name = "Placeholder"
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
                        string query = "SELECT Id, Name FROM POINTS";
                        var cmd = new MySqlCommand(query, dbCon.Connection);
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            Point point = new Point();
                            point.Id = reader.GetInt32(0);
                            point.Name = reader.GetString(1);
                            allPoints.Add(point);
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
                    var collection = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]).GetCollection<Point>("POINTS");
                    allPoints = collection.Find(_ => true).ToList();
                }
                #endregion DB Storage Mongo
            }
            catch (Exception ex)
            {
                new DeliveryWService.Services.LogService(true).Error(DeliveryWService.Services.LogServiceAux.GetCurrentMethod() +" - " +ex.Message);
            }
            
            return allPoints.ToArray();
        }

        public bool SavePoint(Point point)
        {
            try
            {
                #region Validations
                List<Point> points = new PointRepository().GetAllPoints().ToList();
                if (points.Exists(p => p.Name == point.Name))
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
                            var currentData = ((Point[])ctx.Cache[CacheKey]).ToList();

                            if (point.Id == -1)
                            {
                                var maxItemID = currentData.Max(x => x.Id);
                                point.Id = maxItemID + 1;
                            }
                            currentData.Add(point);
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
                            string query = "INSERT INTO `POINTS`(`Name`) VALUES ('" + point.Name + "')";
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
                    point.Id = Convert.ToInt32(Sequence.GetNextSequenceValue("pointid", client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"])));
                    var collection = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]).GetCollection<Point>("POINTS");
                    collection.InsertOne(point);
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

        public bool DeletePoint(Point point)
        {
            try
            {
                #region Validations
                List<Point> points = new PointRepository().GetAllPoints().ToList();
                if (!points.Exists(p => p.Id == point.Id))
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
                            var currentData = ((Point[])ctx.Cache[CacheKey]).ToList();
                            if (currentData.Exists(p => p.Id == point.Id))
                            {
                                currentData.RemoveAt(currentData.FindIndex(p => p.Id == point.Id));
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
                #region DB Storage MySQl
                else if (ConfigurationManager.AppSettings["dbType"].ToUpper() == "DB_MYSQL")
                {
                    try
                    {
                        var dbCon = DBConnection.Instance();
                        if (dbCon.IsConnect())
                        {
                            string query = "DELETE FROM `ROUTES` WHERE IdSource=" + point.Id + " OR IdDestination = " + point.Id;
                            var cmd = new MySqlCommand(query, dbCon.Connection);
                            var reader = cmd.ExecuteNonQuery();

                            query = "DELETE FROM `POINTS` WHERE Id=" + point.Id;
                            cmd = new MySqlCommand(query, dbCon.Connection);
                            reader = cmd.ExecuteNonQuery();
                            dbCon.Close();
                            if (reader == 1)
                                return true;
                            else
                                return false;
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

                    var collectionR = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]).GetCollection<Route>("ROUTES");
                    var Deleteall = collectionR.DeleteMany(Builders<Route>.Filter.Or(
                                      Builders<Route>.Filter.Eq("IdSource", point.Id),
                                      Builders<Route>.Filter.Eq("IdDestination", point.Id)));

                    var collection = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]).GetCollection<Point>("POINTS");
                    var Deleteone = collection.DeleteOneAsync(Builders<Point>.Filter.Eq("Id", point.Id));
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

        public bool UpdatePoint(Point point)
        {
            try
            {
                #region Validations
                List<Point> points = new PointRepository().GetAllPoints().ToList();
                if (!points.Exists(p => p.Id == point.Id))
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
                            var currentData = ((Point[])ctx.Cache[CacheKey]).ToList();
                            if (currentData.Exists(p => p.Id == point.Id))
                            {
                                currentData[currentData.FindIndex(x => x.Id == point.Id)].Name = point.Name;
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
                            string query = "UPDATE `POINTS` SET `Name`='" + point.Name + "' WHERE Id=" + point.Id;
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
                    var collection = client.GetDatabase(ConfigurationManager.AppSettings["mongoDBName"]).GetCollection<Point>("POINTS");
                    var updoneresult = collection.UpdateOneAsync(Builders<Point>.Filter.Eq("Id", point.Id), Builders<Point>.Update.Set("Name", point.Name));
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