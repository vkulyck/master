namespace GmWeb.Logic.Data.Context.Profile
{
    public partial class ProfileCache : BaseDataCache<ProfileCache, ProfileContext>
    {
        public override void Initialize() => WaitlistInitializer.Seed(this);
    }
}
