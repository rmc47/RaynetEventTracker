using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RaynetEventTracker.Models
{
    public class Station
    {
        public double Lat { get; set; }
        public double Long { get; set; }
        public string Name { get; set; }
        public DateTime LastSeen { get; set; }
    }
}