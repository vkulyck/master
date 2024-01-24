using System;
using Kendo.Mvc.UI;
using Kendo.Mvc.UI.Fluent;
using Microsoft.AspNetCore.Mvc.Rendering;
using GmWeb.Web.Scheduler.Models;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GmWeb.Web.Scheduler.Wrappers
{

    public static class GoodMojoScheduler
    {
        public enum SchedulerParameters { ImportBtnCalID = 0
                                        , ExportBtnCalID
                                        , ImportEventBtnID
                                        , SchedulerWidgetID
                                        , SchedulerControllerID
                                        , SecondSchedulerWidgetID
                                        , SecondSchedulerWidgetImportBtnCalID
                                        , SecondSchedulerWidgetExportBtnCalID
                                        , SecondSchedulerWidgetImportEventBtnCalID
        };
        public static readonly Dictionary<SchedulerParameters, String> options = new();

        static GoodMojoScheduler()
        {

            options.Add(SchedulerParameters.ImportBtnCalID, "importUpload");
            options.Add(SchedulerParameters.ExportBtnCalID, "exportButton");
            options.Add(SchedulerParameters.ImportEventBtnID, "importEvent");
            options.Add(SchedulerParameters.SchedulerWidgetID, "scheduler");
            options.Add(SchedulerParameters.SchedulerControllerID, "Scheduler");
            options.Add(SchedulerParameters.SecondSchedulerWidgetID, "scheduler_ImportEvents");

            options.Add(SchedulerParameters.SecondSchedulerWidgetImportBtnCalID, "importUploadSecond");
            options.Add(SchedulerParameters.SecondSchedulerWidgetExportBtnCalID, "exportButtonSecond");
            options.Add(SchedulerParameters.SecondSchedulerWidgetImportEventBtnCalID, "importEventSecond");

        }
        public static string goodmogoSchedulersExtraCtrls(SchedulerParameters importBtnID, SchedulerParameters exportBtnID, SchedulerParameters impEvBtnID, bool bCreatePopupDlg = true) 
        {
            string sourceImpEventDlg = "<ul id=\"contextMenu\"></ul>" + Environment.NewLine + "<ul id=\"contextMenuSecond\"></ul>" + Environment.NewLine;
            if (bCreatePopupDlg)
            {
                string jsonValuesForJS = JsonConvert.SerializeObject(options);
                sourceImpEventDlg += "<div> <input type=\"hidden\" id=\"ctrlForExchValues\" value='"+jsonValuesForJS+"'> </div>" + Environment.NewLine;
                sourceImpEventDlg += "<div id = \"importNewEventsDialog\">" + Environment.NewLine +
               "<em>*closing the window will cancel the changes</em>" + Environment.NewLine +
               "<div id=\"ImportedEvents\"></div>" + Environment.NewLine +
               "<script>" + Environment.NewLine +
               "$(document).ready(function() {" + Environment.NewLine +
               "$(\"#ImportedEvents\").kendoGrid({" + Environment.NewLine +
                   "scrollable: true," + Environment.NewLine +
                   "resizable: false," + Environment.NewLine +
                   "autoBind: false," + Environment.NewLine +
                   "persistSelection: true," + Environment.NewLine +
                   "dataSource: " + Environment.NewLine +
                       "{" + Environment.NewLine +
                       "transport:" + Environment.NewLine +
                           "{" + Environment.NewLine +
                           "read:" + Environment.NewLine +
                            "   {" + Environment.NewLine +
                             "  url: \"/Scheduler/ReadImportedEvents\"," + Environment.NewLine +
                              " type: \"get\"," + Environment.NewLine +
                              " dataType: \"json\"" + Environment.NewLine +
                           "}" + Environment.NewLine +
                           "}," + Environment.NewLine +
                       "schema:" + Environment.NewLine +
                           "{" + Environment.NewLine +
                           "data: \"Data\"," + Environment.NewLine +
                           "total: \"Total\"," + Environment.NewLine +
                           "errors: \"Errors\"," + Environment.NewLine +
                           "model:" + Environment.NewLine +
                               "{" + Environment.NewLine +
                               "id: \"ID\"," + Environment.NewLine +
                               "fields:" + Environment.NewLine +
                                   "{" + Environment.NewLine +
                                   "Attendees: { type: \"Array\" }," + Environment.NewLine +
                                   "Description: { type: \"string\" }," + Environment.NewLine + 
                                   "End: { type: \"string\" }," + Environment.NewLine +
                                   "EndTimezone: { type: \"string\" }," + Environment.NewLine +
                                   "ID: { type: \"number\" }," + Environment.NewLine +
                                   "IsAllDay: { type: \"Boolean\" }," + Environment.NewLine +
                                   "IsEvent: { type: \"Boolean\" }," + Environment.NewLine +
                                   "IsJournal: { type: \"Boolean\" }," + Environment.NewLine +
                                   "IsTodo: { type: \"Boolean\" }," + Environment.NewLine +
                                   "Modified: { type: \"Boolean\" }," + Environment.NewLine +
                                   "OwnerID: { type: \"number\" }," + Environment.NewLine +
                                   "RecurrenceException: { type: \"string\" }," + Environment.NewLine +
                                   "RecurrenceID: { type: \"number\" }," + Environment.NewLine +
                                   "RecurrenceRule: { type: \"string\" }," + Environment.NewLine +
                                   "RoomID: { type: \"number\" }," + Environment.NewLine +
                                   "Start: { type: \"string\" }," + Environment.NewLine +
                                   "StartTimezone: { type: \"string\" }," + Environment.NewLine +
                                   "Title: { type: \"string\" }," + Environment.NewLine +
                                   "UID: { type: \"string\" }" + Environment.NewLine +
                                   "}" + Environment.NewLine +
                               "}" + Environment.NewLine +
                           "}," + Environment.NewLine +
                   "}," + Environment.NewLine +
                   "columns:" + Environment.NewLine +
                       "[" + Environment.NewLine +
                       "{ selectable: true, width: \"50px\" }," + Environment.NewLine +
                       "{ field: \"Title\" }," + Environment.NewLine +
                       "{ field: \"Start\" }," + Environment.NewLine +
                       "{ field: \"End\" }," + Environment.NewLine +
                       "{ field: \"Description\" }]" + Environment.NewLine +
                   "});" + Environment.NewLine +
                   "});" + Environment.NewLine +
               "</script>" + Environment.NewLine +
               "<br/>" + Environment.NewLine +
               "<button id = 'Cancel' class=\"k-button\">Cancel</button>" + Environment.NewLine +
               "<button id = 'Import' class=\"k-button\">Import</button>" + Environment.NewLine +
               "</div>";
            };

            string sourceBtnForScheduler = "<div class=\"k-widget k-upload k-import-upload\">" + Environment.NewLine +
                "<div class=\"k-button k-button-icontext k-upload-button\">" + Environment.NewLine +
                "<span class=\"k-icon k-i-calendar\"></span>Import calendar<input id=\"{0}\" type=\"file\" name=\"file\" />" + Environment.NewLine +
                "</div>" + Environment.NewLine +
                "<button class=\"k-button k-button-icontext\" id=\"{1}\">" + Environment.NewLine +
                "<span class='k-icon k-i-save'></span>Export calendar" + Environment.NewLine +
                "</button>" + Environment.NewLine +
                "<div class=\"k-button k-button-icontext k-upload-button\">" + Environment.NewLine +
                "<span class=\"k-icon k-i-calendar\"></span>Import events<input id=\"{2}\" type=\"file\" name=\"file\" />" + Environment.NewLine +
                "</div>" + Environment.NewLine +
                "</div>";

            sourceBtnForScheduler = String.Format(sourceBtnForScheduler, options[importBtnID], options[exportBtnID], options[impEvBtnID]);

            return sourceImpEventDlg + Environment.NewLine + sourceBtnForScheduler;
        }
        public static SchedulerBuilder<T> gmScheduler<T>(this IHtmlHelper<dynamic> helper, SchedulerParameters name, SchedulerParameters controllerName) where T :  TaskViewModel
        {
            return helper.Kendo().Scheduler<T>()
            .Name(options[name])
            .CurrentTimeMarker(true)
            .Date(DateTime.Now)
            .StartTime(new DateTime(2018, 01, 01, 00, 00, 01))
            .Height(750)
            .Views(views =>
            {
                views.DayView();
                views.WeekView();
                views.WorkWeekView();
                views.MonthView(m => m.Selected(true));
                views.AgendaView();
                views.TimelineView();
            })
            .Timezone("Etc/UTC")
            .Editable(editable =>
            {
                editable.TemplateName(options[name]);
            })
            .Events(e => e.Edit("onEdit"))
            //.Events(e => e.Add("onAdd"))
            .Resources(resource =>
            {
                resource.Add(m => m.Attendees)
                    .Title("Attendee")
                    .Multiple(true)
                    .DataTextField("Text")
                    .DataValueField("Value")
                    .DataColorField("Color")
                    .DataSource(ds => ds
                            .Custom()
                            .Type("aspnetmvc-ajax")
                            .Transport(transport => transport.Read(read => read.Action("Read_Attendees", options[controllerName])))
                            .Schema(schema => schema
                                .Data("Data")
                                .Total("Total")
                                .Errors("Errors")
                                .Model(model =>
                                {
                                    model.Id("Value");
                                    model.Field("Value", typeof(int));
                                    model.Field("Text", typeof(string));
                                    model.Field("Name", typeof(string));
                                    model.Field("Color", typeof(string));
                                    model.Field("Mail", typeof(string));
                                })
                            )
                        );
            })
            .DataSource(d => d
                .Model(m =>
                {
                    m.Id(f => f.ID);
                    m.Field(f => f.Title).DefaultValue("No title");
                    m.RecurrenceId(f => f.RecurrenceID);
                    m.Field(f => f.Description).DefaultValue("");
                })
                .Read("Read", options[controllerName])
                .Create("Create", options[controllerName])
                .Destroy("Destroy", options[controllerName])
                .Update("Update", options[controllerName])
            );
        }
    }
}