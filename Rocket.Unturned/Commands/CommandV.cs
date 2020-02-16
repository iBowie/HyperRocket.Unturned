using Rocket.API;
using Rocket.API.Commands;
using Rocket.Core.Logging;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Player;
using SDG.Unturned;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    public class CommandV : AdvancedRocketCommand
    {
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "v";
        public override string Help => "Gives yourself an vehicle";
        public override string Syntax => "<id>";
        public override List<string> Permissions => new List<string>() { "rocket.v", "rocket.vehicle" };
        public override void Execute(IRocketPlayer caller, CommandArgs args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (args.Count != 1)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            if (args[0].IsVehicle(out VehicleAsset vehicleAsset))
            {
                if (U.Settings.Instance.EnableVehicleBlacklist && !player.HasPermission("vehicleblacklist.bypass"))
                {
                    if (player.HasPermission("vehicle." + vehicleAsset.id))
                    {
                        UnturnedChat.Say(caller, U.Translate("command_v_blacklisted"));
                        return;
                    }
                }

                if (VehicleTool.giveVehicle(player.Player, vehicleAsset.id))
                {
                    Logger.Log(U.Translate("command_v_giving_console", player.CharacterName, vehicleAsset.id));
                    UnturnedChat.Say(caller, U.Translate("command_v_giving_private", vehicleAsset.vehicleName, vehicleAsset.id));
                }
                else
                {
                    UnturnedChat.Say(caller, U.Translate("command_v_giving_failed_private", vehicleAsset.vehicleName, vehicleAsset.id));
                }
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
        }
    }
}