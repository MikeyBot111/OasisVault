using Rocket.API.Collections;
using Rocket.Core.Logging;
using Rocket.Core.Plugins;
using I18N.West;

namespace OasisVault
{
    public class OasisPlugin : RocketPlugin<OasisConfiguration>
    {
        public static string version = "1.0.0.2";
        public static OasisPlugin Instance;

        public override TranslationList DefaultTranslations
        {
            get
            {
                return new TranslationList
                {
                    { "vault_not_enabled", "[Oasis] Vault není aktivovaný." },
                    { "vault_cooldown", "[Oasis] Vault můžeš použít zase za {0} sekundy." },
                    { "vault_no_permissions", "[Oasis] Nemáš oprávnění na tento příkaz." },
                    { "vault_opened", "[Oasis] Vault byl otevřen." }
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

