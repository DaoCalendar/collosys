using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using ColloSys.DataLayer.NhSetup;
using NHibernate.Cfg;

namespace ColloSys.DataLayer.Infra.NhSetup
{
    internal class ConfigurationBuilder
    {
        //config path
        private const string SerializedCfg = @"configuration.bin";

        public Configuration Build(ConfiguredDbTypes dbtype, string connectionString, bool isWeb)
        {
            // TODO : uncomment when solve 
            //var cfg = LoadConfigurationFromFile();
            //if (cfg == null)
            //{
               var cfg = SessionFactoryManager.Build(dbtype, connectionString, isWeb);
            //    SaveConfigurationToFile(cfg);
            //}
            return cfg;
        }

        #region load from file

        private Configuration LoadConfigurationFromFile()
        {
            if (!IsConfigurationFileValid())
                return null;
            try
            {
                using (var file = File.Open(SerializedCfg, FileMode.Open))
                {
                    var bf = new BinaryFormatter();
                    return bf.Deserialize(file) as Configuration;
                }
            }
            catch (Exception)
            {
                // Something went wrong
                // Just build a new one
                return null;
            }
        }

        private bool IsConfigurationFileValid()
        {
            // If we don't have a cached config, 
            // force a new one to be built
            if (!File.Exists(SerializedCfg))
                return false;

            var configInfo = new FileInfo(SerializedCfg);

            var asm = Assembly.GetExecutingAssembly();

            // If the assembly is newer, 
            // the serialized config is stale
            var asmInfo = new FileInfo(asm.Location);
            if (asmInfo.LastWriteTime > configInfo.LastWriteTime)
                return false;

            // If the app.config is newer, 
            // the serialized config is stale
            var appDomain = AppDomain.CurrentDomain;
            var appConfigPath = appDomain.SetupInformation.ConfigurationFile;
            var appConfigInfo = new FileInfo(appConfigPath);
            //if (appConfigInfo.LastWriteTime > configInfo.LastWriteTime)
            //    return false;

            return appConfigInfo.LastWriteTime <= configInfo.LastWriteTime;

            // It's still fresh
        }

        #endregion

        #region save to file

        private void SaveConfigurationToFile(Configuration cfg)
        {
            using (var file = File.Open(SerializedCfg, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(file, cfg);
            }
        }

        #endregion
    }
}