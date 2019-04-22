using System;
using System.Collections.Generic;
using System.Text;
using Wycademy.Core.Enums;
using Wycademy.Core.Models;

namespace KiranicoScraper.Database
{
    class DbWeaponLevelBuilder
    {
        private readonly WeaponLevel _level;

        public DbWeaponLevelBuilder(WeaponLevel level)
        {
            _level = level;
        }

        /// <summary>
        /// Initialises this weapon level for MH4U.
        /// </summary>
        /// <param name="raw">The level's displayed raw damage.</param>
        /// <param name="affinity">The level's affinity.</param>
        /// <param name="defense">The level's defense boost.</param>
        /// <param name="slots">A string representing the level's slots.</param>
        /// <param name="modifier">The multiplier used to convert display raw to true raw.</param>
        /// <param name="frenzyAffinity">The weapon's bonus affinity from frenzy virus.</param>
        /// <exception cref="InvalidOperationException">Thrown if the internal <see cref="WeaponLevel"/> is not of type <see cref="WeaponLevel4U"/>.</exception>
        public void InitialiseLevel4U(int raw, int affinity, int defense, string slots, float modifier, int frenzyAffinity)
        {
            var casted = ConvertToGame<WeaponLevel4U>();
            casted.Raw = raw;
            casted.Affinity = affinity;
            casted.Defense = defense;
            casted.Slots = slots;
            casted.DisplayModifier = modifier;
            casted.FrenzyAffinity = frenzyAffinity;
        }

        /// <summary>
        /// Initialises this weapon level for MHGen.
        /// </summary>
        /// <param name="raw">The level's raw damage.</param>
        /// <param name="affinity">The level's affinity.</param>
        /// <param name="defense">The level's defense boost.</param>
        /// <param name="slots">A string representing the level's slots.</param>
        /// <exception cref="InvalidOperationException">Thrown if the internal <see cref="WeaponLevel"/> is not of type <see cref="WeaponLevelGen"/>.</exception>
        public void InitialiseLevelGen(int raw, int affinity, int defense, string slots)
        {
            var casted = ConvertToGame<WeaponLevelGen>();
            casted.Raw = raw;
            casted.Affinity = affinity;
            casted.Defense = defense;
            casted.Slots = slots;
        }

        /// <summary>
        /// Initialises this weapon level for MHWorld.
        /// </summary>
        /// <param name="raw">The level's displayed raw damage.</param>
        /// <param name="affinity">The level's affinity.</param>
        /// <param name="defense">The level's defense boost.</param>
        /// <param name="slots">A string representing the level's slots.</param>
        /// <param name="modifier">The multiplier used to convert display raw to true raw.</param>
        /// <exception cref="InvalidOperationException">Thrown if the internal <see cref="WeaponLevel"/> is not of type <see cref="WeaponLevelWorld"/>.</exception>
        public void InitialiseLevelWorld(int raw, int affinity, int defense, string slots, float modifier)
        {
            var casted = ConvertToGame<WeaponLevelWorld>();
            casted.Raw = raw;
            casted.Affinity = affinity;
            casted.Defense = defense;
            casted.Slots = slots;
            casted.DisplayModifier = modifier;
        }

        /// <summary>
        /// Adds a weapon effect for this weapon level.
        /// </summary>
        /// <param name="effect">The weapon's effect type.</param>
        /// <param name="attack">The effect's attack value.</param>
        /// <param name="needsAwaken">Whether or not the effect needs awakening to be active.</param>
        public void AddWeaponEffect(WeaponEffectType effectType, int attack, bool needsAwaken)
        {
            _level.WeaponEffects.Add(new WeaponEffect
            {
                EffectType = effectType,
                Attack = attack,
                NeedsAwaken = needsAwaken
            });
        }

        /// <summary>
        /// Adds a sharpness level for this weapon level.
        /// </summary>
        /// <param name="ordinal">The sharpness' ordinal, used for ordering purposes.</param>
        /// <param name="values">A list with a length of 7, representing the values of each sharpness colour.</param>
        public void AddWeaponSharpness(int ordinal, IList<int> values)
        {
            values.ExpectLength(7);

            _level.WeaponSharpnesses.Add(new WeaponSharpness
            {
                SharpnessOrdinal = ordinal,
                Red = values[0],
                Orange = values[1],
                Yellow = values[2],
                Green = values[3],
                Blue = values[4],
                White = values[5],
                Purple = values[6]
            });
        }

