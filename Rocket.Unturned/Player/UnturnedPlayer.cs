﻿using Rocket.API;
using Rocket.API.Serialisation;
using Rocket.Core;
using Rocket.Core.Steam;
using Rocket.Unturned.Chat;
using Rocket.Unturned.Events;
using Rocket.Unturned.Skills;
using SDG.Unturned;
using Steamworks;
using System;
using System.Linq;
using UnityEngine;

namespace Rocket.Unturned.Player
{
    public class PlayerIsConsoleException : Exception { }

    public sealed class UnturnedPlayer : IRocketPlayer
    {

        public string Id => CSteamID.ToString();

        public string DisplayName => CharacterName;

        public bool IsAdmin => player.channel.owner.isAdmin;

        public Profile SteamProfile => new Profile(ulong.Parse(CSteamID.ToString()));

        private readonly SDG.Unturned.Player player;
        public SDG.Unturned.Player Player => player;

        public CSteamID CSteamID => player.channel.owner.playerID.steamID;

        public Exception PlayerIsConsoleException;

        private UnturnedPlayer(SteamPlayer player)
        {
            this.player = player.player;
        }

        public Color Color
        {
            get
            {
                if (Features.Color.HasValue)
                {
                    return Features.Color.Value;
                }
                if (IsAdmin && !Provider.hideAdmins)
                {
                    return Palette.ADMIN;
                }

                RocketPermissionsGroup group = R.Permissions.GetGroups(this, false).Where(g => g.Color != null && g.Color != "white").FirstOrDefault();
                string color = "";
                if (group != null)
                {
                    color = group.Color;
                }

                return UnturnedChat.GetColorFromName(color, Palette.COLOR_W);
            }
            set => Features.Color = value;
        }


        private UnturnedPlayer(CSteamID cSteamID)
        {
            if (string.IsNullOrEmpty(cSteamID.ToString()) || cSteamID.ToString() == "0")
            {
                throw new PlayerIsConsoleException();
            }
            else
            {
                player = PlayerTool.getPlayer(cSteamID);
            }
        }

        public float Ping => player.channel.owner.ping;

        public bool Equals(UnturnedPlayer p)
        {
            if (p == null)
            {
                return false;
            }

            return (CSteamID.ToString() == p.CSteamID.ToString());
        }

        public T GetComponent<T>()
        {
            return (T)(object)Player.GetComponent(typeof(T));
        }

        private UnturnedPlayer(SDG.Unturned.Player p)
        {
            player = p;
        }

        public static UnturnedPlayer FromName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            SDG.Unturned.Player p = null;
            if (ulong.TryParse(name, out ulong id) && id > 76561197960265728)
            {
                p = PlayerTool.getPlayer(new CSteamID(id));
            }
            else
            {
                p = PlayerTool.getPlayer(name);
            }
            if (p == null)
            {
                return null;
            }

            return new UnturnedPlayer(p);
        }

        public static UnturnedPlayer FromCSteamID(CSteamID cSteamID)
        {
            if (string.IsNullOrEmpty(cSteamID.ToString()) || cSteamID.ToString() == "0")
            {
                return null;
            }
            else
            {
                return new UnturnedPlayer(cSteamID);
            }
        }

        public static UnturnedPlayer FromPlayer(SDG.Unturned.Player player)
        {
            return new UnturnedPlayer(player.channel.owner);
        }

        public static UnturnedPlayer FromSteamPlayer(SteamPlayer player)
        {
            return new UnturnedPlayer(player);
        }

        public UnturnedPlayerFeatures Features => player.gameObject.transform.GetComponent<UnturnedPlayerFeatures>();

        public UnturnedPlayerEvents Events => player.gameObject.transform.GetComponent<UnturnedPlayerEvents>();

        public override string ToString()
        {
            return CSteamID.ToString();
        }

        public void TriggerEffect(ushort effectID)
        {
            EffectManager.instance.channel.send("tellEffectPoint", CSteamID, ESteamPacket.UPDATE_UNRELIABLE_BUFFER, new object[] { effectID, player.transform.position });
        }

