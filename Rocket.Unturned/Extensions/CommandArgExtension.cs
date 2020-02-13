using Rocket.API.Commands;
using Rocket.Core.Extensions;
using Rocket.Unturned.Player;
using Steamworks;
using System.Collections.Generic;
using System.Linq;

namespace Rocket.Unturned.Extensions
{
    public static class CommandArgExtension
    {
        public static bool IsPlayer(this CommandArg arg, out UnturnedPlayer value)
        {
            value = UnturnedPlayer.FromName(arg.RawValue);
            return value != null;
        }
        public static bool IsPlayers(this CommandArg arg, out IEnumerable<UnturnedPlayer> value)
        {
            List<UnturnedPlayer> res = new List<UnturnedPlayer>();
            if (arg.RawValue == "*")
            {

            }
            else if (arg.IsPlayer(out var player))
            {
                res.Add(player);
            }
            value = res;
            return value.Count() > 0;
        }
        public static bool IsCSteamID(this CommandArg arg, out CSteamID value)
        {
            value = default;
            if (arg.IsUInt64(out ulong id))
            {
                value = new CSteamID(id);
                return true;
            }
            else
                return false;
        }
    }
}
