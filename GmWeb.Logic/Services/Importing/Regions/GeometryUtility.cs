using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Importing.Regions
{
    public static class GeometryUtility
    {
        public static Geometry SanitizeShape(Geometry geometry)
        {
            var poly = geometry as Polygon;
            var multi = geometry as MultiPolygon;
            if (poly != null)
            {
                if (!poly.Shell.IsCCW)
                    poly = (Polygon)poly.Reverse();
                return poly;
            }
            else if (multi != null)
            {
                var duplicateMulti = multi.First();
                if (duplicateMulti != multi)
                {
                    throw new Exception($"Multipolygon contains a multipolygonal child that isn't itself.");
                }
                var childPolies = multi.Skip(1).ToList();
                if (childPolies.Any(x => !(x is Polygon)))
                {
                    throw new Exception($"MultiPolygon contains multipolygon children.");
                }
                multi = Reorient(multi);
                return multi;
            }
            else
                throw new Exception($"Invalid tract geometry identified.");
        }

        public static MultiPolygon Reorient(MultiPolygon parent)
        {
            var children = parent.Geometries.OfType<Polygon>();
            bool isCCW = children.Any(x => x.Shell.IsCCW);
            if (children.Any(x => x.Shell.IsCCW != isCCW))
                throw new Exception($"Heterogeneous child polygon orientation.");
            if (!isCCW)
            {
                parent = (MultiPolygon)parent.Reverse();
                children = parent.Geometries.OfType<Polygon>();
                isCCW = children.Any(x => x.Shell.IsCCW);
                bool isCW = children.Any(x => !x.Shell.IsCCW);
                if (isCW)
                    throw new Exception($"Clockwise child detected after parent's explicit reversal.");
            }

            // BEGIN CHECKS
            // TODO: Remove these checks after they're confirmed to hold for the entire dataset
            var shells = new List<(bool, bool, bool, bool, bool, bool)>();
            var check = new HashSet<(bool, bool, bool, bool, bool, bool)>();
            var rChildren = new List<Polygon>();
            for (int i = 0; i < parent.Count; i++)
            {
                var child = (Polygon)parent[i];
                shells.Add((child.Shell.IsCCW, child.Shell.IsClosed, child.Shell.IsEmpty, child.Shell.IsRing, child.Shell.IsSimple, child.Shell.IsValid));
                check.Add((child.Shell.IsCCW, child.Shell.IsClosed, child.Shell.IsEmpty, child.Shell.IsRing, child.Shell.IsSimple, child.Shell.IsValid));
                if (!child.Shell.IsCCW)
                    child = child.Reverse() as Polygon;
                rChildren.Add(child);
            }
            var rebuilt = new MultiPolygon(rChildren.ToArray(), parent.Factory);
            var reversed = isCCW ? parent : parent.Reverse() as MultiPolygon;
            bool compare = (reversed == rebuilt);
            if (!compare || check.Count != 1)
                throw new Exception($"Child orientation mismatches.");
            // END CHECKS
            return parent;
        }
    }
}
