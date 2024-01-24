using GmWeb.Logic.Enums;
using GmWeb.Logic.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using Json = Newtonsoft.Json.JsonConvert;
using JsonObjectAttribute = Newtonsoft.Json.JsonObjectAttribute;
using JsonPropertyAttribute = Newtonsoft.Json.JsonPropertyAttribute;
using JsonSerializerSettings = Newtonsoft.Json.JsonSerializerSettings;
using NullValueHandling = Newtonsoft.Json.NullValueHandling;

namespace GmWeb.Logic.Data.Models.Shared
{
    [Table("Clients", Schema = "profile")]
    [JsonObjectAttribute("ProfileData")]
    public class DynamicFieldValue : IDynamicValueType
    {
        #region Database Configurators
        public static readonly JsonSerializerSettings DefaultSerializerSettings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        public static readonly Expression<Func<IList<DynamicFieldValue>, string>> DfvToJsonConversion =
            (IList<DynamicFieldValue> dfvs) => Json.SerializeObject(dfvs, DefaultSerializerSettings)
        ;
        public static readonly Expression<Func<string, IList<DynamicFieldValue>>> JsonToDfvConversion =
            (string json) => Json.DeserializeObject<IList<DynamicFieldValue>>(json, DefaultSerializerSettings)
        ;
        #endregion

        #region Parsing Methods
        protected object GetConvertedValue()
        {
            if (string.IsNullOrWhiteSpace(this.RawValue))
                return null;
            string serialized = this.RawValue;
            switch (this.DataType)
            {
                case DataType.B:
                    if (serialized == "0")
                        return false;
                    if (serialized == "1")
                        return true;
                    return Convert.ToBoolean(serialized);
                case DataType.D:
                    return Convert.ToDateTime(serialized);
                case DataType.L:
                    return Convert.ToInt32(serialized);
                case DataType.N:
                    return Convert.ToInt32(serialized);
                case DataType.S:
                    return serialized;
            }
            throw new Exception($"Invalid data type provided for dynamic value conversion: {this.DataType}");
        }

        protected string GetFormattedValue()
        {
            object converted = this.ConvertedValue;
            if (converted == null)
                return string.Empty;
            switch (this.DataType)
            {
                case DataType.B:
                    return ((bool)converted) ? "Yes" : "No";
                case DataType.D:
                    return ((DateTime)converted).ToString("MM/dd/yyyy");
                case DataType.L:
                    int id = (int)converted;
                    var option = this.Options.Single(x => x.ID == id);
                    return option.Description;
                case DataType.N:
                    return ((int)converted).ToString("N0");
                case DataType.S:
                    return (string)converted;
            }
            throw new Exception($"Invalid data type provided for dynamic value conversion: {this.DataType}");
        }

        public void ParseDataType(string rawDataType)
        {
            if (Enum.TryParse(rawDataType, out DataType result))
                this.DataType = result;
            else
                throw new Exception($"Invalid DFV data type: {rawDataType}");
        }
        #endregion

        #region Core Properties
        [JsonProperty("Name")]
        public string Name { get; set; }
        [JsonProperty("DataType")]
        public DataType DataType { get; set; }
        [JsonProperty("Value")]
        public string RawValue { get; set; }
        #endregion

        #region Typed Accessors
        [NotMapped]
        public object ConvertedValue => this.GetConvertedValue();
        [NotMapped]
        public string FormattedValue => this.GetFormattedValue();
        [NotMapped]
        public string StringValue => this.RawValue;
        [NotMapped]
        public int? IntValue => int.TryParse(this.RawValue, out int result) ? result : default(int?);
        [NotMapped]
        public bool? BoolValue => bool.TryParse(this.RawValue, out bool result) ? result : default(bool?);
        [NotMapped]
        public int? ListReferenceValue => int.TryParse(this.RawValue, out int result) ? result : default(int?);
        [NotMapped]
        public DateTime? DateValue => DateTime.TryParse(this.RawValue, out var result) ? result : default(DateTime?);
        #endregion

        [NotMapped]
        public List<LookupOption> Options { get; set; }

        public DynamicFieldValue()
        {
            this.DataType = DataType.S;
        }

        public DynamicFieldValue(int v)
        {
            this.DataType = DataType.N;
            this.RawValue = v.ToString();
        }

        public static implicit operator DynamicFieldValue(int value) => new DynamicFieldValue(value);

        public override string ToString() => $"{this.FormattedValue} [{this.DataType}]";
    }
}
