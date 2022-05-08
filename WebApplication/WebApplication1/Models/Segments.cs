using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Segments
    {
        public int id_segment { get; set; }
        public string airline_code { get; set; }
        public int flight_num { get; set; }
        public string depart_place { get; set; }
        public DateTime depart_datetime { get; set; }
        public string arrive_place { get; set; }
        public DateTime arrive_datetime { get; set; }
        public string pnr_id { get; set; }
        public string ticket_number { get; set; }
        public bool passed { get; set; }
    }

}
