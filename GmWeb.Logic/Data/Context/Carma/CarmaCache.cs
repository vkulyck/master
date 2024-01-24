namespace GmWeb.Logic.Data.Context.Carma
{
    public partial class CarmaCache : BaseDataCache<CarmaCache, CarmaContext>
    {
        private CalendarFiles _CalendarFiles;
        public CalendarFiles CalendarFiles => this._CalendarFiles ?? (this._CalendarFiles = new CalendarFiles(this));
    }
}
