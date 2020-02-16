using Rocket.API;
using Rocket.API.Commands;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Extensions;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    public class CommandEffect : AdvancedRocketCommand
    {
        public override AllowedCaller AllowedCaller => AllowedCaller.Player;
        public override string Name => "effect";
        public override string Help => "Triggers an effect at your position";
        public override string Syntax => "<id>";
        public override List<string> Permissions => new List<string>() { "rocket.effect" };
        public override void Execute(IRocketPlayer caller, CommandArgs args)
        {
            UnturnedPlayer player = (UnturnedPlayer)caller;
            if (args.Count == 1 && args[0].IsEffect(out var effectAsset))
            {
                player.TriggerEffect(effectAsset.id);
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }
        }
    }
}