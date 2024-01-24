using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;
using GmWeb.Logic.Data.Models.Geography;
using GmWeb.Logic.Data.Models.Profile;
using Newtonsoft.Json;

namespace GmWeb.Web.Demographics.Logic.DataModels
{
    public class ClientMarker : GeoRegion
    {
        // Identifying information is ignored during serialization/web transmition for now.
        [JsonIgnore]
        public override int ID { get => this.ClientID; set => this.ClientID = value; }
        [JsonIgnore]
        public int ClientID { get; set; }
        [JsonIgnore]
        public string FirstName { get; set; }
        [JsonIgnore]
        public string LastName { get; set; }
        [JsonIgnore]
        public string FullName => $"{this.FirstName} {this.LastName}";
        [JsonIgnore]
        public override string Name { get => this.FullName; set => throw new NotImplementedException(); }

        public ClientMarker() { }
        public ClientMarker(Client c)
        {
            this.FirstName = c.FirstName;
            this.LastName = c.LastName;
            this.GEOM = new Point(c.Longitude.Value, c.Latitude.Value);
        }
    }
}