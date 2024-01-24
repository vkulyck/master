using GmWeb.Logic.Data.Context;
using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Interfaces;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace GmWeb.Logic.Importing.Demographics
{
    public class DemographicImporterFactory
    {
        protected class Patterns
        {
            public static readonly string ID = @"[A-Za-z\d]+";
            public static readonly string N = @"\d+";

            public static readonly string Survey = $@"(?<survey>{ID})";
            public static readonly string Year = $@"(?<year>{N})";
            public static readonly string Period = $@"(?<period>{ID})";
            public static readonly string Product = $@"(?<product>{ID})";
            public static readonly string Suffix = $@"(?<suffix>with_ann|metadata)";
            public static readonly string Extension = $@"(?<extension>(?:csv|txt))";
            public static readonly string FilenamePattern = $@"(?<dataset>{Survey}_{Year}_{Period}_{Product})(_{Suffix})?\.{Extension}";
        }
        protected static readonly Regex FilenameParser = new Regex(Patterns.FilenamePattern);

        public static IDataImporter CreateImporter(IDemographicsContext cache, Category category)
        {
            string importerName = $"{category.Identifier}Importer";
            var importerType = //typeof(IDataImporter).Assembly.GetTypes()
                AppDomain.CurrentDomain.GetAssemblies()
                .Where(x => x.GetName().Name.StartsWith("GmWeb"))
                .SelectMany(x => x.GetTypes())
                .Where(x => typeof(IDataImporter).IsAssignableFrom(x))
                .SingleOrDefault(x => x.Name == importerName)
            ;
            if (importerType == null)
                return null;
            var importer = (IDataImporter)Activator.CreateInstance(importerType, cache, category);
            return importer;
        }
    }
}
