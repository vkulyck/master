using System;
using DynamicDataType = GmWeb.Logic.Enums.DataType;
namespace GmWeb.Logic.Interfaces
{
    public interface IDynamicValueType
    {
        /// <summary>
        /// The raw value stored in the database in string format
        /// </summary>
        string RawValue { get; set; }

        /// <summary>
        /// The dynamically accessible datatype describing the raw value: 
        ///     N (integer)
        ///     B (bit)
        ///     D (date)
        ///     L (list item reference)
        /// </summary>
        int? IntValue { get; }
        bool? BoolValue { get; }
        int? ListReferenceValue { get; }
        DateTime? DateValue { get; }
        DynamicDataType DataType { get; }

        void ParseDataType(string rawDataType);
    }
}
