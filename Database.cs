using Rocket.Core.Logging;
using SDG.Unturned;
using Steamworks;
using System;
using System.Collections.Generic;
using MySqlConnector;


namespace OasisVault
{
    internal class Database
    {
        private static string CompileArguments(string command, string[] args, string[] argsValues)
        {
            string NewCommand = command;
            int i = 0;
            foreach (string arg in args)
            {
                NewCommand = NewCommand.Replace(arg, argsValues[i]);
                i++;
            }
            return NewCommand;
        }


        private static MySqlConnection CreateConnection()
        {
            MySqlConnection connection = null;
            var builder = new MySqlConnectionStringBuilder
            {
                Server = OasisPlugin.Instance.Configuration.Instance.DatabaseAddress,
                UserID = OasisPlugin.Instance.Configuration.Instance.DatabaseUsername,
                Password = OasisPlugin.Instance.Configuration.Instance.DatabasePassword,
                Database = OasisPlugin.Instance.Configuration.Instance.DatabaseName,
                Port = (uint)OasisPlugin.Instance.Configuration.Instance.DatabasePort,
                AllowUserVariables = true
            };

            try
            {
                connection = new MySqlConnection(builder.ConnectionString);
                connection.Open();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql Connection Failed");
                Logger.LogException(ex);
            }

            return connection;
        }
        public static void CheckSchema()
        {
            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql Schema Failed - Connection null");
                return;
            }

            try
            {
                var getCommand = mySqlConnection.CreateCommand();

                string[] args = { "@Tablename" };
                string[] argsValues = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName };
                string cmd = "SHOW TABLES LIKE '@Tablename'";

                getCommand.CommandText = CompileArguments(cmd, args, argsValues);
                var reader = getCommand.ExecuteReader();
                if (reader.Read().ToString() == "False")
                {
                    reader.Close();
                    var setCommand = mySqlConnection.CreateCommand();

                    string[] args2 = { "@Tablename" };
                    string[] argsValues2 = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName };
                    string cmd2 = "CREATE TABLE `@Tablename` (`id` int(11) NOT NULL AUTO_INCREMENT,`durability` int(3) NOT NULL,`stacksize` int(11) NULL,`x` int(11) NULL,`y` int(11) NULL,`rotation` int(11) NULL,`itemid` int(4) NOT NULL,`metadata` varchar(255) NOT NULL,`csteamid` varchar(32) NOT NULL,PRIMARY KEY (`id`)) ";
                    
