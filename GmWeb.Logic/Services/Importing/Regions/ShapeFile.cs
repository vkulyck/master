using NetTopologySuite.Geometries;
using NetTopologySuite.IO.ShapeFile.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DbfReader = DbfDataReader.DbfDataReader;

namespace GmWeb.Logic.Importing.Regions
{
    public class ShapeFile : IDisposable
    {
        public static readonly int SRID = 4326;
        protected bool IsDisposed { get; set; } = false;
        public string ShapePath { get; private set; }
        public bool HasShapes { get; private set; }
        public int ShapeCount => (int)this.DbfReader.DbfTable.Header.RecordCount;
        public int CurrentShapeIndex { get; private set; } = -1;
        protected string DbfPath => Regex.Replace(this.ShapePath, $@"\.shp$", ".dbf");
        protected DbfReader DbfReader { get; private set; }
        protected ShapeReader ShapeReader { get; private set; }
        protected IEnumerator<Geometry> ShapeIterator { get; private set; }
        protected GeometryFactory GeometryFactory { get; } = new GeometryFactory(new PrecisionModel(), SRID);

        public ShapeFile(string path)
        {
            this.ShapePath = path;
            this.DbfReader = new DbfReader(this.DbfPath);
            this.ShapeReader = new ShapeReader(this.ShapePath);
            this.ShapeIterator = this.ShapeReader.ReadAllShapes(this.GeometryFactory).Cast<Geometry>().GetEnumerator();
            this.HasShapes = this.ShapeIterator.MoveNext();
        }

        public T ReadMetadata<T>() where T : new()
        {
            var record = this.DbfReader.ReadRecord();
            var table = this.DbfReader.DbfTable;
            var model = new T();
            foreach (var column in table.Columns)
            {
                var property = typeof(T).GetProperty(column.Name);
                object value = record.Values[column.Index].GetValue();
                if ( // Use actual decimals where possible
                    column.ColumnType == DbfDataReader.DbfColumnType.Character
                    && property.PropertyType == typeof(decimal)
                )
                    value = Convert.ToDecimal(value);
                if (property != null && property.SetMethod != null)
                    property.SetMethod.Invoke(model, new object[] { value });
            }
            return model;
        }

        public Geometry ReadShape()
        {
            this.CurrentShapeIndex++;
            var geometry = this.ShapeIterator.Current;
            this.HasShapes = this.ShapeIterator.MoveNext();
            return geometry;
        }

        public void Dispose()
        {
            if (this.IsDisposed)
                return;
            this.ShapeIterator.Dispose();
            this.ShapeReader.Dispose();
            this.DbfReader.Dispose();
            this.IsDisposed = true;
        }
    }
}
