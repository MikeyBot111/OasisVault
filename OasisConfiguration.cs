using Rocket.API;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;
using System;

namespace OasisVault
{
    public class OasisConfiguration : IRocketPluginConfiguration
    {
        public string DatabaseAddress;
        public string DatabaseUsername;
        public string DatabasePassword;
        public string DatabaseName;
        public string DatabaseTableName;
        public int DatabasePort;
        public bool Enabled;
        public byte VaultWidth;
        public byte VaultHeight;
        public int MaxCooldown;

        public void LoadDefaults()
        {
            DatabaseAddress = "localhost";
            DatabaseUsername = "root";
            DatabasePassword = "";
            DatabaseName = "unturned";
            DatabaseTableName = "OasisVault";
            DatabasePort = 3306;
            Enabled = true;
            VaultWidth = 8;
            VaultHeight = 8;
            MaxCooldown = 8;
        }
    }

    public class VaultPlayerComponent : UnturnedPlayerComponent
    {
        public DateTime LastUsed = DateTime.MinValue;
        public List<Item> OldItems = new List<Item>();
        public Items VaultItems;
    }
}
