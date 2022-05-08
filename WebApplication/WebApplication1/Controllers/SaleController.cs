using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using System.Diagnostics;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Newtonsoft.Json;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WebApplication1.Controllers
{
    [Route("process/sale")]
    [ApiController]
    public class SaleController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public SaleController(IConfiguration configuration) 
        {
            _configuration = configuration;
        }


        [HttpPost]

        public StatusCodeResult PostSale(object json)
        {
            long size = 0;
            using (Stream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, json.ToString());
                size = stream.Length;
                Debug.WriteLine("{0}", size);
                if (size > 2 * 1024)
                {
                    //return new JsonResult("Big data");
                    return StatusCode(413);
                }
            }

            string jsonString = json.ToString();
            JSchema schema = JSchema.Parse(System.IO.File.ReadAllText("SchemaSale.json"));
            JObject jsonObject = JObject.Parse(jsonString);
            IList<string> validationEvents = new List<string>();
            if (jsonObject.IsValid(schema, out validationEvents))
            {
                string ticket_number_readed = (string)jsonObject["passenger"]["ticket_number"];
                string request1 = @"select count(ticket_number) as count_ticket from segments where (ticket_number = @ticket_number)";

                DataTable table = new DataTable();
                string sqlDataSource = _configuration.GetConnectionString("EmployeeAppCon");
                NpgsqlDataReader myReader;
                using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource))
                {
                    myCon.Open();
                    using (NpgsqlCommand myCommand = new NpgsqlCommand(request1, myCon))
                    {
                        myCommand.Parameters.AddWithValue("@ticket_number", ticket_number_readed);
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);

                        myReader.Close();
                        myCon.Close();
                    }
                }

                if (table.Rows[0]["count_ticket"].ToString() == "0")
                {
                    var routes = jsonObject.Value<JArray>("routes").Children<JObject>();
                    foreach (var obj in routes)
                    {
                        Segments seg = System.Text.Json.JsonSerializer.Deserialize<Segments>(obj.ToString());
                        string query = @"
                        insert into Segments(airline_code, flight_num, depart_place, depart_datetime,arrive_place, arrive_datetime, pnr_id, ticket_number, passed) 
                        values (@airline_code, @flight_num, @depart_place, @depart_datetime, @arrive_place, @arrive_datetime, @pnr_id, @ticket_number, @passed)";

                        DataTable table_result = new DataTable();
                        string sqlDataSource_result = _configuration.GetConnectionString("EmployeeAppCon");
                        NpgsqlDataReader myReader_result;
                        using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource_result))
                        {
                            myCon.Open();
                            using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                            {
                                myCommand.Parameters.AddWithValue("@airline_code", seg.airline_code);
                                myCommand.Parameters.AddWithValue("@flight_num", seg.flight_num);
                                myCommand.Parameters.AddWithValue("@depart_place", seg.depart_place);
                                myCommand.Parameters.AddWithValue("@depart_datetime", seg.depart_datetime);
                                myCommand.Parameters.AddWithValue("@arrive_place", seg.arrive_place);
                                myCommand.Parameters.AddWithValue("@arrive_datetime", seg.arrive_datetime);
                                myCommand.Parameters.AddWithValue("@pnr_id", seg.pnr_id);
                                myCommand.Parameters.AddWithValue("@ticket_number", ticket_number_readed);
                                myCommand.Parameters.AddWithValue("@passed", false);
                                myReader_result = myCommand.ExecuteReader();
                                table.Load(myReader_result);

                                myReader_result.Close();
                                myCon.Close();
                            }
                        }
                    }

                    //return new JsonResult("Added Successfully");
                    return StatusCode(200);

                } else {
                    //return new JsonResult("The ticket already exists");
                    return StatusCode(409);
                }

            } else 
            {
                //return new JsonResult(validationEvents.ToString());
                return StatusCode(400);
            }
        }



    }
}
