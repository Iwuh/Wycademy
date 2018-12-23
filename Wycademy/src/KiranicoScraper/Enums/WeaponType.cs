using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Text;

namespace KiranicoScraper.Enums
{
    enum WeaponType
    {
        GS,
        LS,
        [PgName("sns")]
        SnS,
        DB,
        Hammer,
        HH,
        Lance,
        GL,
        SA,
        IG,
        CB,
        LBG,
        HBG,
        Bow
    }
}
