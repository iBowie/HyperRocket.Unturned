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
            get { return color; }
            set { color = value; }
        }


        private bool vanishMode = false;
        public bool VanishMode
        {
            get { return vanishMode; }
            set
            {
                Player.GetComponent<UnturnedPlayerMovement>().VanishMode = value;
                PlayerMovement pMovement = Player.GetComponent<PlayerMovement>();
                pMovement.canAddSimulationResultsToUpdates = !value;
                if (vanishMode && !value)
                {
                    pMovement.updates.Add(new PlayerStateUpdate(pMovement.real, Player.Player.look.angle, Player.Player.look.rot));
                    pMovement.isUpdated = true;
                    PlayerManager.updates++;
                }
                vanishMode = value;
            }
        }

        private bool godMode = false;
        public bool GodMode
        {
            set
            {
                if (value)
                {
                    DamageTool.damagePlayerRequested += DamageTool_damagePlayerRequested;
                }
                else
                {
                    DamageTool.damagePlayerRequested -= DamageTool_damagePlayerRequested;
                }
                godMode = value;
            }
            get
            {
                return godMode;
            }
        }

        private void DamageTool_damagePlayerRequested(ref DamagePlayerParameters parameters, ref bool shouldAllow)
        {
            if (parameters.player == Player.Player && godMode)
            {
                shouldAllow = false;
            }
        }

        private bool initialCheck;

        Vector3 oldPosition = new Vector3();

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
            for (int i = s.Length; i > 0; i--) r += s[i - 1];
            return r;
        }

        protected override void Load()
        {

            if (godMode)
            {
                DamageTool.damagePlayerRequested += DamageTool_damagePlayerRequested;
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
