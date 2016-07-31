using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wycademy
{
    static class WycademyConst
    {
        public static char WYCADEMY_PREFIX = '<';
        public static ulong OWNER_ID = 176775302897336331;
        public static string JSON_PATH = @".\..\..\Data\json";
        public static string INVALID_MONSTER_NAME = "is not a valid choice. Try `<info list` for the list of choices.";

        public static string[] MONSTER_LIST = new string[71] 
        {
            "Great Maccao",
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
        public static string[] HITZONE_COLUMN_NAMES = new string[] { "Cut", "Impact", "Shot", "Fire", "Water", "Ice", "Thunder", "Dragon" };
        public static string[] STAGGER_COLUMN_NAMES = new string[] { "Stagger Value", "Sever Value", "Extract Colour" };
        public static string[] STATUS_COLUMN_NAMES = new string[] { "Initial", "Increase", "Max", "Duration", "Reduction", "Damage" };
        public static string[] ITEMEFFECTS_COLUMN_NAMES = new string[] { "Duration Normal", "Duration Enraged", "Duration Fatigued" };
    }
}
