using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WycademyV2.Commands.Utilities
{
    public class UserEqualityComparer : IEqualityComparer<IUser>
    {
        public bool Equals(IUser x, IUser y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(IUser obj)
        {
            return obj.Id.GetHashCode();
        }
    }
}
