using System.Configuration;

namespace GmWeb.Logic.Data.Context
{
    public abstract class BaseDataInitializer
    {
        protected static readonly string BaseConfigKey = $"{typeof(BaseDataInitializer).Namespace}";
        protected static readonly string AppendKey = $"{BaseConfigKey}.AppendNewData";
        public static readonly bool AppendNewData =
            bool.TryParse(ConfigurationManager.AppSettings[AppendKey]?.ToString(), out bool result) ? result
            : true
        ;
        public static readonly bool ClearExistingData = !AppendNewData;

        protected abstract string ConcreteConfigKeySuffix { get; }
        protected string EnableInitializerKey => $"{BaseConfigKey}.{this.ConcreteConfigKeySuffix}.EnableInitializer";
        public bool EnableInitializer =>
            bool.TryParse(ConfigurationManager.AppSettings[this.EnableInitializerKey]?.ToString(), out bool result) ? result
            : true
        ;
        public void Seed()
        {
            if (!this.EnableInitializer)
                return;
            this.OnSeed();
        }
        public abstract void OnSeed();
        public abstract void ClearEntities();
    }
    public abstract class BaseDataInitializer<TCache, TContext> : BaseDataInitializer
        where TCache : BaseDataCache<TCache, TContext>, new()
        where TContext : BaseDataContext<TContext>, new()
    {
        public TCache Cache { get; private set; }

        public BaseDataInitializer(TCache cache)
        {
            this.Cache = cache;
        }
    }
}
