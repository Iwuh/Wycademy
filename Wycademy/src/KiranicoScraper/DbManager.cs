using KiranicoScraper.Enums;
using KiranicoScraper.Scrapers.Lists;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NLog;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;

namespace KiranicoScraper
{
    class DbManager : IDisposable
    {
        private const string CONNECTION_STRING = "Host=localhost;Database=WycademyTest;Username=matt_;Password=Lagiacrus";

        private NpgsqlConnection _conn;
        private Logger _logger = LogManager.GetCurrentClassLogger();

        public ScraperListCollection ScraperLists { get; }

        public DbManager()
        {
            _logger.Info($"Connecting to database with connection string '{CONNECTION_STRING}'");

            _conn = new NpgsqlConnection(CONNECTION_STRING);
            _conn.Open();

            CreateTables();
            _conn.ReloadTypes();
            _conn.TypeMapper.MapEnum<Game>("public.game_enum");
            _conn.TypeMapper.MapEnum<WeaponType>("public.weapon_type_enum");
            _conn.TypeMapper.MapEnum<WeaponEffect>("public.weapon_effect_enum");
            _conn.TypeMapper.MapEnum<HornNote>("public.horn_note_enum");

            _logger.Info("Created tables");

            ScraperLists = JsonConvert.DeserializeObject<ScraperListCollection>(File.ReadAllText(@"Data\ScraperLists.json", new UTF8Encoding(false)));
        }

        public NpgsqlTransaction BeginTransaction()
        {
            return _conn.BeginTransaction();
        }

        #region Monster Methods

        /// <summary>
        /// Adds a monster to the database if it's not already in.
        /// </summary>
        /// <param name="webName">The monster's web name.</param>
        /// <returns>The monster's internal ID to be used as a foreign key.</returns>
        public int AddMonsterAndGetId(string webName)
        {
            string properName = Utils.GetFormattedMonsterName(webName);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO monsters(web_name, proper_name)
                    VALUES (@web, @prop)
                    ON CONFLICT DO NOTHING;";

                cmd.AddParameters(
                    new NpgsqlParameter<string>("web", webName),
                    new NpgsqlParameter<string>("prop", properName));

                cmd.ExecuteNonQuery();
            }

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = "SELECT id FROM monsters WHERE web_name=@name;";

                cmd.Parameters.Add(new NpgsqlParameter<string>("name", webName));

                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Adds a hitzone to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="monsterId">The ID of the monster that the hitzone belongs to.</param>
        /// <param name="game">The game that the hitzone applies to.</param>
        /// <param name="partName">The body part that the hitzone applies to.</param>
        /// <param name="values">A list with a length of 8, representing the values for each of the 8 columns.</param>
        public void AddHitzone(int monsterId, Game game, string partName, IList<int> values)
        {
            AssertLength(values, 8);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO hitzones(monster_id, game, name, cut, impact, shot, fire, water, ice, thunder, dragon)
                    VALUES (@id, @game, @name, @hz0, @hz1, @hz2, @hz3, @hz4, @hz5, @hz6, @hz7)
                    ON CONFLICT (monster_id, game, name)
                    DO UPDATE
                    SET cut     = excluded.cut,
                        impact   = excluded.impact,
                        shot    = excluded.shot,
                        fire    = excluded.fire,
                        water   = excluded.water,
                        ice     = excluded.ice,
                        thunder = excluded.thunder,
                        dragon  = excluded.dragon;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", monsterId),
                    new NpgsqlParameter<Game>("game", game),
                    new NpgsqlParameter<string>("name", partName));

                cmd.AddParameters(Utils.Sequence(0, 8, i => new NpgsqlParameter<int>($"hz{i}", values[i])));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a stagger limit for MH4U, replacing it if it already exists.
        /// </summary>
        /// <param name="monsterId">The ID of the monster that the stagger limit belongs to.</param>
        /// <param name="partName">The body part that the stagger limit applies to.</param>
        /// <param name="staggerValue">The damage needed to stagger the monster.</param>
        /// <param name="extractColour">The Kinsect extract colour.</param>
        public void AddStagger4U(int monsterId, string partName, int staggerValue, string extractColour)
            => AddStaggerCommon(monsterId, Game.Four, partName, staggerValue, extractColour);

