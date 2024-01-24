using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CarmaContext = GmWeb.Logic.Data.Context.Carma.CarmaContext;
using ActivityModel = GmWeb.Logic.Data.Models.Carma.Activity;
using CsvHelper.Configuration.Attributes;
using GmWeb.Logic.Utility.Csv;

namespace GmWeb.Logic.Services.Importing.Activities.Spreadsheets;

public class ActivityRecord : CsvRecord
{
    [Name("Activity Name")]
    public string Name { get; set; }
    [Name("Event Type")]
    public string EventType { get; set; }
    [Format("M/d/yyyy")]
    public DateTime Date { get; set; }
    [Name("Start Time")]
    [Format("hh:mm:ss tt")]
    public DateTime StartTime { get; set; }
    [Name("End Time")]
    [Format("hh:mm:ss tt")]
    public DateTime? EndTime { get; set; }
    [Name("Event Capacity")]
    public int? Capacity { get; set; }
    [Name("Venue/Room Name")]
    [NameIndex(0)]
    public string BuildingName { get; set; }
    [Name("Venue/Room Name")]
    [NameIndex(1)]
    public string RoomName { get; set; }
    [Name("Street Address")]
    public string StreetAddress { get; set; }
    [Name("City, State, Zip")]
    public string CityStateZip { get; set; }
    [Name("Description")]
    [NameIndex(0)]
    public string Activity { get; set; }
    [Name("Description")]
    [NameIndex(1)]
    public string SignUpRequired { get; set; }
    [Name("Description")]
    [NameIndex(2)]
    public string Provider { get; set; }
    [Name("Description")]
    [NameIndex(3)]
    public string DeliveryMethod { get; set; }

    [Ignore]
    public string Location =>
        $"{this.BuildingName} {this.RoomName}\n{this.StreetAddress}\n{this.CityStateZip}";
    [Ignore]
    public string Description => 
        $"Description: {this.Activity}\nSign up Required: {this.SignUpRequired}\nProvider: {this.Provider}\nDelivery Method: {this.DeliveryMethod}";
    public async Task<ActivityModel> LoadModelAsync(ActivityImportOptions options, CarmaContext context)
    {
        var activity = new ActivityModel 
        { 
            AgencyID = options.AgencyID,
            Name = this.Name,
            EventType = this.EventType,
            StartTime = this.Date.Add(this.StartTime.TimeOfDay),
            EndTime = this.Date.Add(this.EndTime?.TimeOfDay ?? new TimeSpan(23,59,59)),
            Capacity = this.Capacity,
            Location = this.Location,
            Description = this.Description
        };
        return await Task.FromResult(activity);
    }
}
