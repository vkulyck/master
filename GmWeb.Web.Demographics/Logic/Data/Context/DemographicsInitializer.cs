using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using GmWeb.Logic.Data.Models.Demographics;
using GmWeb.Logic.Importing.Demographics;
using GmWeb.Logic.Utility.Extensions;
using GmWeb.Logic.Enums;
using Category = GmWeb.Logic.Data.Models.Demographics.Category;
using System.Configuration;
using EFCore.BulkExtensions;
using GmWeb.Web.Demographics.Logic.Data.Context;

namespace GmWeb.Logic.Data.Context.DataInitializers
{
    public class DemographicInitializer : BaseDataInitializer<DemographicsCache,DemographicsContext>
    {
        private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void Seed(DemographicsCache cache) => new DemographicInitializer(cache).Seed();
        protected static List<string> TractIds { get; } = new List<string>();
        protected static Random _random = new Random();
        protected override string ConcreteConfigKeySuffix => nameof(DemographicInitializer);

        static DemographicInitializer()
        {
            foreach (var tractId in ResourceExtensions.IterateEmbeddedResourceLines(@"Data\Context\DataInitializers\TexasTractIds.txt"))
            {
                var id = tractId.Trim();
                TractIds.Add(id);
            }
        }

        public DemographicInitializer(DemographicsCache cache) : base(cache) { }

        public override void OnSeed()
        {
            if(!AppendNewData)
                this.ClearEntities();

            var datasets = this.Cache.Datasets.ToList();
            var categories = this.Cache.Categories.ToList();
            if (!AppendNewData || datasets.Count == 0 || categories.Count == 0)
            {
                this.ClearEntities();
                datasets = this.CreateDatasets(Dataset.Directory);
                this.Cache.Datasets.AddRange(datasets);
                this.Cache.Save();
                categories = this.CreateCategories(datasets);
                this.Cache.Categories.AddRange(categories);
                this.Cache.Save();
            }
            else
            {
                var newDatasets = this.CreateDatasets(Dataset.Directory);
                var newCategories = this.CreateCategories(newDatasets);
                if(newDatasets.Count != datasets.Count || newCategories.Count != categories.Count)
                {
                    this.ClearEntities();
                    this.Cache.Datasets.AddRange(newDatasets);
                    this.Cache.Save();
                    this.Cache.Categories.AddRange(newCategories);
                    this.Cache.Save();
                    categories = newCategories;
                }
            }
            this.ImportCategoryData(categories);
        }

        public override void ClearEntities()
        {
            this.Cache.DataContext.BinValues.BatchDelete();
            this.Cache.DataContext.Bins.BatchDelete();
            this.Cache.DataContext.CategoryValues.BatchDelete();
            this.Cache.DataContext.Categories.BatchDelete();
            this.Cache.DataContext.Datasets.BatchDelete();
        }

        protected List<Dataset> CreateDatasets(string datafileDir)
        {
            var datasets = new List<Dataset>
            {
                new Dataset{ Date = new DateTime(2017,01,01), Name = "ACS_17_5YR_B19001" },
                new Dataset{ Date = new DateTime(2017,01,01), Name = "ACS_17_5YR_S1901" },
                new Dataset{ Date = new DateTime(2017,01,01), Name = "ACS_17_5YR_S1902" },
                new Dataset{ Date = new DateTime(2017,01,01), Name = "ACS_17_5YR_S1903" },
                new Dataset{ Date = new DateTime(2017,01,01), Name = "ACS_17_5YR_B01003" },
                new Dataset{ Date = new DateTime(2017,01,01), Name = "DEC_10_PL_P1" },
            };
            return datasets;
        }