        /// <summary>
        /// Adds a stagger limit for MHGen, replacing it if it already exists.
        /// </summary>
        /// <param name="monsterId">The ID of the monster that the stagger limit belongs to.</param>
        /// <param name="partName">The body part that the stagger limit applies to.</param>
        /// <param name="staggerValue">The damage needed to stagger the monster.</param>
        /// <param name="extractColour">The Kinsect extract colour.</param>
        /// <param name="severValue">The damage needed to sever the part, or null if not applicable.</param>
        public void AddStaggerGen(int monsterId, string partName, int staggerValue, string extractColour, int? severValue)
        {
            var commonId = AddStaggerCommon(monsterId, Game.Generations, partName, staggerValue, extractColour);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                        INSERT INTO stagger_limits_gen(common_id, sever)
                        VALUES (@id, @sever) 
                        ON CONFLICT (common_id)
                        DO UPDATE SET sever = excluded.sever;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", commonId),
                    new NpgsqlParameter("sever", severValue ?? (object)DBNull.Value));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a stagger limit for MHWorld, replacing it if it already exists.
        /// </summary>
        /// <param name="monsterId">The ID of the monster that the stagger limit belongs to.</param>
        /// <param name="partName">The body part that the stagger limit applies to.</param>
        /// <param name="staggerValue">The damage needed to stagger the monster.</param>
        /// <param name="extractColour">The Kinsect extract colour.</param>
        /// <param name="severValue">The damage needed to sever the part, or null if not applicable.</param>
        /// <param name="breakValue">The damage needed to break the part, or null if not applicable.</param>
        public void AddStaggerWorld(int monsterId, string partName, int staggerValue, string extractColour, int? severValue, int? breakValue)
        {
            var commonId = AddStaggerCommon(monsterId, Game.World, partName, staggerValue, extractColour);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                        INSERT INTO stagger_limits_world(common_id, break, sever)
                        VALUES (@id, @break, @sever) 
                        ON CONFLICT (common_id)
                        DO UPDATE 
                        SET break = excluded.break, sever = excluded.sever;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", commonId),
                    new NpgsqlParameter("break", breakValue ?? (object)DBNull.Value),
                    new NpgsqlParameter("sever", severValue ?? (object)DBNull.Value));