        /// <summary>
        /// Adds a set of horn notes for this weapon level.
        /// </summary>
        /// <param name="values">A list of length 3, representing the 3 notes.</param>
        public void AddHornNotes(IList<HornNote> values)
        {
            values.ExpectLength(3);

            _level.HornNotes = new HornNotes
            {
                Note1 = values[0],
                Note2 = values[1],
                Note3 = values[2]
            };
        }

        /// <summary>
        /// Adds gunlance shell stats for this weapon level.
        /// </summary>
        /// <param name="shellType">The shell's type (Normal, Long, or Wide).</param>
        /// <param name="shellLevel">The shell's level.</param>
        public void AddGunlanceShellStats(string shellType, int shellLevel)
        {
            _level.GunlanceShellStats = new GunlanceShellStats
            {
                ShellType = shellType,
                ShellLevel = shellLevel
            };
        }

        /// <summary>
        /// Adds a switch axe or charge blade phials for this weapon level.
        /// </summary>
        /// <param name="phialType">The phial's damage type.</param>
        /// <param name="phialValue">The phial's damage value, if applicable.</param>
        public void AddPhials(string phialType, int? phialValue)
        {
            _level.Phials = new Phial
            {
                PhialType = phialType,
                PhialValue = phialValue
            };
        }

        /// <summary>
        /// Adds bow stats for this weapon level.
        /// </summary>
        /// <param name="arcShot">The bow's arc shot type (for 4U, Power counts as an arc shot).</param>
        /// <param name="chargeShots">The bow's charged shot levels.</param>
        /// <param name="coatings">The coatings usable by the bow.</param>
        public void AddBowStats(string arcShot, string[] chargeShots, string[] coatings)
        {
            _level.BowStats = new BowStats
            {
                ArcShot = arcShot,
                ChargeShots = chargeShots,
                Coatings = coatings
            };
        }

        /// <summary>
        /// Adds gun stats for this weapon level.
        /// </summary>
        /// <param name="values">A list of length 3, representing the reload speed, recoil, and deviation.</param>
        public void AddGunStats(IList<string> values)
        {
            _level.GunStats = new GunStats
            {
                ReloadSpeed = values[0],
                Recoil = values[1],
                Deviation = values[2]
            };
        }

        /// <summary>
        /// Adds a gun shot for this level's gun stats.
        /// </summary>
        /// <param name="name">The shot's name.</param>
        /// <param name="clipSize">The shot's clip size.</param>
        /// <param name="needsSkill">Whether the shot can be used by default or requires an armour skill.</param>
        public void AddGunShot(string name, int clipSize, bool needsSkill)
        {
            _level.GunStats.GunShots.Add(new GunShot
            {
                Name = name,
                ClipSize = clipSize,
                NeedsSkill = needsSkill
            });
        }

        /// <summary>
        /// Adds a gun internal shot for this level's gun stats.
        /// </summary>
        /// <param name="name">The shot's name.</param>
        /// <param name="capacity">The number of bullets that are given at the start of the quest.</param>
        /// <param name="clipSize">The shot's clip size.</param>
        public void AddGunInternalShot(string name, int capacity, int clipSize)
        {
            _level.GunStats.GunInternalShots.Add(new GunInternalShot
            {
                Name = name,
                Capacity = capacity,
                ClipSize = clipSize
            });
        }

        /// <summary>
        /// Adds a gun rapid fire shot for this level's gun stats.
        /// </summary>
        /// <param name="name">The shot's name/</param>
        /// <param name="shotCount">The number of shots per volley.</param>
        /// <param name="modifier">The damage modifier applied to each shot.</param>
        /// <param name="waitTime">THe wait time between volleys.</param>
        public void AddGunRapidFireShot(string name, int shotCount, float? modifier = null, string waitTime = null)
        {
            _level.GunStats.GunRapidFireShots.Add(new GunRapidFireShot
            {
                Name = name,
                ShotCount = shotCount,
                Modifier = modifier,
                WaitTime = waitTime
            });
        }

        /// <summary>
        /// Adds a gun crouching fire shot for this level's gun stats.
        /// </summary>
        /// <param name="name">The shot's name.</param>
        /// <param name="clipSize">The shot's clip size.</param>
        public void AddGunCrouchingFireShot(string name, int clipSize)
        {
            _level.GunStats.GunCrouchingFireShots.Add(new GunCrouchingFireShot
            {
                Name = name,
                ClipSize = clipSize
            });
        }

        private T ConvertToGame<T>()
            where T: WeaponLevel
        {
            if (_level is T casted)
            {
                return casted;
            }
            throw new InvalidOperationException($"Cannot convert inner WeaponLevel of type '{_level.GetType()}' to type '{typeof(T)}'.");
        }
    }
}
