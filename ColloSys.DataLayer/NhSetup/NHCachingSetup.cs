using NHibernate.Caches.SysCache;
using NHibernate.Cfg;

namespace ColloSys.DataLayer.NhSetup
{
    internal class NhCachingSetup
    {
        public static void Setup(Configuration cfg)
        {
            // use both L1 & L2 cache
            cfg.SetProperty(Environment.BatchSize, "100")
               .SetProperty(Environment.UseQueryCache, "true")
               .SetProperty(Environment.UseSecondLevelCache, "true")
               .Cache(x => x.Provider<SysCacheProvider>());

            // set 60 min expiration time
            cfg.SessionFactory().Caching.WithDefaultExpiration(60);
        }
    }
}