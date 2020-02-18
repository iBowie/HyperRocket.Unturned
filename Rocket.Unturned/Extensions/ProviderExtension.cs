using Rocket.Unturned.Player;
using SDG.Unturned;
using System;
using System.Collections.Generic;

namespace Rocket.Unturned.Extensions
{
    public static class ProviderExtension
    {
        public static IEnumerable<UnturnedPlayer> GetUnturnedPlayers()
        {
            foreach (SteamPlayer sp in Provider.clients)
            {
                if (sp == null || sp.player == null)
                {
                    continue;
                }

                UnturnedPlayer up = UnturnedPlayer.FromSteamPlayer(sp);
                yield return up;
            }
        }
        public static IEnumerable<UnturnedPlayer> GetUnturnedPlayers(Func<UnturnedPlayer, bool> filter)
        {
            if (filter == null)
            {
                throw new ArgumentNullException("filter");
            }

            foreach (SteamPlayer sp in Provider.clients)
            {
                if (sp == null || sp.player == null)
                {
                    continue;
                }

                UnturnedPlayer up = UnturnedPlayer.FromSteamPlayer(sp);
                if (filter.Invoke(up))
                {
                    yield return up;
                }
            }
        }
    }
}
