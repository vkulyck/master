using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Text.RegularExpressions;
using Path = System.IO.Path;
using SysFile = System.IO.File;

namespace GmWeb.Logic.Data.Models.Demographics
{
    /// <summary>
    /// A dataset corresponding to a single CSV file provided by US Census affiliates.
    /// </summary>
    [Table("tblDatasets", Schema = "dmg")]
    public class Dataset : BaseDataModel
    {
        #region File Access
        private static readonly string ConfigKey = $"{typeof(Dataset).Namespace}.DatasetDirectory";
        public static readonly string Directory = ConfigurationManager.AppSettings[ConfigKey]?.ToString() ?? string.Empty;
        public string SourcePath => Path.Combine(Directory, $"{this.Name}_with_ann.csv");
        public string MetadataPath => Path.Combine(Directory, $"{this.Name}_metadata.csv");
        public bool HasValidSourcePath => SysFile.Exists(this.SourcePath);
        public bool HasValidMetadataPath => SysFile.Exists(this.MetadataPath);
        #endregion

        [Key]
        public int DatasetID { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        [InverseProperty("Dataset")]
        public virtual IList<Category> Categories { get; set; } = new List<Category>();

        public string ForeignID => Regex.Match(this.Name, @"(?<=_)[A-Za-z\d]+$").Value;

        public override string ToString() => $"{this.DatasetID}: {this.Name}";
    }
}
