using Rocket.Core.Utils;
using Rocket.Unturned.Events;
using SDG.Unturned;
using System;
using UnityEngine;

namespace Rocket.Unturned.Player
{
    public sealed class UnturnedPlayerFeatures : UnturnedPlayerComponent
    {

        public DateTime Joined = DateTime.Now;

        internal Color? color = null;
        internal Color? Color
        {
            get => color;
            set => color = value;
        }


        private bool vanishMode = false;
        public bool VanishMode
        {
            get => vanishMode;
            set
            {
                Player.GetComponent<UnturnedPlayerMovement>().VanishMode = value;
                PlayerMovement pMovement = Player.GetComponent<PlayerMovement>();
                pMovement.canAddSimulationResultsToUpdates = !value;
                if (vanishMode && !value)
                {
                    pMovement.updates.Add(new PlayerStateUpdate(pMovement.real, Player.Player.look.angle, Player.Player.look.rot));
#pragma warning disable CS0612 // Type or member is obsolete
                    pMovement.isUpdated = true;
                    PlayerManager.updates++;
#pragma warning restore CS0612 // Type or member is obsolete
                }
                vanishMode = value;
            }
        }

        private bool godMode = false;
        public bool GodMode
        {
            get => godMode;
            set
            {
                if (value)
                {
                    DamageTool.damagePlayerRequested += DamageTool_damagePlayerRequested;
                    Player.Events.OnUpdateHealth += e_OnPlayerUpdateHealth;
                    Player.Events.OnUpdateWater += e_OnPlayerUpdateWater;
                    Player.Events.OnUpdateFood += e_OnPlayerUpdateFood;
                    Player.Events.OnUpdateVirus += e_OnPlayerUpdateVirus;
                }
                else
                {
                    DamageTool.damagePlayerRequested -= DamageTool_damagePlayerRequested;
                    Player.Events.OnUpdateHealth -= e_OnPlayerUpdateHealth;
                    Player.Events.OnUpdateWater -= e_OnPlayerUpdateWater;
                    Player.Events.OnUpdateFood -= e_OnPlayerUpdateFood;
                    Player.Events.OnUpdateVirus -= e_OnPlayerUpdateVirus;
                }
                godMode = value;
            }
        }

        private void e_OnPlayerUpdateVirus(UnturnedPlayer player, byte virus)
        {
            if (virus < 95)
            {
                Player.Infection = 0;
            }

            TaskDispatcher.QueueOnMainThread(() =>
            {
                if (virus < 95)
                {
                    Player.Infection = 0;
                }
            }, 0.5f);
        }

        private void e_OnPlayerUpdateFood(UnturnedPlayer player, byte food)
        {
            if (food < 95)
            {
                Player.Hunger = 0;
            }

            TaskDispatcher.QueueOnMainThread(() =>
            {
                if (food < 95)
                {
                    Player.Hunger = 0;
                }
            }, 0.5f);
        }

        private void e_OnPlayerUpdateWater(UnturnedPlayer player, byte water)
        {
            if (water < 95)
            {
                Player.Thirst = 0;
            }

            TaskDispatcher.QueueOnMainThread(() =>
            {
                if (water < 95)
                {
                    Player.Thirst = 0;
                }
            }, 0.5f);
        }

        private void e_OnPlayerUpdateHealth(UnturnedPlayer player, byte health)
        {
            if (health < 95)
            {
                Player.Heal(100);
                Player.Bleeding = false;
                Player.Broken = false;
            }
            TaskDispatcher.QueueOnMainThread(() =>
            {
                if (health < 95)
                {
                    Player.Heal(100);
                    Player.Bleeding = false;
                    Player.Broken = false;
                }
            }, 0.5f);
        }

        private void DamageTool_damagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (parameters.player.channel.owner.playerID.steamID == Player.CSteamID && godMode)
            {
                shouldAllow = false;
            }
        }

        private bool initialCheck;
        private Vector3 oldPosition = new Vector3();

        private void FixedUpdate()
        {
            if (oldPosition != Player.Position)
            {
                UnturnedPlayerEvents.fireOnPlayerUpdatePosition(Player);
                oldPosition = Player.Position;
            }
            if (!initialCheck && (DateTime.Now - Joined).TotalSeconds > 3)
            {
                Check();
            }
        }

        private void Check()
        {
            initialCheck = true;

            if (U.Settings.Instance.CharacterNameValidation)
            {
                string username = Player.CharacterName;
                System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(U.Settings.Instance.CharacterNameValidationRule);
                System.Text.RegularExpressions.Match match = regex.Match(username);
                if (match.Groups[0].Length != username.Length)
                {
                    Provider.kick(Player.CSteamID, U.Translate("invalid_character_name"));
                }
            }
        }

        private static string reverse(string s)
        {
            string r = "";
            for (int i = s.Length; i > 0; i--)
            {
                r += s[i - 1];
            }

            return r;
        }

        protected override void Load()
        {

            if (godMode)
            {
                DamageTool.damagePlayerRequested += DamageTool_damagePlayerRequested;
                Player.Events.OnUpdateHealth += e_OnPlayerUpdateHealth;
                Player.Events.OnUpdateWater += e_OnPlayerUpdateWater;
                Player.Events.OnUpdateFood += e_OnPlayerUpdateFood;
                Player.Events.OnUpdateVirus += e_OnPlayerUpdateVirus;
                Player.Heal(100);
                Player.Infection = 0;
                Player.Hunger = 0;
                Player.Thirst = 0;
                Player.Bleeding = false;
                Player.Broken = false;
            }
        }
    }
}
