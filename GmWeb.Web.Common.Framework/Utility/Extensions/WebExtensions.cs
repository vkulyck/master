using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;

namespace GmWeb.Web.Common.Utility
{
    public static class WebExtensions
    {
        public static Dictionary<string, object> ToDictionary(this object o)
        {
            var dictionary = new Dictionary<string, object>();

            foreach (var propertyInfo in o.GetType().GetProperties())
            {
                if (propertyInfo.GetIndexParameters().Length == 0)
                {
                    dictionary.Add(propertyInfo.Name, propertyInfo.GetValue(o, null));
                }
            }

            return dictionary;
        }

        public static ViewDataDictionary ToViewDataDictionary(this object o)
        {
            if (o == null) return null;

            IDictionary<string, object> dict = o.ToDictionary();
            ViewDataDictionary vd = new ViewDataDictionary();
            foreach (var item in dict)
                vd[item.Key] = item.Value;
            return vd;
        }

        public static Dictionary<string,object> ToDictionary(this HttpSessionStateBase session)
        {
            var dict = new Dictionary<string, object>();
            foreach(string key in session.Keys)
                dict[key] = session[key];
            return dict;
        }

        public static Dictionary<string, T> ToDictionary<T>(this HttpSessionStateBase session) where T : class
        {
            var dict = new Dictionary<string, T>();
            foreach (string key in session.Keys)
                dict[key] = session[key] as T;
            return dict;
        }

        public static List<SelectListItem> ToListItems(this DataTable table, string valueColumn = "ID", string textColumn = "Description")
        {
            var items = new List<SelectListItem>();
            foreach(DataRow row in table.Rows)
            {
                var item = new SelectListItem
                {
                    Value = row.ToString(valueColumn),
                    Text = row.ToString(textColumn)
                };
                items.Add(item);
            }
            return items;
        }
    }
}