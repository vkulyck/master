using GmWeb.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;

namespace GmWeb.Logic.Utility.Extensions
{
    public static class TableDataExtensions
    {
        public static dynamic ToDynamic(this DataTable table)
        {
            var rows = new List<Dictionary<string, object>>();
            foreach (DataRow dr in table.Rows)
            {
                var row = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    row.Add(col.ColumnName, dr[col]);
                }
                rows.Add(row);
            }
            return rows;
        }

        /// <summary>
        /// Returns an Object with the specified Type and whose value is equivalent to the specified object.
        /// </summary>
        /// <param name="value">An Object that implements the IConvertible interface.</param>
        /// <param name="conversionType">The Type to which value is to be converted.</param>
        /// <returns>An object whose Type is conversionType (or conversionType's underlying type if conversionType
        /// is Nullable&lt;&gt;) and whose value is equivalent to value. -or- a null reference, if value is a null
        /// reference and conversionType is not a value type.</returns>
        /// <remarks>
        /// This method exists as a workaround to System.Convert.ChangeType(Object, Type) which does not handle
        /// nullables as of version 2.0 (2.0.50727.42) of the .NET Framework. The idea is that this method will
        /// be deleted once Convert.ChangeType is updated in a future version of the .NET Framework to handle
        /// nullable types, so we want this to behave as closely to Convert.ChangeType as possible.
        /// This method was written by Peter Johnson at:
        /// http://aspalliance.com/author.aspx?uId=1026.
        /// </remarks>
        public static object ChangeType(this object value, Type conversionType)
        {
            if (value.GetType() == typeof(DBNull))
                value = null;
            // Note: This if block was taken from Convert.ChangeType as is, and is needed here since we're
            // checking properties on conversionType below.
            if (conversionType == null)
            {
                throw new ArgumentNullException("conversionType");
            } // end if

            // If it's not a nullable type, just pass through the parameters to Convert.ChangeType

            if (conversionType.IsGenericType &&
              conversionType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                // It's a nullable type, so instead of calling Convert.ChangeType directly which would throw a
                // InvalidCastException (per http://weblogs.asp.net/pjohnson/archive/2006/02/07/437631.aspx),
                // determine what the underlying type is
                // If it's null, it won't convert to the underlying type, but that's fine since nulls don't really
                // have a type--so just return null
                // Note: We only do this check if we're converting to a nullable type, since doing it outside
                // would diverge from Convert.ChangeType's behavior, which throws an InvalidCastException if
                // value is null and conversionType is a value type.
                if (value == null)
                {
                    return null;
                } // end if

                // It's a nullable type, and not null, so that means it can be converted to its underlying type,
                // so overwrite the passed-in conversion type with this underlying type
                var nullableConverter = new NullableConverter(conversionType);
                conversionType = nullableConverter.UnderlyingType;
            } // end if

            // Now that we've guaranteed conversionType is something Convert.ChangeType can handle (i.e. not a
            // nullable type), pass the call on to Convert.ChangeType
            return Convert.ChangeType(value, conversionType);
        }

        public static void PerformDefaultRowConversion(this IDataRowModel model, DataRow row)
        {
            var publicAttributes = BindingFlags.Public | BindingFlags.Instance;
            var tableFields = row.Table.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToHashSet();
            var modelFields = model.GetType()
                .GetProperties(publicAttributes)
                .Where(x => x.CanRead && x.CanWrite)
                .Select(x => x.Name)
                .ToHashSet();
            ;
            var sharedFields = modelFields.Intersect(tableFields).ToHashSet();
            foreach (string field in sharedFields)
            {
                var property = model.GetType().GetProperty(field);
                object value = row[field];
                object cast = value.ChangeType(property.PropertyType);
                property.SetValue(model, cast);
            }
        }

        public static List<T> RowsToModels<T>(this DataTable table, DbContext context) where T : IDataRowModel, new()
        {
            var models = new List<T>();
            foreach (DataRow row in table.Rows)
            {
                var model = new T();
                if (model is IMultiQueryDataRowModel)
                    model.LoadDataRow(row, new GmWeb.Logic.Data.Context.Account.AccountContext());
                else
                    model.LoadDataRow(row);
                models.Add(model);
            }
            return models;
        }

        public static List<string> RowsToStrings(this DataTable table)
        {
            var items = new List<string>();
            foreach (DataRow row in table.Rows)
            {
                string item = row.ItemArray[0]?.ToString();
                items.Add(item);
            }
            return items;
        }

        public static List<string> RowsToStrings(this DataTable table, string field)
        {
            var items = new List<string>();
            if (!table.Columns.Contains(field))
            {
                var cols = table.Columns.OfType<DataColumn>().Select(x => x.ColumnName).ToList();
                string joined = string.Join(", ", cols);
                throw new ArgumentException($"No such field '{field}' exists in table columns: {joined}");
            }
            var column = table.Columns[field];
            foreach (DataRow row in table.Rows)
            {
                string item = row[column]?.ToString();
                items.Add(item);
            }
            return items;
        }

        /// <summary>
        /// This method exists because many of the database procedures insert empty header-style rows
        /// into their result sets as a way to alter the UI. Eventually we will need to remove these
        /// rows at the data source, and allow the UI to handle the headers.
        /// 
        /// For now this extension method will allow us to encapsulate the workaround and mark the 
        /// affected data access calls.
        /// </summary>
        /// <param name="table">
        /// A table whose first row is empty and/or filled with a header row
        /// intended to be displayed at the top of a dropdown list.
        /// </param>
        /// <returns>The input table without the initial header row.</returns>
        public static DataTable RemoveEmptyRow(this DataTable table)
        {
            table.Rows.RemoveAt(0);
            return table;
        }

        public static string ToString(this DataRow row, string column) => row[column]?.ToString();
    }
}
