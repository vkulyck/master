using GmWeb.Logic.Data.Models.Profile;
using NetTopologySuite.Geometries;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using Point = NetTopologySuite.Geometries.Point;

namespace GmWeb.Logic.Data.Models.Geography
{
    public abstract class GeoRegion : BaseDataModel
    {
        public abstract int ID { get; set; }
        [NotMapped]
        public abstract string Name { get; set; }
        public Geometry GEOM { get; set; }
        public double Latitude => this.GEOM.Centroid.Y;
        public double Longitude => this.GEOM.Centroid.X;

        public int ClientCount(IQueryable<Client> clients) => this.ClientCount(clients, null);
        public int ClientCount(IQueryable<Client> clients, Expression<Func<Client, bool>> filter)
        {
            if (filter != null)
                clients = clients.Where(filter);
            clients = clients
                .Where(x => x.Longitude.HasValue).Where(x => x.Latitude.HasValue)
                .Where(x => this.GEOM.Contains(new Point(x.Longitude.Value, x.Latitude.Value)))
            ;
            int nClients = clients.Count();
            return nClients;
        }
    }
}
