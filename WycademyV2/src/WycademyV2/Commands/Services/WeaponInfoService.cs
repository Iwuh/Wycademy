using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WycademyV2.Commands.Entities;

namespace WycademyV2.Commands.Services
{
    public class WeaponInfoService
    {
        private readonly string[] FILENAMES = new string[] { "bow", "chargeblade", "dualblades", "greatsword", "gunlance", "hammer", "heavybowgun", "huntinghorn", "insectglaive", "lance", "lightbowgun", "longsword", "switchaxe", "swordshield" };

        private List<WeaponInfo> _weapons;
        private List<string> _pages;
        private StringBuilder _currentPage;

        public WeaponInfoService()
        {
            var deserialized = new List<List<WeaponInfo>>();
            foreach (var name in FILENAMES)
            {
                deserialized.Add(JsonConvert.DeserializeObject<List<WeaponInfo>>(string.Join(Path.DirectorySeparatorChar.ToString(), WycademyConst.DATA_LOCATION, "weapon", $"{name}.json")));
            }
            // Flatten the list of lists into a single list.
            _weapons = deserialized.SelectMany(x => x).ToList();

            _pages = new List<string>();
            _currentPage = new StringBuilder();
        }

        /// <summary>
        /// Find 0 or more WeaponInfo objects using part or all of the weapon's name.
        /// </summary>
        /// <param name="searchTerm">The string to search names for.</param>
        /// <returns>0 or more search results.</returns>
        public List<WeaponInfo> SearchWeaponInfo(string searchTerm)
        {
            var lowerTerm = searchTerm.ToLower();
            return _weapons.Where(w => w.Name.ToLower().Contains(searchTerm)).ToList();
        }

        /// <summary>
        /// Gets a WeaponInfo by its ID.
        /// </summary>
        /// <param name="id">The weapon's ID.</param>
        /// <returns>The WeaponInfo with the requested ID, or null if not found.</returns>
        public WeaponInfo GetWeaponInfoById(int? id)
        {
            return _weapons.FirstOrDefault(w => w.ID == id);
        }

        public List<string> BuildWeaponInfoPages(WeaponInfo info)
        {
            return new WeaponInfoBuilder().Build(info, GetWeaponInfoById(info.UpgradesFrom));
        }
    }
}
