using GmWeb.Logic.Data.Models.Shared;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GmWeb.Logic.Data.Models.Profile
{
    public class ExtendedClient : BaseDataModel
    {
        public int ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName => $"{this.FirstName} {this.LastName}";
        public string Zip
        {
            get
            {
                if (this.ProfileFieldMap.TryGetValue("Zip", out var result))
                    return result.RawValue;
                return null;
            }
        }
        public string City
        {
            get
            {
                if (this.ProfileFieldMap.TryGetValue("City", out var result))
                    return result.RawValue;
                return null;
            }
        }
        public int? Age
        {
            get
            {
                if (this.ProfileFieldMap.TryGetValue("Age", out var result))
                    return Convert.ToInt32(result.RawValue);
                return null;
            }
        }
        public int? FamilySize
        {
            get
            {
                if (this.ProfileFieldMap.TryGetValue("Family Size", out var result))
                    return Convert.ToInt32(result.RawValue);
                return null;
            }
        }

        private Dictionary<string, DynamicFieldValue> _map;
        public Dictionary<string, DynamicFieldValue> ProfileFieldMap
        {
            get
            {
                if (this._map == null)
                    this._map = this.ProfileFields.ToDictionary(x => x.Name);
                return this._map;
            }
        }
        public virtual IList<DynamicFieldValue> ProfileFields { get; set; } = new List<DynamicFieldValue>();
    }
}
