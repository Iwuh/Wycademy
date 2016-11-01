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
        public const string INVALID_MONSTER_NAME = "is not a valid choice. Try `<info monsterlist` for the list of valid names.";

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

        public static readonly string[] EYE_EMOJIS = new string[] { "<:eyesFlipped:214941562096844800>", "<:eyesCross:214941564185739265>",
                                                                    "<:eyesReverse:214941566475829258>", "<:eyesCenter:214946506350919680>",
                                                                    "<:eyesLeft:215357294987182080>", "<:Eyenoculars:211850820315119617>",
                                                                    "<:flippedEyes:214942499964321793>",":eyes:" };

        public static readonly string[] VOWELS = new string[] { "a", "e", "i", "o", "u", "y" };

        public static Random RandomNumbers = new Random();
    }
}