                cmd.ExecuteNonQuery();
            }
        }

        private int AddStaggerCommon(int monsterId, Game game, string partName, int staggerValue, string extractColour)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO stagger_limits_common (monster_id, game, name, stagger, extract_colour)
                    VALUES (@id, @game, @name, @stagger, @colour)
                    ON CONFLICT (monster_id, game, name)
                    DO UPDATE
                    SET stagger = excluded.stagger, extract_colour = excluded.extract_colour
                    RETURNING id;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", monsterId),
                    new NpgsqlParameter<Game>("game", game),
                    new NpgsqlParameter<string>("name", partName),
                    new NpgsqlParameter<int>("stagger", staggerValue),
                    new NpgsqlParameter<string>("colour", extractColour));

                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Adds a monster's status vulnerability to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="monsterId">The ID of the monster that the status vulnerability belongs to.</param>
        /// <param name="game">The game that the status vulnerability applies to.</param>
        /// <param name="statusName">The name of the status.</param>
        /// <param name="values">A list of length 7, representing the values for each of the 7 columns.</param>
        public void AddStatus(int monsterId, Game game, string statusName, IList<int> values)
        {
            AssertLength(values, 7);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO status_effects (monster_id, game, name, initial_threshold, increase, max_threshold, duration, damage, reduction_time, reduction_amount)
                    VALUES (@id, @game, @name, @s0, @s1, @s2, @s3, @s4, @s5, @s6)
                    ON CONFLICT (monster_id, game, name)
                    DO UPDATE
                    SET initial_threshold = excluded.initial_threshold,
                        increase          = excluded.increase,
                        max_threshold     = excluded.max_threshold,
                        duration          = excluded.duration,
                        damage            = excluded.damage,
                        reduction_time    = excluded.reduction_time,
                        reduction_amount  = excluded.reduction_amount;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", monsterId),
                    new NpgsqlParameter<Game>("game", game),
                    new NpgsqlParameter<string>("name", statusName));

                cmd.AddParameters(Utils.Sequence(0, 7, i => new NpgsqlParameter<int>($"s{i}", values[i])));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a monster's item vulnerability to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="monsterId">The ID of the monster that the item vulnerability belongs to.</param>
        /// <param name="game">The game that it applies to.</param>
        /// <param name="itemName">The name of the item.</param>
        /// <param name="values">A list of length 3, representing the values for each of the 3 columns.</param>
        public void AddItemEffect(int monsterId, Game game, string itemName, IList<int> values)
        {
            AssertLength(values, 3);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO item_effects (monster_id, game, name, normal, enraged, fatigued)
                    VALUES (@id, @game, @name, @ie0, @ie1, @ie2)
                    ON CONFLICT (monster_id, game, name)
                    DO UPDATE
                    SET normal   = excluded.normal,
                        enraged  = excluded.enraged,
                        fatigued = excluded.fatigued;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", monsterId),
                    new NpgsqlParameter<Game>("game", game),
                    new NpgsqlParameter<string>("name", itemName));

                cmd.AddParameters(Utils.Sequence(0, 3, i => new NpgsqlParameter<int>($"ie{i}", values[i])));

                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Weapon Methods

        /// <summary>
        /// Adds an MH4U weapon to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="name">The weapon's name.</param>
        /// <param name="type">The weapon's type.</param>
        /// <param name="rare">The weapon's rarity level.</param>
        /// <param name="url">The URL of the weapon's Kiranico page.</param>
        /// <param name="parentId">The database id of the weapon's parent, or null if there is none.</param>
        /// <returns>The weapon's database id, to be used as a foreign key.</returns>
        public int AddWeapon4U(string name, WeaponType type, string rare, string url, int? parentId)
            => AddWeaponCommon(Game.Four, name, type, rare, url, parentId);

        /// <summary>
        /// Adds an MHGen weapon to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="name">The weapon's name before being fully upgraded.</param>
        /// <param name="type">The weapon's type.</param>
        /// <param name="rare">The weapon's rarity level.</param>
        /// <param name="url">The URL of the weapon's Kiranico page.</param>
        /// <param name="parentId">The database id of the weapon's parent, or null if there is none.</param>
        /// <param name="finalName">The weapon's name when fully upgraded.</param>
        /// <param name="description">The weapon's description before being fully upgraded.</param>
        /// <param name="finalDescription">The weapon's description when fully upgraded.</param>
        /// <returns>The weapon's database id, to be used as a foreign key.</returns>
        public int AddWeaponGen(string name, WeaponType type, string rare, string url, int? parentId, string finalName, string description, string finalDescription)
        {
            var commonId = AddWeaponCommon(Game.Generations, name, type, rare, url, parentId);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO weapons_gen (common_id, final_name, description, final_description)
                    VALUES (@id, @fname, @desc, @fdesc)
                    ON CONFLICT (common_id)
                    DO UPDATE
                    SET final_name        = excluded.final_name,
                        description       = excluded.description,
                        final_description = excluded.final_description;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", commonId),
                    new NpgsqlParameter<string>("fname", finalName),
                    new NpgsqlParameter<string>("desc", description),
                    new NpgsqlParameter<string>("fdesc", finalDescription));

                cmd.ExecuteNonQuery();
            }

            return commonId;
        }

        /// <summary>
        /// Adds an MHWorld weapon to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="name">The weapon's name.</param>
        /// <param name="type">The weapon's type.</param>
        /// <param name="rare">The weapon's rarity level.</param>
        /// <param name="url">The URL of the weapon's Kiranico page.</param>
        /// <param name="parentId">The database id of the weapon's parent, or null if there is none.</param>
        /// <returns>The weapon's database id, to be used as a foreign key.</returns>
        public int AddWeaponWorld(string name, WeaponType type, string rare, string url, int? parentId)
            => AddWeaponCommon(Game.World, name, type, rare, url, parentId);

        private int AddWeaponCommon(Game game, string name, WeaponType type, string rare, string url, int? parentId)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO weapons_common (game, name, weapon_type, rare, url, parent_id)
                    VALUES (@game, @name, @type, @rare, @url, @parentId)
                    ON CONFLICT (game, name, weapon_type)
                    DO UPDATE
                    SET rare      = excluded.rare,
                        url       = excluded.url,
                        parent_id = excluded.parent_id
                    RETURNING id;";

                cmd.AddParameters(
                    new NpgsqlParameter<Game>("game", game),
                    new NpgsqlParameter<string>("name", name),
                    new NpgsqlParameter<WeaponType>("type", type),
                    new NpgsqlParameter<string>("rare", rare),
                    new NpgsqlParameter<string>("url", url),
                    new NpgsqlParameter("parentId", parentId ?? (object)DBNull.Value));

                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Adds an MHWorld weapon level to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="weaponId">The database id of the weapon that the level belongs to.</param>
        /// <param name="raw">The level's displayed raw damage.</param>
        /// <param name="affinity">The level's affinity.</param>
        /// <param name="defense">The level's defense boost.</param>
        /// <param name="slots">A string representing the level's slots.</param>
        /// <param name="modifier">The multiplier used to convert display raw to true raw.</param>
        /// <param name="frenzyAffinity">The weapon's bonus affinity from frenzy virus.</param>
        /// <returns>The weapon level's database id.</returns>
        public int AddWeaponLevel4U(int weaponId, int raw, int affinity, int defense, string slots, float modifier, int frenzyAffinity)
        {
            var commonId = AddWeaponLevelCommon(weaponId, 1, raw, affinity, defense, slots);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO weapon_levels_4u (common_id, modifier, frenzy_affinity)
                    VALUES (@id, @modif, @aff)
                    ON CONFLICT (common_id)
                    DO UPDATE SET modifier = excluded.modifier, frenzy_affinity = excluded.frenzy_affinity;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", commonId),
                    new NpgsqlParameter<float>("modif", modifier),
                    new NpgsqlParameter<int>("aff", frenzyAffinity));

                cmd.ExecuteNonQuery();
            }

            return commonId;
        }

        /// <summary>
        /// Adds an MHGen weapon level to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="weaponId">The database id of the weapon that the level belongs to.</param>
        /// <param name="ordinal">The weapon level's ordinal, used for ordering purposes.</param>
        /// <param name="raw">The level's raw damage.</param>
        /// <param name="affinity">The level's affinity.</param>
        /// <param name="defense">The level's defense boost.</param>
        /// <param name="slots">A string representing the level's slots.</param>
        /// <returns>The weapon level's database id.</returns>
        public int AddWeaponLevelGen(int weaponId, int ordinal, int raw, int affinity, int defense, string slots)
            => AddWeaponLevelCommon(weaponId, ordinal, raw, affinity, defense, slots);

        /// <summary>
        /// Adds an MHWorld weapon level to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="weaponId">The database id of the weapon that the level belongs to.</param>
        /// <param name="raw">The level's displayed raw damage.</param>
        /// <param name="affinity">The level's affinity.</param>
        /// <param name="defense">The level's defense boost.</param>
        /// <param name="slots">A string representing the level's slots.</param>
        /// <param name="modifier">The multiplier used to convert display raw to true raw.</param>
        /// <returns>The weapon level's database id.</returns>
        public int AddWeaponLevelWorld(int weaponId, int raw, int affinity, int defense, string slots, float modifier)
        {
            var commonId = AddWeaponLevelCommon(weaponId, 1, raw, affinity, defense, slots);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO weapon_levels_world (common_id, modifier)
                    VALUES (@id, @modif)
                    ON CONFLICT (common_id)
                    DO UPDATE SET modifier = excluded.modifier;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", commonId),
                    new NpgsqlParameter<float>("modif", modifier));

                cmd.ExecuteNonQuery();
            }

            return commonId;
        }

        private int AddWeaponLevelCommon(int weaponId, int ordinal, int raw, int affinity, int defense, string slots)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO weapon_levels_common (weapon_id, level_ordinal, raw, affinity, defense, slots)
                    VALUES (@id, @ord, @raw, @aff, @def, @slots)
                    ON CONFLICT (weapon_id, level_ordinal)
                    DO UPDATE
                    SET raw              = excluded.raw,
                        affinity         = excluded.affinity,
                        defense          = excluded.defense,
                        slots            = excluded.slots
                    RETURNING id;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", weaponId),
                    new NpgsqlParameter<int>("ord", ordinal),
                    new NpgsqlParameter<int>("raw", raw),
                    new NpgsqlParameter<int>("aff", affinity),
                    new NpgsqlParameter<int>("def", defense),
                    new NpgsqlParameter<string>("slots", slots));

                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Adds a weapon effect to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="levelId">The database id of the weapon level the effect belongs to.</param>
        /// <param name="effect">The weapon's effect type.</param>
        /// <param name="attack">The effect's attack value.</param>
        /// <param name="needsAwaken">Whether or not the effect needs awakening to be active.</param>
        public void AddWeaponEffect(int levelId, WeaponEffect effect, int attack, bool needsAwaken)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO weapon_effects (weapon_level_id, effect_type, attack, needs_awaken)
                    VALUES (@id, @type, @attack, @awaken)
                    ON CONFLICT (weapon_level_id, effect_type)
                    DO UPDATE SET attack = excluded.attack, needs_awaken = excluded.needs_awaken;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", levelId),
                    new NpgsqlParameter<WeaponEffect>("type", effect),
                    new NpgsqlParameter<int>("attack", attack),
                    new NpgsqlParameter<bool>("awaken", needsAwaken));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a weapon sharpness to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="levelId">The database id of the weapon level the sharpness belongs to.</param>
        /// <param name="ordinal">The sharpness' ordinal, used for ordering purposes.</param>
        /// <param name="values">A list with a length of 7, representing the values of each sharpness colour.</param>
        public void AddWeaponSharpness(int levelId, int ordinal, IList<int> values)
        {
            AssertLength(values, 7);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO weapon_sharpnesses (weapon_level_id, sharpness_ordinal, red, orange, yellow, green, blue, white, purple)
                    VALUES (@id, @ord, @s0, @s1, @s2, @s3, @s4, @s5, @s6)
                    ON CONFLICT (weapon_level_id, sharpness_ordinal)
                    DO UPDATE
                    SET red    = excluded.red,
                        orange = excluded.orange,
                        yellow = excluded.yellow,
                        green  = excluded.green,
                        blue   = excluded.blue,
                        white  = excluded.white,
                        purple = excluded.purple;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", levelId),
                    new NpgsqlParameter<int>("ord", ordinal));

                cmd.AddParameters(Utils.Sequence(0, 7, i => new NpgsqlParameter<int>($"s{i}", values[i])));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a set of horn notes to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="levelId">The database id of the weapon level it belongs to.</param>
        /// <param name="values">A list of length 3, representing the 3 notes.</param>
        public void AddHornNotes(int levelId, IList<HornNote> values)
        {
            AssertLength(values, 3);

            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO horn_notes (weapon_level_id, note_1, note_2, note_3)
                    VALUES (@id, @n0, @n1, @n2)
                    ON CONFLICT (weapon_level_id)
                    DO UPDATE
                    SET note_1 = excluded.note_1,
                        note_2 = excluded.note_2,
                        note_3 = excluded.note_3;";

                cmd.Parameters.Add(new NpgsqlParameter<int>("id", levelId));

                cmd.AddParameters(Utils.Sequence(0, 3, i => new NpgsqlParameter<HornNote>($"n{i}", values[i])));
            }
        }

        /// <summary>
        /// Adds a gunlance shell to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="levelId">The database id of the level it belongs to.</param>
        /// <param name="shellType">The shell's type (Normal, Long, or Wide).</param>
        /// <param name="shellLevel">The shell's level.</param>
        public void AddGunlanceShells(int levelId, string shellType, int shellLevel)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO gunlance_shells (weapon_level_id, shell_type, shell_level)
                    VALUES (@id, @type, @level)
                    ON CONFLICT (weapon_level_id)
                    DO UPDATE SET shell_type = excluded.shell_type, shell_level = excluded.shell_level;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", levelId),
                    new NpgsqlParameter<string>("type", shellType),
                    new NpgsqlParameter<int>("level", shellLevel));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a switch axe/charge blade phial to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="levelId">The database id of the weapon level it belongs to.</param>
        /// <param name="phialType">The phial's damage type.</param>
        /// <param name="phialValue">The phial's damage value, if applicable.</param>
        public void AddPhials(int levelId, string phialType, int? phialValue)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO phials (weapon_level_id, phial_type, phial_value)
                    VALUES (@id, @type, @value)
                    ON CONFLICT (weapon_level_id)
                    DO UPDATE SET phial_type = excluded.phial_type, phial_value = excluded.phial_value;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", levelId),
                    new NpgsqlParameter<string>("type", phialType),
                    new NpgsqlParameter("value", phialValue ?? (object)DBNull.Value));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a set of bow stats to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="levelId">The database id of the level that it belongs to.</param>
        /// <param name="arcShot">The bow's arc shot type (for 4U, Power counts as an arc shot).</param>
        /// <param name="chargeShots">The bow's charged shot levels.</param>
        /// <param name="coatings">The coatings usable by the bow.</param>
        public void AddBowStats(int levelId, string arcShot, string[] chargeShots, string[] coatings)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO bow_stats (weapon_level_id, arc_shot, charge_shots, coatings)
                    VALUES (@id, @arc, @charge, @coat)
                    ON CONFLICT (weapon_level_id)
                    DO UPDATE
                    SET arc_shot     = excluded.arc_shot,
                        charge_shots = excluded.charge_shots,
                        coatings     = excluded.coatings;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", levelId),
                    new NpgsqlParameter<string>("arc", arcShot),
                    new NpgsqlParameter<string[]>("charge", chargeShots),
                    new NpgsqlParameter<string[]>("coat", coatings));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a set of gun stats to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="levelId">The database id of the weapon level that it belongs to.</param>
        /// <param name="values">A list of length 3, representing the reload speed, recoil, and deviation.</param>
        /// <returns>The gun stats' database id, to be used as a foreign key.</returns>
        public int AddGunStats(int levelId, IList<string> values)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO gun_stats (weapon_level_id, reload_speed, recoil, deviation)
                    VALUES (@id, @g0, @g1, @g2)
                    ON CONFLICT (weapon_level_id)
                    DO UPDATE
                    SET reload_speed = excluded.reload_speed,
                        recoil = excluded.recoil,
                        deviation = excluded.deviation
                    RETURNING id;";

                cmd.Parameters.Add(new NpgsqlParameter<int>("id", levelId));

                cmd.AddParameters(Utils.Sequence(0, 3, i => new NpgsqlParameter<string>($"g{i}", values[i])));

                return (int)cmd.ExecuteScalar();
            }
        }

        /// <summary>
        /// Adds a gun shot to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="gunStatsId">The database id of the gun stats that the shot belongs to.</param>
        /// <param name="name">The shot's name.</param>
        /// <param name="clipSize">The shot's clip size.</param>
        /// <param name="needsSkill">Whether the shot can be used by default or requires an armour skill.</param>
        public void AddGunShot(int gunStatsId, string name, int clipSize, bool needsSkill)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO gun_shots (gun_stats_id, name, clip_size, needs_skill)
                    VALUES (@id, @name, @clip, @skill)
                    ON CONFLICT (gun_stats_id, name)
                    DO UPDATE SET clip_size = excluded.clip_size, needs_skill = excluded.needs_skill;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", gunStatsId),
                    new NpgsqlParameter<string>("name", name),
                    new NpgsqlParameter<int>("clip", clipSize),
                    new NpgsqlParameter<bool>("skill", needsSkill));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a gun internal shot to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="gunStatsId">The database id of the gun stats that the shot belongs to.</param>
        /// <param name="name">The shot's name.</param>
        /// <param name="capacity">The number of bullets that are given at the start of the quest.</param>
        /// <param name="clipSize">The shot's clip size.</param>
        public void AddGunInternalShot(int gunStatsId, string name, int capacity, int clipSize)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO gun_internal_shots (gun_stats_id, name, capacity, clip_size)
                    VALUES (@id, @name, @count, @clip)
                    ON CONFLICT (gun_stats_id, name)
                    DO UPDATE SET capacity = excluded.capacity, clip_size = excluded.clip_size;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", gunStatsId),
                    new NpgsqlParameter<string>("name", name),
                    new NpgsqlParameter<int>("count", capacity),
                    new NpgsqlParameter<int>("clip", clipSize));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a gun rapid fire shot to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="gunStatsId">The database id of the gun stats that the shot belongs to.</param>
        /// <param name="name">The shot's name/</param>
        /// <param name="shotCount">The number of shots per volley.</param>
        /// <param name="modifier">The damage modifier applied to each shot.</param>
        /// <param name="waitTime">THe wait time between volleys.</param>
        public void AddGunRapidFireShot(int gunStatsId, string name, int shotCount, float? modifier = null, string waitTime = null)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO gun_rapid_fire_shots (gun_stats_id, name, shot_count, modifier, wait_time)
                    VALUES (@id, @name, @count, @modif, @wait)
                    ON CONFLICT (gun_stats_id, name)
                    DO UPDATE
                    SET shot_count = excluded.shot_count,
                        modifier   = excluded.modifier,
                        wait_time  = excluded.wait_time;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", gunStatsId),
                    new NpgsqlParameter<string>("name", name),
                    new NpgsqlParameter<int>("count", shotCount),
                    new NpgsqlParameter("modif", modifier ?? (object)DBNull.Value),
                    new NpgsqlParameter("wait", waitTime ?? (object)DBNull.Value));

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Adds a gun crouching fire shot to the database, replacing it if it already exists.
        /// </summary>
        /// <param name="gunStatsId">The database id of the gun stats that this shot belongs to.</param>
        /// <param name="name">The shot's name.</param>
        /// <param name="clipSize">The shot's clip size.</param>
        public void AddGunCrouchingFireShot(int gunStatsId, string name, int clipSize)
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = @"
                    INSERT INTO gun_crouching_fire_shots (gun_stats_id, name, clip_size)
                    VALUES (@id, @name, @clip)
                    ON CONFLICT (gun_stats_id, name)
                    DO UPDATE SET clip_size = excluded.clip_size;";

                cmd.AddParameters(
                    new NpgsqlParameter<int>("id", gunStatsId),
                    new NpgsqlParameter<string>("name", name),
                    new NpgsqlParameter<int>("clip", clipSize));

                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        /// <summary>
        /// Ensure that a list has the specified length and throw an exception if it doesn't.
        /// </summary>
        /// <param name="collection">The collection to check.</param>
        /// <param name="expectedLength">The length to check against.</param>
        /// <exception cref="ArgumentException">Thrown if the length of <paramref name="collection"/> is not equal to <paramref name="expectedLength"/>.</exception>
        private void AssertLength<T>(IList<T> collection, int expectedLength)
        {
            if (collection.Count != expectedLength)
            {
                throw new ArgumentException($"List has length of {collection.Count}, expected {expectedLength}");
            }
        }

        private void CreateTables()
        {
            using (var cmd = _conn.CreateCommand())
            {
                cmd.CommandText = File.ReadAllText(@"Data\CreateTables.sql", new UTF8Encoding(false));
                cmd.ExecuteNonQuery();
            }
        }

        public void Dispose()
        {
            _conn.Dispose();
        }
    }
}
