using System;
using Wycademy.Core.Enums;
using Wycademy.Core.Models;

namespace KiranicoScraper.Database
{
    class DbWeaponBuilder
    {
        private readonly WycademyContext _context;

        private Weapon _weapon;
        private Game _game;
        private int _levelOrdinal;

        public DbWeaponBuilder(WycademyContext context)
        {
            _context = context;
            _levelOrdinal = 1;
        }

        /// <summary>
        /// Adds the built <see cref="Weapon"/> to the database in a single transaction and returns its generated id.
        /// </summary>
        /// <returns>The weapon's id, to be used as a foreign key.</returns>
        public int Commit()
        {
            if (_weapon == null) throw new InvalidOperationException("Cannot add uninitialised weapon to database.");

            _context.Add(_weapon);
            _context.SaveChanges();

            if (_weapon.WeaponType != WeaponType.HBG && _weapon.WeaponType != WeaponType.LBG && _weapon.WeaponType != WeaponType.Bow)
            {
                foreach (WeaponLevel level in _weapon.WeaponLevels)
                {
                    SharpnessImageGenerator.GenerateImage(_game, level.Id, level.WeaponSharpnesses);
                }
            }

            return _weapon.Id;
        }

        /// <summary>
        /// Initialises the <see cref="Weapon"/> instance as a 4U weapon.
        /// </summary>
        /// <param name="name">The weapon's name.</param>
        /// <param name="type">The weapon's type.</param>
        /// <param name="rare">The weapon's rarity level.</param>
        /// <param name="url">The URL of the weapon's Kiranico page.</param>
        /// <param name="parentId">The database id of the weapon's parent, or null if there is none.</param>
        public void InitialiseWeapon4U(string name, WeaponType type, string rare, string url, int? parentId)
        {
            _game = Game.Four;
            _weapon = new Weapon4U
            {
                Name = name,
                WeaponType = type,
                Rare = rare,
                Url = url,
                ParentId = parentId
            };
        }

        /// <summary>
        /// Initialises the <see cref="Weapon"/> instance as a Gen weapon.
        /// </summary>
        /// <param name="name">The weapon's name before being fully upgraded.</param>
        /// <param name="type">The weapon's type.</param>
        /// <param name="rare">The weapon's rarity level.</param>
        /// <param name="url">The URL of the weapon's Kiranico page.</param>
        /// <param name="parentId">The database id of the weapon's parent, or null if there is none.</param>
        /// <param name="finalName">The weapon's name when fully upgraded.</param>
        /// <param name="description">The weapon's description before being fully upgraded.</param>
        /// <param name="finalDescription">The weapon's description when fully upgraded.</param>
        public void InitialiseWeaponGen(string name, WeaponType type, string rare, string url, int? parentId, string finalName, string description, string finalDescription)
        {
            _game = Game.Generations;
            _weapon = new WeaponGen
            {
                Name = name,
                WeaponType = type,
                Rare = rare,
                Url = url,
                ParentId = parentId,
                FinalName = finalName,
                FinalDescription = finalDescription
            };
        }

        /// <summary>
        /// Initialises the <see cref="Weapon"/> instance as a World weapon.
        /// </summary>
        /// <param name="name">The weapon's name.</param>
        /// <param name="type">The weapon's type.</param>
        /// <param name="rare">The weapon's rarity level.</param>
        /// <param name="url">The URL of the weapon's Kiranico page.</param>
        /// <param name="parentId">The database id of the weapon's parent, or null if there is none.</param>
        public void InitialiseWeaponWorld(string name, WeaponType type, string rare, string url, int? parentId)
        {
            _game = Game.World;
            _weapon = new WeaponWorld
            {
                Name = name,
                WeaponType = type,
                Rare = rare,
                Url = url,
                ParentId = parentId
            };
        }

        /// <summary>
        /// Gets a <see cref="DbWeaponLevelBuilder"/> used to configure a singular weapon level for this weapon.
        /// </summary>
        public DbWeaponLevelBuilder CreateWeaponLevelBuilder()
        {
            WeaponLevel level;
            switch (_game)
            {
                case Game.Four when _levelOrdinal > 1:
                case Game.World when _levelOrdinal > 1:
                    throw new InvalidOperationException($"A weapon with game type '{_game}' cannot have more than one level.");
                case Game.Four:
                    level = new WeaponLevel4U { LevelOrdinal = _levelOrdinal };
                    break;
                case Game.Generations:
                    level = new WeaponLevelGen { LevelOrdinal = _levelOrdinal };
                    break;
                case Game.World:
                    level = new WeaponLevelWorld { LevelOrdinal = _levelOrdinal };
                    break;
                default:
                    throw new NotSupportedException($"Unknown game type: '{_game}'");
            }
            _levelOrdinal++;
            _weapon.WeaponLevels.Add(level);
            return new DbWeaponLevelBuilder(level);
        }
    }
}
