using Newtonsoft.Json.Linq;
using RaynetEventTracker.Models;
using Simple.Rest.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace RaynetEventTracker.Controllers
{
    public class AprsDataController : ApiController
    {
        private const string c_ApiKey = "21888.ptyZlTEPX0BQYHvj";

        [HttpGet]
        public async Task<IEnumerable<Station>> Stations()
        {
            var client = new Simple.Rest.RestClient(new JsonSerializer());
            Dictionary<string, string> callsignMappings = new Dictionary<string, string>
            {
                { "M0VFC-4", "Oscar" },
                { "M0VFC-7", "Sweep" },
                { "M0VFC-5", "Sweep" },
            };
            var callsignsToCheck = callsignMappings.Keys;
            var result = await client.GetAsync<dynamic>(new Uri(string.Format("http://api.aprs.fi/api/get?name={0}&what=loc&apikey={1}&format=json", string.Join(",", callsignsToCheck) , c_ApiKey)));
            string status = result.Resource.result;
            switch (status)
            {
                case "ok":
                    break;
                default:
                    throw new System.Web.HttpException(500, "APRS.fi said no");
            }

            JArray entries = result.Resource.entries;

            Dictionary<string, Station> stations = new Dictionary<string, Station>();
            foreach (var entry in entries)
            {
                Station existingStation;
                Station potentialStation = new Station { 
                    Name = callsignMappings[(string)entry["name"]], 
                    Lat = (double)entry["lat"], 
                    Long = (double)entry["lng"], 
                    LastSeen = FromUnixTimestamp((long)entry["lasttime"]) 
                };

                if (stations.TryGetValue(potentialStation.Name, out existingStation))
                {
                    if (potentialStation.LastSeen > existingStation.LastSeen)
                        stations[potentialStation.Name] = potentialStation;
                }
                else
                {
                    stations[potentialStation.Name] = potentialStation;
                }
            }
            return stations.Values;
        }

        private static DateTime FromUnixTimestamp(long timestamp)
        {
            return new DateTime(1970, 1, 1).AddSeconds(timestamp);
        }
    }
}
