﻿using Rocket.API;
using Rocket.Core;
using Rocket.Unturned.Player;
using SDG.Unturned;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rocket.Unturned.Commands
{
    public sealed class UnturnedCommands : MonoBehaviour
    {
        private void Awake()
        {
            foreach (Command vanillaCommand in Commander.commands)
            {
                R.Commands.Register(new UnturnedVanillaCommand(vanillaCommand), vanillaCommand.command.ToLower(), Core.Serialization.CommandPriority.Low);
            }
        }

        internal class UnturnedVanillaCommand : IRocketCommand
        {
            public Command command;

            public UnturnedVanillaCommand(Command command)
            {
                this.command = command;
            }

            public List<string> Aliases => new List<string>();

            public AllowedCaller AllowedCaller => AllowedCaller.Both;

            public string Help => command.help;

            public string Name => command.command;

            public List<string> Permissions => new List<string>() { "unturned." + command.command.ToLower() };

            public string Syntax => command.info.Replace("/", " ");

            public void Execute(IRocketPlayer caller, string[] command)
            {
                CSteamID id = CSteamID.Nil;
                if (caller is UnturnedPlayer)
                {
                    id = ((UnturnedPlayer)caller).CSteamID;
                }
                Commander.commands.Where(c => c.command == Name).FirstOrDefault()?.check(id, Name, string.Join("/", command));
            }
        }




    }
}