                    setCommand.CommandText = CompileArguments(cmd2, args2, argsValues2);
                    setCommand.ExecuteNonQuery();
                }
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql Schema Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
            }
        }
        internal static List<ItemJar> RetrieveItems(CSteamID csteamid)
        {
            List<ItemJar> list = new List<ItemJar>();

            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql RetrieveItems Failed - Connection null");
                return null;
            }
            try
            {
                int num = 0;

                string[] args = { "@Tablename", "@cSteamId" };
                string[] argsValues = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName, csteamid.ToString()};
                string cmd = "SELECT COUNT(*) FROM `@Tablename` WHERE `csteamid` = '@cSteamId';";
                MySqlCommand command1 = mySqlConnection.CreateCommand();
                command1.CommandText = CompileArguments(cmd, args, argsValues);
                var reader = command1.ExecuteReader();
                while (reader.Read())
                {
                    num = reader.GetInt32(0);
                }
                reader.Close();
                if (num != 0)
                {
                    string[] args2 = { "@Tablename", "@cSteamId" };
                    string[] argsValues2 = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName, csteamid.ToString() };
                    string cmd2 = "SELECT * FROM `@Tablename` WHERE `csteamid` = '@cSteamId' ORDER BY id;";
                    MySqlCommand command2 = mySqlConnection.CreateCommand();
                    command2.CommandText = CompileArguments(cmd2, args2, argsValues2);
                    var reader2 = command2.ExecuteReader();
                    while (reader2.Read())
                    {
                        list.Add(new ItemJar((byte)reader2.GetInt32("x"), (byte)reader2.GetInt32("y"), (byte)reader2.GetInt32("rotation"), new Item(reader2.GetUInt16("itemid"), true)
                        {
                            durability = (byte)reader2.GetInt32("durability"),
                            metadata = Convert.FromBase64String(reader2.GetString("metadata")),
                            amount = (byte)reader2.GetInt32("stacksize")
                        }));
                    }
                    reader2.Close();

                }
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql RetrieveItems Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
            }
            return list;       
        }

        internal static void UpdateItem(CSteamID cSteamID, ItemJar item)
        {
            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql UpdateItem Failed - Connection null");
                return;
            }

            try
            {
                string text = (item.item.metadata == null) ? "" : Convert.ToBase64String(item.item.metadata);


                string[] args = { "@Tablename", "@metadata", "@csteamid", "@itemx", "@itemy", "@itemid" };
                string[] argsValues = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName, text, cSteamID.ToString(), item.x.ToString(), item.y.ToString(), item.item.id.ToString()};
                string cmd = "UPDATE `@Tablename` SET `metadata`='@metadata' WHERE `csteamid`='@csteamid' AND `x` = '@itemx' AND `y` = '@itemy' AND `itemid` = '@itemid' LIMIT 1;";

                MySqlCommand command1 = mySqlConnection.CreateCommand();
                command1.CommandText = CompileArguments(cmd, args, argsValues);
                command1.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql UpdateItem Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
            }
        }

        internal static bool AddItem(CSteamID csteamid, ItemJar item)
        {
            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql AddItem Failed - Connection null");
                return false;
            }
            try
            {
                string text = (item.item.metadata == null) ? "" : Convert.ToBase64String(item.item.metadata);

                string[] args = { "@Tablename", "@csteamid", "@durability", "@xpos", "@ypos", "@rotation", "@metadata", "@itemid", "@stacksize" };
                string[] argsValues = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName, csteamid.ToString(), item.item.durability.ToString(), item.x.ToString(), item.y.ToString(), item.rot.ToString(), text, item.item.id.ToString(), item.item.amount.ToString() };
                string cmd = "INSERT INTO `@Tablename` (`csteamid`,`durability`,`x`,`y`,`rotation`,`metadata`,`itemid`,`stacksize`) VALUES(@csteamid,@durability,@xpos,@ypos,@rotation,'@metadata',@itemid,@stacksize);";

                MySqlCommand command1 = mySqlConnection.CreateCommand();
                command1.CommandText = CompileArguments(cmd, args, argsValues);

                command1.ExecuteNonQuery();
                mySqlConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql AddItem Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
                return false;
            }
        }
        internal static bool DeleteItem(CSteamID csteamid, ItemJar item)
        {
            MySqlConnection mySqlConnection = Database.CreateConnection();
            if (mySqlConnection == null)
            {
                Logger.LogWarning("MySql DeleteItem Failed - Connection null");
                return false;
            }
            try
            {
                string text = (item.item.metadata == null) ? "" : Convert.ToBase64String(item.item.metadata);

                string[] args = { "@Tablename", "@csteamid", "@itemx", "@itemy", "@durability", "@metadata", "@itemid", "@stacksize" };
                string[] argsValues = { OasisPlugin.Instance.Configuration.Instance.DatabaseTableName, csteamid.ToString(), item.x.ToString(), item.y.ToString(), item.item.durability.ToString(), text, item.item.id.ToString(), item.item.amount.ToString() };
                string cmd = "DELETE FROM `@Tablename` WHERE `csteamid` = '@csteamid' AND `x` = '@itemx' AND `y` = '@itemy' AND `durability` = '@durability' AND `metadata` = '@metadata' AND `itemid` = '@itemid' AND `stacksize` = '@stacksize' LIMIT 1;";

                MySqlCommand command1 = mySqlConnection.CreateCommand();
                command1.CommandText = CompileArguments(cmd, args, argsValues);

                command1.ExecuteNonQuery();
                mySqlConnection.Close();
                return true;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("MySql DeleteItem Failed");
                Logger.LogException(ex);
                mySqlConnection.Close();
                return false;
            }
        }
    }
}
