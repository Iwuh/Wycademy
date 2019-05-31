using System;
using System.Collections.Generic;
using System.Linq;
using Wycademy.Core.Enums;
using Wycademy.Core.Models;

namespace KiranicoScraper.Database
{
    class DbMonsterBuilder
    {
        private readonly WycademyContext _context;
        private Monster _monster;
        private bool _monsterExistsInDatabase;

        public DbMonsterBuilder(WycademyContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds the built <see cref="Monster"/> to the database in a single transaction.
        /// </summary>
        public void Commit()
        {
            if (_monster == null) throw new InvalidOperationException("Cannot add uninitialised monster to database.");

            if (!_monsterExistsInDatabase)
            {
                _context.Add(_monster);
            }
            _context.SaveChanges();
        }

        /// <summary>
        /// Initialises the <see cref="Monster"/> instance.
        /// </summary>
        /// <param name="webName">The monster's Kiranico URL name.</param>
        public void InitialiseMonster(string webName)
        {
            var monster = _context.Monsters.SingleOrDefault(m => m.WebName == webName);

            // Record whether or not a row already exists for the monster; avoids key violations.
            _monsterExistsInDatabase = monster != null;
            _monster = _monsterExistsInDatabase ? monster :
                new Monster
                {
                    WebName = webName,
                    ProperName = Utils.GetFormattedMonsterName(webName)
                };
        }

        /// <summary>
        /// Adds a hitzone for the monster.
        /// </summary>
        /// <param name="game">The game this hitzone applies to.</param>
        /// <param name="partName">The name of the body part.</param>
        /// <param name="values">A list of length 8 containing the values for cut, impact, shot, fire, water, ice, thunder, and dragon, in that order.</param>
        public void AddHitzone(Game game, string partName, IList<int> values)
        {
            values.ExpectLength(8);

            _monster.Hitzones.Add(new Hitzone
            {
                Name = partName,
                Game = game,
                Cut = values[0],
                Impact = values[1],
                Shot = values[2],
                Fire = values[3],
                Water = values[4],
                Ice = values[5],
                Thunder = values[6],
                Dragon = values[7]
            });
        }

        /// <summary>
        /// Adds a MH4U hitzone for the monster.
        /// </summary>
        /// <param name="partName">The name of the body part.</param>
        /// <param name="staggerValue">The damage threshold for a stagger.</param>
        /// <param name="extractColour">The kinsect extract colour.</param>
        public void AddStagger4U(string partName, int staggerValue, string extractColour)
        {
            _monster.StaggerLimits.Add(new StaggerLimit4U
            {
                Name = partName,
                Stagger = staggerValue,
                ExtractColour = extractColour
            });
        }

        /// <summary>
        /// Adds a MHGen hitzone for the monster.
        /// </summary>
        /// <param name="partName">The name of the body part.</param>
        /// <param name="staggerValue">The damage threshold for a stagger.</param>
        /// <param name="extractColour">The kinsect extract colour.</param>
        /// <param name="severValue">The damage threshold to sever the part, if applicable.</param>
        public void AddStaggerGen(string partName, int staggerValue, string extractColour, int? severValue)
        {
            _monster.StaggerLimits.Add(new StaggerLimitGen
            {
                Name = partName,
                Stagger = staggerValue,
                ExtractColour = extractColour,
                Sever = severValue
            });
        }

        /// <summary>
        /// Adds a MHW hitzone for the monster.
        /// </summary>
        /// <param name="partName">The name of the body part.</param>
        /// <param name="staggerValue">The damage threshold for a stagger.</param>
        /// <param name="extractColour">The kinsect extract colour.</param>
        /// <param name="severValue">The damage threshold to sever the part, if applicable.</param>
        /// <param name="breakValue">The damage threshold to break the part, if applicable.</param>
        public void AddStaggerWorld(string partName, int staggerValue, string extractColour, int? severValue, int? breakValue)
        {
            _monster.StaggerLimits.Add(new StaggerLimitWorld
            {
                Name = partName,
                Stagger = staggerValue,
                ExtractColour = extractColour,
                Sever = severValue,
                Break = breakValue
            });
        }

        /// <summary>
        /// Adds a status effect for the monster.
        /// </summary>
        /// <param name="game">The game this status effect applies to.</param>
        /// <param name="statusName">The name of the status effect.</param>
        /// <param name="values">A list of length 7 containing the values for initial, increase, max, duration, damage, reduction time, and reduction amount, in that order.</param>
        public void AddStatus(Game game, string statusName, IList<int> values)
        {
            values.ExpectLength(7);

            _monster.StatusEffects.Add(new StatusEffect
            {
                Game = game,
                Name = statusName,
                InitialThreshold = values[0],
                Increase = values[1],
                MaxThreshold = values[2],
                Duration = values[3],
                Damage = values[4],
                ReductionTime = values[5],
                ReductionAmount = values[6]
            });
        }

        /// <summary>
        /// Adds an item effect for the monster.
        /// </summary>
        /// <param name="game">The game this item effect applies to.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="values">A list of length 3 containing the values for normal duration, enraged duration, and fatigued duration.</param>
        public void AddItemEffect(Game game, string itemName, IList<int> values)
        {
            values.ExpectLength(3);

            _monster.ItemEffects.Add(new ItemEffect
            {
                Game = game,
                Name = itemName,
                Normal = values[0],
                Enraged = values[1],
                Fatigued = values[2]
            });
        }
    }
}
