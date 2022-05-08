﻿using Microsoft.AspNetCore.Http;
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
using System.Runtime.InteropServices;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace WebApplication1.Controllers
{
    [Route("process/refund")]
    [ApiController]
    public class RefundController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public RefundController(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        [HttpPost]

        public StatusCodeResult PostRefund(object json)
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
            JSchema schema = JSchema.Parse(System.IO.File.ReadAllText("SchemaRefund.json"));
            JObject jsonObject = JObject.Parse(jsonString);
            IList<string> validationEvents = new List<string>();
            if (jsonObject.IsValid(schema, out validationEvents))
            {
                string ticket_number_readed = (string)jsonObject["ticket_number"];
                string request1 = @"select count(ticket_number) as count_ticket from segments where (ticket_number = @ticket_number and passed = false)";

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

                if (table.Rows[0]["count_ticket"].ToString() != "0")
                {
                    string query = @"update segments set passed = true where ticket_number = @ticket_number";

                    DataTable table_result = new DataTable();
                    string sqlDataSource_result = _configuration.GetConnectionString("EmployeeAppCon");
                    NpgsqlDataReader myReader_result;
                    using (NpgsqlConnection myCon = new NpgsqlConnection(sqlDataSource_result))
                    {
                        myCon.Open();
                        using (NpgsqlCommand myCommand = new NpgsqlCommand(query, myCon))
                        {
                            myCommand.Parameters.AddWithValue("@ticket_number", ticket_number_readed);
                            myReader_result = myCommand.ExecuteReader();
                            table.Load(myReader_result);

                            myReader_result.Close();
                            myCon.Close();
                        }
                    }

                    //return new JsonResult("Refund Successfully");
                    return StatusCode(200);
                }
                else
                {
                    //return new JsonResult("The ticket has already been handed over");
                    return StatusCode(409);
                }

            }
            else
            {
                //return new JsonResult(validationEvents.ToString());
                return StatusCode(400);
            }
        }
    }
}
