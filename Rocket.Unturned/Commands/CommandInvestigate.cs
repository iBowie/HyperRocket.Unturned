﻿using Rocket.API;
using Rocket.Unturned.Chat;
using SDG.Unturned;
using System.Collections.Generic;

namespace Rocket.Unturned.Commands
{
    public class CommandInvestigate : IRocketCommand
    {
        public AllowedCaller AllowedCaller => AllowedCaller.Both;
        public string Name => "investigate";
        public string Help => "Shows you the SteamID64 of a player";
        public string Syntax => "<player>";
        public List<string> Aliases => new List<string>();
        public List<string> Permissions => new List<string>() { "rocket.investigate" };
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length != 1)
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_invalid_parameter"));
                throw new WrongUsageOfCommandException(caller, this);
            }

            SteamPlayer otherPlayer = PlayerTool.getSteamPlayer(command[0]);
            if (otherPlayer != null && (caller == null || otherPlayer.playerID.steamID.ToString() != caller.ToString()))
            {
                UnturnedChat.Say(caller, U.Translate("command_investigate_private", otherPlayer.playerID.characterName, otherPlayer.playerID.steamID.ToString()));
            }
            else
            {
                UnturnedChat.Say(caller, U.Translate("command_generic_failed_find_player"));
                throw new WrongUsageOfCommandException(caller, this);
            }
        }
    }
}