﻿using Rocket.API;
using Rocket.API.Extensions;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Player;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    public class CommandMore : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Player;
        public string Name => "more";
        public string Help => "Gives more of an item that you have in your hands.";
        public string Syntax => "<amount>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "rocket.more" };
        public void Execute(IRocketPlayer caller, string[] command)
        {
            byte? amount = command.GetByteParameter(0);
            if (command.Length == 0 || command.Length > 1 || amount == null || amount == 0)
            {
                UnturnedChat.Say(caller, U.Translate("command_more_usage"));
                return;
            }

            UnturnedPlayer player = (UnturnedPlayer)caller;
            ushort itemId = player.Player.equipment.itemID;
            if (itemId == 0)
            {
                UnturnedChat.Say(caller, U.Translate("command_more_dequipped"));
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_more_give", amount, itemId));
                player.GiveItem(itemId, (byte)amount);
            }
        }
    }
}
