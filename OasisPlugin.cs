using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using I18N.West;

namespace OasisVault
{
    public class OasisPlugin : RocketPlugin<OasisConfiguration>
    {
        public static string version = "1.0.0.3";
        public static OasisPlugin Instance;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "vault_not_enabled", "[Oasis] Vault is not enabled." },
                    { "vault_cooldown", "[Oasis] You can use Vault again in {0} seconds." },
                    { "vault_no_permissions", "[Oasis] You do not have permission for this command." },
                    { "vault_opened", "[Oasis] Vault has been opened." }
                };
            }
        }

        protected override void Load()
        {
            OasisPlugin.Instance = this;

            Logger.Log("OasisVault, Version " + OasisPlugin.version);
            Logger.Log("Checking errors... ");
            if (Configuration.Instance.Enabled)
            {
                CP1250 cP1250 = new CP1250();
                Database.CheckSchema();
            }
            Logger.Log("No errors have been found!");
        }

        protected override void Unload()
        {
        }
    }
}