        protected List<Category> CreateCategories(List<Dataset> datasets)
        {
            Category createCategory(Dataset dataset)
            {
                Category category;
                switch (dataset.ForeignID)
                {
                    case "B19001":
                        category = new Category
                        {
                            MetricType = MetricType.HouseholdCount,
                            Name = "Household Income"
                        }; break;
                    case "S1901":
                        category = new Category
                        {
                            MetricType = MetricType.HouseholdCount,
                            Name = "Household Makeup"
                        }; break;
                    case "S1902":
                        category = new Category
                        {
                            MetricType = MetricType.MeanValue,
                            Name = "Mean Income"
                        }; break;
                    case "S1903":
                        category = new Category
                        {
                            MetricType = MetricType.MedianValue,
                            Name = "Median Income"
                        }; break;                    
                    case "B01003":
                        category = new Category
                        {
                            MetricType = MetricType.SumValue,
                            Name = "Total Population"
                        }; break;                    
                    case "P1":
                        category = new Category
                        {
                            MetricType = MetricType.SumValue,
                            Name = "Racial Density"
                        }; break;
                    default:
                        throw new Exception($"No category available for dataset '{dataset.Name}");
                }
                category.Dataset = dataset;
                category.DatasetID = dataset.DatasetID;
                return category;
            }
            var categories = new List<Category>();
            foreach(var dataset in datasets)
            {
                categories.Add(createCategory(dataset));
            }
            return categories;
        }

        protected void ImportCategoryData(List<Category> categories)
        {
            foreach (var category in categories)
            {
                var dataset = category.Dataset;
                if (!dataset.HasValidSourcePath)
                    throw new Exception($"No source file found for dataset: {dataset.SourcePath}");
                var importer = DemographicImporterFactory.CreateImporter(this.Cache.DataContext, category);
                if (importer == null)
                {
                    _logger.Warn($"Importer missing for category: {category.Name}");
                    continue;
                }
                importer.Import();
            }
        }

        private void seedTestValues(List<Category> categories)
        {
            foreach (var category in categories)
            {
                var bins = new List<Bin> {
                    CreateBin(category, 1),
                    CreateBin(category, 2),
                    CreateBin(category, 3),
                };
                this.Cache.Bins.AddRange(bins);
                this.Cache.Save();

                var catValues = this.CreateCategoryValue(category);
                this.Cache.CategoryValues.AddRange(catValues);
                this.Cache.Save();

                foreach (var bin in bins)
                {
                    var binValues = this.CreateBinValues(bin);
                    this.Cache.BinValues.AddRange(binValues);
                    this.Cache.Save();
                }
            }
        }

        protected Bin CreateBin(Category category, int sequence)
        {
            var bin = new Bin(category);
            bin.Identifier = $"SampleBin{category.Name}{sequence:D5}";
            bin.Description = $"{category.Name} bin with sequence number {sequence:N0}";
            return bin;
        }

        protected List<BinValue> CreateBinValues(Bin bin)
        {
            var binValues = new List<BinValue>();
            int
                count = 10,
                index = _random.Next() % (TractIds.Count - count)
            ;
            foreach (var geoId in TractIds.Skip(index).Take(count))
            {
                decimal value;
                if (bin.Category.MetricType == MetricType.HouseholdCount)
                    value = _random.Next() % ((int)1E6);
                else
                    value = (decimal)(_random.NextDouble() * 1E6);
                var binValue = new BinValue
                {
                    Bin = bin,
                    BinID = bin.BinID,
                    TractID = geoId.Substring(9),
                    Value = value
                };
                binValues.Add(binValue);
            }
            return binValues;
        }

        protected List<CategoryValue> CreateCategoryValue(Category category)
        {
            var catValues = new List<CategoryValue>();
            int 
                count = 10,
                index = _random.Next() % (TractIds.Count - count)
            ;
            foreach (var geoId in TractIds.Skip(index).Take(count))
            {
                decimal value;
                if(category.MetricType == MetricType.HouseholdCount)
                    value = _random.Next() % ((int)1E6);
                else
                    value = (decimal)(_random.NextDouble() * 1E6);
                var catValue = new CategoryValue
                {
                    Category = category,
                    CategoryID = category.CategoryID,
                    TractID = geoId.Substring(9),
                    Value = value
                };
                catValues.Add(catValue);
            }
            return catValues;
        }
    }
}
