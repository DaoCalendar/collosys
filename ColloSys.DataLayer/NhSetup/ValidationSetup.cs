#region references

using System.Reflection;
using NHibernate.Cfg;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Cfg.Loquacious;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Event;
using Env = NHibernate.Validator.Cfg.Environment;

#endregion

namespace ColloSys.DataLayer.NhSetup
{
    internal static class ValidationSetup
    {
        public static void Setup(Configuration cfg)
        {
            // init validation engine
            Env.SharedEngineProvider = new NHibernateSharedEngineProvider();
            var validatorEngine = Env.SharedEngineProvider.GetEngine();

            // create fluent instance for configuring parameters    
            var nhvConfiguration = new FluentConfiguration();

            // use only validation def, DONT use Nhibernate attributes
            nhvConfiguration.SetDefaultValidatorMode(ValidatorMode.OverrideExternalWithAttribute);

            // use the validations also during creation of db
            nhvConfiguration.Register(Assembly.GetExecutingAssembly().ValidationDefinitions())
                            .IntegrateWithNHibernate // use in nhibernate
                            .ApplyingDDLConstraints() // use while creating schema
                            .RegisteringListeners(); // also call pre-insert/update etc listeners

            // configure the validation engine using above settings
            validatorEngine.Configure(nhvConfiguration);

            // init engine on global level
            cfg.Initialize(validatorEngine);
        }
    }
}