using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;

namespace GmWeb.Web.Profile.Models.ClientProfile
{
    public class SupplementalGridRow
    {
        public static readonly Dictionary<string, string> ProfileToFundingValueMap = new Dictionary<string, string>
        {
            { "Household Income level", "TotalHouseholdIncome" },
            { "Size of Household", "NumberInHousehold" },
            { "Family Income Level", "TotalFamilyIncome" },
            { "Family Size", "NumberInFamily" },
            { "Residence", "Residence" },
            { "Age", "ClientAge" },
        };
        public static readonly Dictionary<string, string> ProfileToFundingCheckedMap = new Dictionary<string, string>
        {
            { "HIV Status", "AIDS" },
            { "Currently Employed", "CurrentEmployment" },
            { "Disabled", "SevereDisabilities" },
        };
        public static readonly Dictionary<string, string> ProfileToFundingSelectionMap = new Dictionary<string, string>
        {
            { "Head of Household", "HeadofHouseholdType" },
            { "Employment Type", "EmploymentTypeID" },
            { "Education level", "TargetPopulationEducationLevelID" },
            { "Language Proficiency", "LanguageProficiencyID" },
            { "Language Primary", "LanguageIDPrimary" },
            { "Language Secondary", "LanguageIDSecondary" },
            { "EEO Industry Code", "EEOIndustryCode" },
            { "Status", "Status" },
        };

        public string Name { get; set; }
        public string DataType { get; set; }
        public int? ClientID { get; set; }
        public string Operation { get; set; }
        public string FundingValue { get; set; }
        public bool? FundingBitValue { get; set; }
        public string FundingNumericValue { get; set; }
        public string FundingListSelectionValue { get; set; }

        public SupplementalGridRowListColumnSettings ListSettings { get; set; }

        /// <summary>
        /// Returns a default string or numeric value for each of the profile names if there is one; if not, it returns an empty string.
        /// </summary>
        /// <param name="name">The profile name.</param>
        /// <param name="rowClient">This client's client data.</param>
        /// <param name="rowAssociation">This client's association data.</param>
        /// <returns>Default value if it exists; else an empty string.</returns>
        public void SetFundingNumeric(DataRow rowClient, DataRow rowAssociation)
        {
            if (this.DataType == "L" || this.DataType == "B")
                return;
            
            if(ProfileToFundingCheckedMap.TryGetValue(this.Name, out string field))
            {
                this.FundingNumericValue = rowClient.ToString(field);
            }
            else
            {
                var associationColumn = this.Name.Replace(" ", "");
                this.FundingNumericValue = rowAssociation?.ToString(associationColumn, AllowMissing: true) ?? string.Empty;
            }

            // When a numeric value ends with .0000, change it to .00.
            if (this.FundingNumericValue.EndsWith(".0000"))
                this.FundingNumericValue = this.FundingNumericValue.Substring(0, this.FundingNumericValue.Length - 2);
            this.FundingValue = this.FundingNumericValue?.ToString(); // TODO: Consolidate value conversions
        }

        /// <summary>
        /// Returns a default boolean value for each of the profile names if there is one; if not, it returns False.
        /// </summary>
        /// <param name="name">The profile name.</param>
        /// <param name="rowClient">This client's client data.</param>
        /// <param name="rowAssociation">This client's association data.</param>
        /// <returns>Default value if it exists; else False.</returns>
        public void SetFundingBit(DataRow rowClient, DataRow rowAssociation)
        {
            if (this.DataType != "B")
                return;

            if (ProfileToFundingCheckedMap.TryGetValue(this.Name, out string field))
            {
                this.FundingBitValue = rowClient.ToBoolean(field);
            }
            else
            {
                var associationColumn = this.Name.Replace(" ", "");
                this.FundingBitValue = rowAssociation?.ToBoolean(associationColumn, AllowMissing: true) ?? false;
            }
            this.FundingValue = this.FundingBitValue?.ToString();
            this.FundingValue = this.FundingNumericValue?.ToString(); // TODO: Consolidate value conversions
        }

        public void SetFundingListSelection(DataRow rowClient, DataRow rowAssociation)
        {
            if (this.DataType != "L")
                return;
        }

        public void SetOperator()
        {
            if (this.DataType == "N")
                this.Operation = "<=";
            else if (this.DataType == "S" || this.DataType == "D")
                this.Operation = "=";
        }
    }
}