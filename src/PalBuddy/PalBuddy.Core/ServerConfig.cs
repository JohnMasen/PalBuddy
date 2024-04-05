using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PalBuddy.Core
{
    public class ServerConfig
    {
        private static Regex kvMatch = new Regex(@"OptionSettings=[(]{1}((?<key>\w+)=(?<value>[^,]+)[,]{0,})+[)]{1}");
        public ServerConfigDifficultyEnum Difficulty { get; set; } = ServerConfigDifficultyEnum.None;
        public float DayTimeSpeedRate { get; set; } = 1f;
        public float NightTimeSpeedRate { get; set; } = 1f;
        public float ExpRate { get; set; } = 1f;
        public float PalCaptureRate { get; set; } = 1f;
        public float PalSpawnNumRate { get; set; } = 1f;
        public float PalDamageRateAttack { get; set; } = 1f;
        public float PalDamageRateDefense { get; set; } = 1f;
        public float PlayerDamageRateAttack { get; set; } = 1f;
        public float PlayerDamageRateDefense { get; set; } = 1f;
        public float PlayerStomachDecreaceRate { get; set; } = 1f;
        public float PlayerAutoHPRegeneRate { get; set; } = 1f;
        public float PlayerAutoHpRegeneRateInSleep { get; set; } = 1f;
        public float PalStomachDecreaceRate { get; set; } = 1f;
        public float PalStaminaDecreaceRate { get; set; } = 1f;
        public float PalAutoHpRegeneRateInSleep { get; set; } = 1f;
        public float BuildObjectDamageRate { get; set; } = 1f;
        public float BuildObjectDeteriorationDamageRate { get; set; } = 1f;
        public float CollectionDropRate { get; set; } = 1f;
        public float CollectionObjectHpRate { get; set; } = 1f;
        public float CollectionObjectRespawnSpeedRate { get; set; } = 1f;

        public float EnemyDropItemRate { get; set; } = 1f;
        public ServerConfigDeathPenaltyEnum DeathPenalty { get; set; }

        [StreamPropertyName("bEnablePlayerToPlayerDamage")]
        public bool EnablePlayerToPlayerDamage { get; set; } = false;

        [StreamPropertyName("bEnableFriendlyFire")]
        public bool EnableFriendlyFire { get; set; } = false;

        [StreamPropertyName("bEnableInvaderEnemy")]
        public bool EnableInvaderEnemy { get; set; } = true;

        [StreamPropertyName("bActiveUNKO")]
        public bool ActiveUNKO { get; set; } = true;
        [StreamPropertyName("bEnableAimAssistPad")]
        public bool EnableAimAssistPad { get; set; } = true;
        [StreamPropertyName("bEnableAimAssistKeyboard")]
        public bool EnableAimAssistKeyboard { get; set; } = false;

        public int DropItemMaxNum { get; set; } = 3000;
        public int DropItemMaxNum_UNKO { get; set; } = 100;
        public int BaseCampMaxNum { get; set; } = 128;

        public int BaseCampWorkerMaxNum { get; set; } = 15;
        public float DropItemAliveMaxHours { get; set; } = 1f;
        [StreamPropertyName("bAutoResetGuildNoOnlinePlayers")]
        public bool AutoResetGuildNoOnlinePlayers { get; set; } = false;
        public float AutoResetGuildTimeNoOnlinePlayers { get; set; } = 72;
        public int GuildPlayerMaxNum { get; set; } = 20;
        public float PalEggDefaultHatchingTime { get; set; } = 72f;
        public float WorkSpeedRate { get; set; } = 1f;
        [StreamPropertyName("bIsMultiplay")]
        public bool IsMultiplay { get; set; } = false;

        [StreamPropertyName("bIsPvP")]
        public bool IsPVP { get; set; } = false;
        [StreamPropertyName("bCanPickupOtherGuildDeathPenaltyDrop")]
        public bool CanPickupOtherGuildDeathPenaltyDrop { get; set; } = false;
        [StreamPropertyName("bEnableNonLoginPenalty")]
        public bool EnableNonLoginPenalty { get; set; } = false;
        [StreamPropertyName("bEnableFastTravel")]
        public bool EnableFastTravel { get; set; } = true;

        [StreamPropertyName("bIsStartLocationSelectByMap")]
        public bool IsStartLocationSelectByMap { get; set; } = true;
        [StreamPropertyName("bExistPlayerAfterLogout")]
        public bool ExistPlayerAfterLogout { get; set; }
        [StreamPropertyName("bEnableDefenseOtherGuildPlayer")]
        public bool EnableDefenseOtherGuildPlayer { get; set; } = false;

        public int CoopPlayerMaxNum { get; set; } = 4;

        public int ServerPlayerMaxNum { get; set; } = 32;

        public string ServerName { get; set; } = "Default Palworld Server";
        public string ServerDescription { get; set; } = string.Empty;
        public string AdminPassword { get; set; } = string.Empty;
        public string ServerPassword { get; set; } = string.Empty;
        public int PublicPort { get; set; } = 8211;
        public string PublicIP { get; set; } = string.Empty;
        public bool RCONEnabled { get; set; } = false;
        public int RCONPort { get; set; } = 25575;
        public string Region { get; set; } = string.Empty;
        [StreamPropertyName("bUseAuth")]
        public bool UseAuth { get; set; } = true;
        public string BanListURL { get; set; } = "https://api.palworldgame.com/api/banlist.txt";


        public static ServerConfig ReadFrom(Stream stream)
        {
            using StreamReader reader = new StreamReader(stream,Encoding.UTF8);
            string? buffer = reader.ReadLine();
            Dictionary<string, string> kvTable = new Dictionary<string, string>();
            while (buffer != null)
            {
                buffer = buffer.Trim();
                var x=kvMatch.Match(buffer);
                if (x.Success)
                {
                    var keys = x.Groups["key"].Captures;
                    var values = x.Groups["value"].Captures;
                    if (keys.Count!=values.Count)
                    {
                        throw new InvalidOperationException("invalid INI file format, value pair not match in OptionsSettings section");
                    }
                    for (int i = 0; i < keys.Count; i++)
                    {
                        var v = values[i].Value;
                        if (v.StartsWith("\"") && v.EndsWith("\""))
                        {
                            v = v.Substring(1, v.Length - 2);
                        }
                        kvTable.Add(keys[i].Value, v);
                    }
                    break;
                }
                buffer = reader.ReadLine();
            }
            if (kvTable.Count>0)
            {
                ServerConfig result = new ServerConfig();
                var ps = result.GetType().GetProperties();
                foreach (var p in ps)
                {
                    var name = p.GetCustomAttribute<StreamPropertyNameAttribute>()?.Name ?? p.Name;
                    if(kvTable.TryGetValue(name, out string value))
                    {
                        result.SetPropertyByName(p.Name, value);
                    }
                    //string value = kvTable[name];
                    
                    //if (p.PropertyType.IsEnum)
                    //{
                    //    p.SetValue(result, Enum.Parse(p.PropertyType, kvTable[name]));
                    //}
                    //else
                    //{
                    //    switch (Type.GetTypeCode(p.PropertyType))
                    //    {
                    //        case TypeCode.String:
                    //            p.SetValue(result, kvTable[name]);
                    //            break;
                    //        case TypeCode.Boolean:
                    //            p.SetValue(result, bool.Parse(kvTable[name]));
                    //            break;
                    //        case TypeCode.Single:
                    //            p.SetValue(result, float.Parse(kvTable[name]));
                    //            break;
                    //        case TypeCode.Int32:
                    //            p.SetValue(result, int.Parse(kvTable[name]));
                    //            break;
                    //        default:
                    //            throw new InvalidOperationException($"failed to parse config file,unknown property {p.Name}");
                    //    }
                    //}
                }
                return result;
            }
            throw new InvalidOperationException("failed to parse config file,section [OptionSettings=...] not found");


        }

        public static ServerConfig ReadFrom(string path)
        {
            using var f = File.OpenRead(path);
            return ReadFrom(f);
        }
        public  string GetPropertyByName(string name)
        {
            var p = GetType().GetProperty(name);
            return p.GetValue(this).ToString();
        }

        public  void SetPropertyByName(string name,string value)
        {
            var p = GetType().GetProperty(name);
            if (p.PropertyType.IsEnum)
            {
                p.SetValue(this, Enum.Parse(p.PropertyType, value));
            }
            else
            {
                switch (Type.GetTypeCode(p.PropertyType))
                {
                    case TypeCode.String:
                        p.SetValue(this, value);
                        break;
                    case TypeCode.Boolean:
                        p.SetValue(this, bool.Parse(value));
                        break;
                    case TypeCode.Single:
                        p.SetValue(this, float.Parse(value));
                        break;
                    case TypeCode.Int32:
                        p.SetValue(this, int.Parse(value));
                        break;
                    default:
                        throw new InvalidOperationException($"unknown property {p.Name}");
                }
            }
        }

        public void SaveTo(Stream stream,bool leaveOpen=false)
        {
            using StreamWriter writer = new(stream, Encoding.UTF8,-1,leaveOpen);
            writer.WriteLine("[/Script/Pal.PalGameWorldSettings]");
            writer.Write("OptionSettings=(");
            bool isFirst = true;
            foreach (var p in this.GetType().GetProperties())
            {
                if (!isFirst)
                {
                    writer.Write(",");
                }
                string name = p.GetCustomAttribute<StreamPropertyNameAttribute>()?.Name ?? p.Name;
                writer.Write($"{name}=");

                //if (p.PropertyType.IsEnum)
                //{
                //    writer.Write(Enum.GetName(p.PropertyType, p.GetValue(this)));
                //}

                switch (Type.GetTypeCode(p.PropertyType))
                {
                    //case TypeCode.Boolean:
                    //    writer.Write(p.GetValue(this));
                    //    break;
                    //case TypeCode.Int32:
                    //    writer.Write(p.GetValue(this));
                    //    break;
                    //case TypeCode.Single:
                    //    writer.Write(p.GetValue(this));
                    //    break;
                    case TypeCode.String:
                        writer.Write($"\"{GetPropertyByName(p.Name)}\"");
                        break;
                    default:
                        writer.Write(GetPropertyByName(p.Name));
                        break;
                }


                isFirst = false;
            }
            writer.Write(")");
        }

        public void SaveTo(string path)
        {
            using var f=File.OpenWrite(path);
            SaveTo(f);
        }
    }



    public enum ServerConfigDifficultyEnum
    {
        None,
        Difficulty
    }
    public enum ServerConfigDeathPenaltyEnum
    {
        All,
        None,
        Item,
        ItemAndEquipment
    }

    [System.AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    sealed class StreamPropertyNameAttribute : Attribute
    {
        public string Name { get; }
        public StreamPropertyNameAttribute(string name)
        {
            Name = name;
        }
    }
}