        public string IP
        {
            get
            {
                SteamGameServerNetworking.GetP2PSessionState(CSteamID, out P2PSessionState_t State);
                return Parser.getIPFromUInt32(State.m_nRemoteIP);
            }
        }

        public void MaxSkills()
        {
            PlayerSkills skills = player.skills;

            foreach (Skill skill in skills.skills.SelectMany(s => s))
            {
                skill.level = skill.max;
            }

            skills.askSkills(player.channel.owner.playerID.steamID);
        }

        public string SteamGroupName()
        {
            FriendsGroupID_t id;
            id.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
            return SteamFriends.GetFriendsGroupName(id);
        }

        public int SteamGroupMembersCount()
        {
            FriendsGroupID_t id;
            id.m_FriendsGroupID = (short)SteamGroupID.m_SteamID;
            return SteamFriends.GetFriendsGroupMembersCount(id);
        }

        public SteamPlayer SteamPlayer()
        {
            foreach (SteamPlayer SteamPlayer in Provider.clients)
            {
                if (CSteamID == SteamPlayer.playerID.steamID)
                {
                    return SteamPlayer;
                }
            }
            return null;
        }

        public PlayerClothing Clothing => player.clothing;
        public PlayerCrafting Crafting => player.crafting;
        public PlayerEquipment Equipment => player.equipment;
        public PlayerInput Input => player.input;
        public PlayerInteract Interact => player.interact;
        public PlayerInventory Inventory => player.inventory;
        public PlayerLife Life => player.life;
        public PlayerLook Look => player.look;
        public PlayerQuests Quests => player.quests;
        public PlayerSkills Skills => player.skills;
        public PlayerVoice Voice => player.voice;

        public bool GiveItem(ushort itemId, byte amount)
        {
            return ItemTool.tryForceGiveItem(player, itemId, amount);
        }

        public bool GiveItem(Item item)
        {
            return player.inventory.tryAddItem(item, false);
        }

        public bool GiveVehicle(ushort vehicleId)
        {
            return VehicleTool.giveVehicle(player, vehicleId);
        }

        public CSteamID SteamGroupID => player.channel.owner.playerID.group;

        public void Kick(string reason)
        {
            Provider.kick(CSteamID, reason);
        }

        public void Ban(string reason, uint duration)
        {
            Provider.ban(CSteamID, reason, duration);
        }

        public void Admin(bool admin)
        {
            Admin(admin, null);
        }

        public void Admin(bool admin, UnturnedPlayer issuer)
        {
            if (admin)
            {
                if (issuer == null)
                {
                    SteamAdminlist.admin(CSteamID, new CSteamID(0));
                }
                else
                {
                    SteamAdminlist.admin(CSteamID, issuer.CSteamID);
                }
            }
            else
            {
                SteamAdminlist.unadmin(player.channel.owner.playerID.steamID);
            }
        }

        public void Teleport(UnturnedPlayer target)
        {
            Vector3 d1 = target.player.transform.position;
            Vector3 vector31 = target.player.transform.rotation.eulerAngles;
            Teleport(d1, MeasurementTool.angleToByte(vector31.y));
        }

