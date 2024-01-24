using Ical.Net;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using iCalSerializer = Ical.Net.Serialization.CalendarSerializer;

namespace GmWeb.Logic.Data.Models.Carma
{
    public class AgencyCalendarFile : CalendarFile
    {
        [NotMapped]
        public int AgencyID
        {
            get
            {
                string fn = System.IO.Path.GetFileNameWithoutExtension(this.Name);
                string[] assignments = fn.Split(';');
                foreach (string assignment in assignments)
                {
                    string[] pieces = assignment.Split('=');
                    if (pieces[0].ToLower() != "agencyid")
                        continue;
                    if (int.TryParse(pieces[1], out int result))
                        return result;
                }
                throw new ArgumentException($"Unable to parse AgencyID from calendar file '{this.Name}'");
            }
        }

        public Calendar Deserialize()
        {
            string content = Encoding.ASCII.GetString(this.FileStream);
            var deserialized = Calendar.Load(content);
            return deserialized;
        }

        public void Serialize(Calendar calendar)
        {
            var serializer = new iCalSerializer();
            string serialized = serializer.SerializeToString(calendar);
            byte[] data = Encoding.ASCII.GetBytes(serialized);
            this.FileStream = data;
        }
    }
}
