using Rocket.API;
using Rocket.Core.Logging;
using Rocket.Core.Utils;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;


namespace OasisVault
{
    public class OasisCommand : IRocketCommand
    {
        #region Declarations
        private Dictionary<CSteamID, DateTime> Cooldown = new Dictionary<CSteamID, DateTime>();
        public List<string> Permissions => new List<String>() { "OasisVault.vault" };
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "vault";
        public string Syntax => "";
        public string Help => "Příkaz pro uložení itemů ve vaultu.";
        public List<string> Aliases => new List<string>();
        #endregion

        public void Execute(IRocketPlayer caller, string[] command)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (!OasisPlugin.Instance.Configuration.Instance.Enabled)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("vault_not_enabled"), UnityEngine.Color.red);
                return;
            }
            if (!player.HasPermission("vault") && !player.IsAdmin)
            {
                UnturnedChat.Say(player, OasisPlugin.Instance.Translate("vault_no_permissions"), UnityEngine.Color.red);
                return;
            }
            Items inventory = new Items(7);

            if (command.Length != 0 && command[0].ToString() == "spy" && player.IsAdmin)
            {
                if (command.Length != 2)
                {
                    return;
                }

                CSteamID CSID = (CSteamID)(ulong)Int64.Parse(command[1].ToString());

                if (CSID != null) 
                {
                    byte sizeN = OasisPlugin.Instance.Configuration.Instance.VaultHeight;
                    inventory.resize(OasisPlugin.Instance.Configuration.Instance.VaultWidth, sizeN);
                    List<ItemJar> itemsSpy = Database.RetrieveItems(CSID);
                    foreach (ItemJar item in itemsSpy)
                    {
                        ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, item.item.id);
                        bool failed = false;
                        try
                        {

                            inventory.addItem(item.x, item.y, item.rot, item.item);

                        }
                        catch (Exception err)
                        {
                            Logger.Log("Err : " + err);
                            failed = true;
                        }
                        if (failed)
                            UnturnedChat.Say(caller, string.Concat("Failed to retrieve ", itemAsset.itemName, ", free some space and try again!"));
                    }
                }
                player.Player.inventory.updateItems(7, inventory);
                player.Player.inventory.sendStorage();
                return;
            }

            if (Cooldown.ContainsKey(player.CSteamID))
            {
                int timeLeft = Convert.ToInt32(System.Math.Abs((DateTime.Now - Cooldown[player.CSteamID]).TotalSeconds));
                if (timeLeft < OasisPlugin.Instance.Configuration.Instance.MaxCooldown)
                {
                    UnturnedChat.Say(player, OasisPlugin.Instance.Translate("vault_cooldown", OasisPlugin.Instance.Configuration.Instance.MaxCooldown - timeLeft), UnityEngine.Color.red);
                    return;
                }
                Cooldown.Remove(player.CSteamID);
            }

            if (Cooldown.ContainsKey(player.CSteamID))
            {
                Cooldown[player.CSteamID] = DateTime.Now;
            }
            else
            {
                Cooldown.Add(player.CSteamID, DateTime.Now);
            }
        

            byte size = OasisPlugin.Instance.Configuration.Instance.VaultHeight;
            inventory.resize(OasisPlugin.Instance.Configuration.Instance.VaultWidth, size);
            List<ItemJar> items = Database.RetrieveItems(player.CSteamID);
            foreach (ItemJar item in items)
            {
                ItemAsset itemAsset = (ItemAsset)Assets.find(EAssetType.ITEM, item.item.id);
                bool failed = false;
                try
                {
                    inventory.addItem(item.x, item.y, item.rot, item.item);

                }
                catch (Exception err)
                {
                    Logger.Log("Err : " + err);
                    failed = true;
                }
                if (failed)
                    UnturnedChat.Say(caller, string.Concat("Failed to retrieve ", itemAsset.itemName, ", free some space and try again!"));
            }

            inventory.onItemAdded = (byte page, byte index, ItemJar jar) => {
                Database.AddItem(player.CSteamID, jar);
            };

            inventory.onItemRemoved = (byte page, byte index, ItemJar jar) => {
                Database.DeleteItem(player.CSteamID, jar);
            };


            inventory.onItemUpdated = (byte page, byte index, ItemJar jar) =>
            {
                Database.UpdateItem(player.CSteamID, jar);
            };

            player.Player.inventory.updateItems(7, inventory);
            player.Player.inventory.sendStorage();
            UnturnedChat.Say(player, OasisPlugin.Instance.Translate("vault_opened"), UnityEngine.Color.green);
        }
    }
}