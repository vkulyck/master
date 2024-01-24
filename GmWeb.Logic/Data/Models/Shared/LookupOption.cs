namespace GmWeb.Logic.Data.Models.Shared
{
    public class LookupOption : BaseDataModel
    {
        public virtual int ID { get; set; }
        public string Description { get; set; }

        public override string ToString() => $"LKP {this.ID} = {this.Description}";
    }
}
