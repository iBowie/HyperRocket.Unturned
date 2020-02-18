using Rocket.API;
using Rocket.API.Commands;
using Rocket.Core.Extensions;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    public class CommandI : AdvancedRocketCommand
    {
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "i";
        public override string Help => "Gives yourself an item";
        public override string Syntax => "<id> [amount]";
        public override List<string> Aliases => new List<string>() { "item" };
        public override List<string> Permissions => new List<string>() { "rocket.item", "rocket.i" };
        public override void Execute(IRocketPlayer caller, CommandArgs args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (args.Count == 0 || args.Count > 2)
            {
                UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            ushort id;
            byte amount = 1;
            Asset a;

            if (args[0].IsItem(out ItemAsset itemAsset))
            {
                a = itemAsset;
                id = itemAsset.id;
            }
            else
            {
                UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            if (args.Count == 2 && !args[1].IsByte(out amount) || a == null)
            {
                UnturnedChat.Say(player, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            string assetName = ((ItemAsset)a).itemName;

            if (U.Settings.Instance.EnableItemBlacklist && !player.HasPermission("itemblacklist.bypass"))
            {
                if (player.HasPermission("item." + id))
                {
                    UnturnedChat.Say(player, U.Translate("command_i_blacklisted"));
                    return;
                }
            }

            if (U.Settings.Instance.EnableItemSpawnLimit && !player.HasPermission("itemspawnlimit.bypass"))
            {
                if (amount > U.Settings.Instance.MaxSpawnAmount)
                {
                    UnturnedChat.Say(player, U.Translate("command_i_too_much", U.Settings.Instance.MaxSpawnAmount));
                    return;
                }
            }

            if (player.GiveItem(id, amount))
            {
                Logger.Log(U.Translate("command_i_giving_console", player.DisplayName, id, amount));
                UnturnedChat.Say(player, U.Translate("command_i_giving_private", amount, assetName, id));
            }
            else
            {
                UnturnedChat.Say(player, U.Translate("command_i_giving_failed_private", amount, assetName, id));
            }
        }
    }
}
