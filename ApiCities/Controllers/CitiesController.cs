using ApiCities.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Data.SqlClient;
using Newtonsoft.Json;
using MySqlConnector;


namespace ApiCities.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        public readonly IConfiguration _configuration;

        public CitiesController(IConfiguration configuration)
        {

            _configuration = configuration;

        }

        [HttpGet]
        [Route("GetAllCities")]
        public string GetCities()
        {
            Response response = new Response();
            string cs = _configuration.GetConnectionString("MapAppCon").ToString();
            try
            {
                using var con = new MySqlConnection(cs);
                con.Open();
                //Requete sql
                var query = "SELECT * FROM map.cities";
                // Create a new SqlDataAdapter with the SQL query and the open connection
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);

                // Create a new DataTable
                DataTable dt = new DataTable();

                // Fill the DataTable with the data from the database
                da.Fill(dt);

                // Close the connection after using it
                con.Close();
                List<City> lstCities = new List<City>();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        City cities = new City
                        {
                            Name = Convert.ToString(dt.Rows[i]["name"]),
                            Longitude = Convert.ToDouble(dt.Rows[i]["Latitude"]),
                            Latitude = Convert.ToDouble(dt.Rows[i]["Longitude"]),
                            Region = Convert.ToString(dt.Rows[i]["region"]),
                            Population = Convert.ToInt32(dt.Rows[i]["population"])
                        };
                        lstCities.Add(cities);
                    }
                }
                if (lstCities.Count > 0)
                {
                    return JsonConvert.SerializeObject(lstCities);
                }
                else
                {
                    response.StatusCode = 100;
                    response.ErrorMessage = "No Data Found";
                    return JsonConvert.SerializeObject(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ErrorMessage = ex.Message + ex.StackTrace;
                return JsonConvert.SerializeObject(response);
            }
        }

        [HttpGet]
        [Route("GetCityInPerimeter/{longitude}/{latitude}/{rayon}")]
        public string GetCityInPerimeter(double latitude, double longitude, double rayon)
        {
            double rayonTerre = 6371;
            Response response = new Response();
            string cs = _configuration.GetConnectionString("MapAppCon").ToString();
            try
            {
                using var con = new MySqlConnection(cs);
                con.Open();
                //Requete sql
                var query = "SELECT * FROM map.cities";
                // Create a new SqlDataAdapter with the SQL query and the open connection
                MySqlDataAdapter da = new MySqlDataAdapter(query, con);

                // Create a new DataTable
                DataTable dt = new DataTable();

                // Fill the DataTable with the data from the database
                da.Fill(dt);

                // Close the connection after using it
                con.Close();
                List<City> lstCities = new List<City>();
                double distance = 0;
                City city = null;
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        city = new City
                        {
                            Name = Convert.ToString(dt.Rows[i]["name"]),
                            Longitude = Convert.ToDouble(dt.Rows[i]["Longitude"]),
                            Latitude = Convert.ToDouble(dt.Rows[i]["Latitude"]),
                            Region = Convert.ToString(dt.Rows[i]["region"]),
                            Population = Convert.ToInt32(dt.Rows[i]["population"])
                        };

                        double DifLatNewCity = city.ToRadians(city.Latitude - latitude);
                        double DifLongNewCity = city.ToRadians(city.Longitude - longitude);

                        double a = Math.Sin(DifLatNewCity / 2) * Math.Sin(DifLatNewCity / 2) +
                                   Math.Cos(city.ToRadians(latitude)) * Math.Cos(city.ToRadians(city.Latitude)) *
                                   Math.Sin(DifLongNewCity / 2) * Math.Sin(DifLongNewCity / 2);

                        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

                        distance = rayonTerre * c;

                        if (distance <= rayon)
                        {
                            lstCities.Add(city);
                        }
                    }
                }
                if (lstCities.Count > 0)
                {
                    return JsonConvert.SerializeObject(lstCities);
                }
                else
                {
                    response.StatusCode = 100;
                    response.ErrorMessage = "No Data Found " + distance + " " + city.Name + " " + city.Latitude + " " + city.Longitude;
                    return JsonConvert.SerializeObject(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.ErrorMessage = ex.Message + ex.StackTrace;
                return JsonConvert.SerializeObject(response);
            }
        }
    }
}