        public void Teleport(Vector3 position, float rotation)
        {
            if (VanishMode)
            {
                player.channel.send("askTeleport", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
                player.channel.send("askTeleport", ESteamCall.NOT_OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new Vector3(position.y, position.y + 1337, position.z), MeasurementTool.angleToByte(rotation));
                player.channel.send("askTeleport", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
            }
            else
            {
                player.channel.send("askTeleport", ESteamCall.ALL, ESteamPacket.UPDATE_RELIABLE_BUFFER, position, MeasurementTool.angleToByte(rotation));
            }
        }

        public bool VanishMode
        {
            get
            {
                UnturnedPlayerFeatures features = player.GetComponent<UnturnedPlayerFeatures>();
                return features.VanishMode;
            }
            set
            {
                UnturnedPlayerFeatures features = player.GetComponent<UnturnedPlayerFeatures>();
                features.VanishMode = value;
            }
        }

        public bool GodMode
        {
            get
            {
                UnturnedPlayerFeatures features = player.GetComponent<UnturnedPlayerFeatures>();
                return features.GodMode;
            }
            set
            {
                UnturnedPlayerFeatures features = player.GetComponent<UnturnedPlayerFeatures>();
                features.GodMode = value;
            }
        }

        public Vector3 Position => player.transform.position;

        public EPlayerStance Stance => player.stance.stance;

        public float Rotation => player.transform.rotation.eulerAngles.y;

        public bool Teleport(string nodeName)
        {
            Node node = LevelNodes.nodes.Where(n => n.type == ENodeType.LOCATION && ((LocationNode)n).name.ToLower().Contains(nodeName)).FirstOrDefault();
            if (node != null)
            {
                Vector3 c = node.point + new Vector3(0f, 0.5f, 0f);
                player.sendTeleport(c, MeasurementTool.angleToByte(Rotation));
                return true;
            }
            return false;
        }

        public byte Stamina => player.life.stamina;

        public string CharacterName => player.channel.owner.playerID.characterName;

        public string SteamName => player.channel.owner.playerID.playerName;

        public byte Infection
        {
            get => player.life.virus;
            set
            {
                player.life.askDisinfect(100);
                player.life.askInfect(value);
            }
        }

        public uint Experience
        {
            get => player.skills.experience;
            set
            {
                player.skills.channel.send("tellExperience", ESteamCall.SERVER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
                player.skills.channel.send("tellExperience", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, value);
            }
        }

        public int Reputation
        {
            get => player.skills.reputation;
            set => player.skills.askRep(value);
        }

        public byte Health => player.life.health;

        public byte Hunger
        {
            get => player.life.food;
            set
            {
                player.life.askEat(100);
                player.life.askStarve(value);
            }
        }

        public byte Thirst
        {
            get => player.life.water;
            set
            {
                player.life.askDrink(100);
                player.life.askDehydrate(value);
            }
        }

        public bool Broken
        {
            get => player.life.isBroken;
            set
            {
                player.life.tellBroken(Provider.server, value);
                player.life.channel.send("tellBroken", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] { value });
            }
        }
        public bool Bleeding
        {
            get => player.life.isBleeding;
            set
            {
                player.life.tellBleeding(Provider.server, value);
                player.life.channel.send("tellBleeding", ESteamCall.OWNER, ESteamPacket.UPDATE_RELIABLE_BUFFER, new object[] { value });
            }
        }

        public bool Dead => player.life.isDead;

        public void Heal(byte amount)
        {
            Heal(amount, null, null);
        }

        public void Heal(byte amount, bool? bleeding, bool? broken)
        {
            player.life.askHeal(amount, bleeding != null ? bleeding.Value : player.life.isBleeding, broken != null ? broken.Value : player.life.isBroken);
        }

        public void Suicide()
        {
            player.life.askSuicide(player.channel.owner.playerID.steamID);
        }

        public EPlayerKill Damage(byte amount, Vector3 direction, EDeathCause cause, ELimb limb, CSteamID damageDealer)
        {
            player.life.askDamage(amount, direction, cause, limb, damageDealer, out EPlayerKill playerKill);
            return playerKill;
        }

        public bool IsPro => player.channel.owner.isPro;

        public InteractableVehicle CurrentVehicle => player.movement.getVehicle();

        public bool IsInVehicle => CurrentVehicle != null;

        public void SetSkillLevel(UnturnedSkill skill, byte level)
        {
            GetSkill(skill).level = level;
            player.skills.askSkills(CSteamID);
        }

        public byte GetSkillLevel(UnturnedSkill skill)
        {
            return GetSkill(skill).level;
        }

        public Skill GetSkill(UnturnedSkill skill)
        {
            PlayerSkills skills = player.skills;
            return skills.skills[skill.Speciality][skill.Skill];
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(obj);
        }
    }
}
