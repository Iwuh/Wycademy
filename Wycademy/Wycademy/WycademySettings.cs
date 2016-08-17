using Discord;
using Discord.Commands.Permissions.Userlist;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    static class WycademySettings
    {
        public const char WYCADEMY_PREFIX = '<';
        public const ulong OWNER_ID = 176775302897336331;
        public const string JSON_PATH = @".\..\..\Data\json";
        public const string INVALID_MONSTER_NAME = "is not a valid choice. Try `<info list` for the list of choices.";

        public static readonly string[] MONSTER_LIST = new string[71] 
        {
            "Great_Maccao",
            "Velocidrome",
            "Bulldrome",
            "Seltas",
            "Arzuros",
            "Redhelm_Arzuros",
            "Gendrome",
            "Cephadrome",
            "Yian_Kut-Ku",
            "Iodrome",
            "Kecha_Wacha",
            "Lagombi",
            "Snowbaron_Lagombi",
            "Gypceros",
            "Tetsucabra",
            "Drilltusk_Tetsucabra",
            "Daimyo_Hermitaur",
            "Stonefist_Hermitaur",
            "Volvidon",
            "Royal_Ludroth",
            "Malfestio",
            "Zamtrios",
            "Khezu",
            "Rathian",
            "Gold_Rathian",
            "Dreadqueen_Rathian",
            "Nibelsnarf",
            "Plesioth",
            "Blagonga",
            "Lavasioth",
            "Shogun_Ceanataur",
            "Najarala",
            "Nargacuga",
            "Silverwind_Nargacuga",
            "Yian_Garuga",
            "Deadeye_Yian_Garuga",
            "Uragaan",
            "Crystalbeard_Uragaan",
            "Seltas_Queen",
            "Rathalos",
            "Silver_Rathalos",
            "Dreadking_Rathalos",
            "Lagiacrus",
            "Zinogre",
            "Thunderlord_Zinogre",
            "Mizutsune",
            "Astalos",
            "Gammoth",
            "Glavenus",
            "Hellblade_Glavenus",
            "Agnaktor",
            "Gore_Magala",
            "Seregios",
            "Duramboros",
            "Tigrex",
            "Grimclaw_Tigrex",
            "Kirin",
            "Brachydios",
            "Shagaru_Magala",
            "Rajang",
            "Furious_Rajang",
            "Deviljho",
            "Savage_Deviljho",
            "Kushala_Daora",
            "Chameleos",
            "Teostra",
            "Akantor",
            "Ukanlos",
            "Amatsu",
            "Nakarkos",
            "Alatreon"
        };
        public static readonly string[] HITZONE_COLUMN_NAMES = new string[] { "Cut", "Impact", "Shot", "Fire", "Water", "Ice", "Thunder", "Dragon" };
        public static readonly string[] STAGGER_COLUMN_NAMES = new string[] { "Stagger Value", "Sever Value", "Extract Colour" };
        public static readonly string[] STATUS_COLUMN_NAMES = new string[] { "Initial", "Increase", "Max", "Duration", "Reduction", "Damage" };
        public static readonly string[] ITEMEFFECTS_COLUMN_NAMES = new string[] { "Duration Normal", "Duration Enraged", "Duration Fatigued" };

        public static List<ulong> InitializeBlacklist()
        {
            // Populate the blacklist with the IDs stored in blacklist.txt
            using (StreamReader sr = new StreamReader("blacklist.txt"))
            {
                if (sr.ReadToEnd() != string.Empty)
                {
                    return sr.ReadToEnd().Split(',').Select(x => ulong.Parse(x)).ToList();
                }
                return new List<ulong>();
            }
        }

        public static async Task UpdateBlacklist(DiscordClient _client)
        {
            // Converts Blacklist to a string and then writes it to a file.
            string blacklistString = string.Join(",", _client.GetBlacklistedUserIds());

            using (StreamWriter sw = new StreamWriter("blacklist.txt", false))
            {
                await sw.WriteAsync(blacklistString);
            }
        }
    }
}
