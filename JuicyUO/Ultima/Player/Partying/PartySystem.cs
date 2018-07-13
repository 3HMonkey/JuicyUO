#region license
//  Copyright (C) 2018 JuicyUO Development Community on Github
//
//	This project is an alternative client for the game Ultima Online.
//	The goal of this is to develop a lightweight client considering 
//	new technologies such as DirectX (MonoGame included). The foundation
//	is originally licensed (GNU) on JuicyUO and the JuicyUO Development
//	Team. (Copyright (c) 2015 JuicyUO Development Team)
//    
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <https://www.gnu.org/licenses/>.
#endregion

using System.Collections.Generic;
using JuicyUO.Core.Network;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.Network.Client;
using JuicyUO.Ultima.Network.Client.Partying;
using JuicyUO.Ultima.Network.Server.GeneralInfo;
using JuicyUO.Ultima.UI;
using JuicyUO.Ultima.UI.WorldGumps;
using JuicyUO.Ultima.World;

namespace JuicyUO.Ultima.Player.Partying
{
    public class PartySystem
    {
        Serial m_LeaderSerial;
        Serial m_InvitingPartyLeader;
        List<PartyMember> m_PartyMembers = new List<PartyMember>();
        bool m_AllowPartyLoot;

        public Serial LeaderSerial => m_LeaderSerial;
        public List<PartyMember> Members => m_PartyMembers;
        public bool InParty => m_PartyMembers.Count > 1;
        public bool PlayerIsLeader => InParty && PlayerState.Partying.LeaderSerial == WorldModel.PlayerSerial;

        public bool AllowPartyLoot
        {
            get
            {
                return m_AllowPartyLoot;
            }
            set
            {
                m_AllowPartyLoot = value;
                INetworkClient network = Service.Get<INetworkClient>();
                network.Send(new PartyCanLootPacket(m_AllowPartyLoot));
            }
        }

        public void ReceivePartyMemberList(PartyMemberListInfo info)
        {
            bool wasInParty = InParty;
            m_PartyMembers.Clear();
            for (int i = 0; i < info.Count; i++)
                AddMember(info.Serials[i]);
            RefreshPartyGumps();
            if (InParty && !wasInParty)
            {
                AllowPartyLoot = m_AllowPartyLoot;
            }
        }

        public void ReceiveRemovePartyMember(PartyRemoveMemberInfo info)
        {
            m_PartyMembers.Clear();
            for (int i = 0; i < info.Count; i++)
                AddMember(info.Serials[i]);
            RefreshPartyGumps();
        }

        public void ReceiveInvitation(PartyInvitationInfo info)
        {
            m_InvitingPartyLeader = info.PartyLeaderSerial;
        }

        public void AddMember(Serial serial)
        {
            int index = m_PartyMembers.FindIndex(p => p.Serial == serial);
            if (index != -1)
            {
                m_PartyMembers.RemoveAt(index);
            }
            m_PartyMembers.Add(new PartyMember(serial));
            INetworkClient network = Service.Get<INetworkClient>();
            network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, serial));
        }

        public PartyMember GetMember(int index)
        {
            if (index >= 0 && index < m_PartyMembers.Count)
                return m_PartyMembers[index];
            return null;
        }

        public PartyMember GetMember(Serial serial)
        {
            return m_PartyMembers.Find(p => p.Serial == serial);
        }

        public void LeaveParty()
        {
            INetworkClient network = Service.Get<INetworkClient>();
            network.Send(new PartyLeavePacket());
            m_PartyMembers.Clear();
            m_LeaderSerial = Serial.Null;
            UserInterfaceService ui = Service.Get<UserInterfaceService>();
            ui.RemoveControl<PartyHealthTrackerGump>();
        }

        public void DoPartyCommand(string text)
        {
            // I do this a little differently than the legacy client. With legacy, if you type "/add this other player,
            // please ?" the client will detect the first word is "add" and request an add player target. Instead, I
            // interpret this as a message, and send the message "add this other player, please?" as a party message.
            INetworkClient network = Service.Get<INetworkClient>();
            WorldModel world = Service.Get<WorldModel>();
            string command = text.ToLower();
            bool commandHandled = false;
            switch (command)
            {
                case "help":
                    ShowPartyHelp();
                    commandHandled = true;
                    break;
                case "add":
                    RequestAddPartyMemberTarget();
                    commandHandled = true;
                    break;
                case "rem":
                case "remove":
                    if (InParty && PlayerIsLeader)
                    {
                        world.Interaction.ChatMessage("Who would you like to remove from your party?", 3, 10, false);
                        network.Send(new PartyRequestRemoveTargetPacket());
                    }
                    commandHandled = true;
                    break;
                case "accept":
                    if (!InParty && m_InvitingPartyLeader.IsValid)
                    {
                        network.Send(new PartyAcceptPacket(m_InvitingPartyLeader));
                        m_LeaderSerial = m_InvitingPartyLeader;
                        m_InvitingPartyLeader = Serial.Null;
                    }
                    commandHandled = true;
                    break;
                case "decline":
                    if (!InParty && m_InvitingPartyLeader.IsValid)
                    {
                        network.Send(new PartyDeclinePacket(m_InvitingPartyLeader));
                        m_InvitingPartyLeader = Serial.Null;
                    }
                    commandHandled = true;
                    break;
                case "quit":
                    LeaveParty();
                    commandHandled = true;
                    break;
            }
            if (!commandHandled)
            {
                network.Send(new PartyPublicMessagePacket(text));
            }
        }

        internal void BeginPrivateMessage(int serial)
        {
            PartyMember member = GetMember((Serial)serial);
            if (member != null)
            {
                ChatControl chat = Service.Get<ChatControl>();
                chat.SetModeToPartyPrivate(member.Name, member.Serial);
            }
        }

        public void SendPartyPrivateMessage(Serial serial, string text)
        {
            WorldModel world = Service.Get<WorldModel>();
            PartyMember recipient = GetMember(serial);
            if (recipient != null)
            {
                INetworkClient network = Service.Get<INetworkClient>();
                network.Send(new PartyPrivateMessagePacket(serial, text));
                world.Interaction.ChatMessage($"To {recipient.Name}: {text}", 3, Settings.UserInterface.PartyPrivateMsgColor, false);
            }
            else
            {
                world.Interaction.ChatMessage("They are no longer in your party.", 3, Settings.UserInterface.PartyPrivateMsgColor, false);
            }
        }

        internal void RequestAddPartyMemberTarget()
        {
            INetworkClient network = Service.Get<INetworkClient>();
            WorldModel world = Service.Get<WorldModel>();
            if (!InParty)
            {
                m_LeaderSerial = WorldModel.PlayerSerial;
                network.Send(new PartyRequestAddTargetPacket());
            }
            else if (InParty && PlayerIsLeader)
            {
                network.Send(new PartyRequestAddTargetPacket());
            }
            else if (InParty && !PlayerIsLeader)
            {
                world.Interaction.ChatMessage("You may only add members to the party if you are the leader.", 3, 10, false);
            }
        }

        public void RefreshPartyGumps()
        {
            UserInterfaceService ui = Service.Get<UserInterfaceService>();
            ui.RemoveControl<PartyHealthTrackerGump>();
            for (int i = 0; i < Members.Count; i++)
            {
                ui.AddControl(new PartyHealthTrackerGump(Members[i]), 5, 40 + (48 * i));
            }
            Gump gump;
            if ((gump = ui.GetControl<PartyGump>()) != null)
            {
                int x = gump.X;
                int y = gump.Y;
                ui.RemoveControl<PartyGump>();
                ui.AddControl(new PartyGump(), x, y);
            }
        }

        public void RemoveMember(Serial serial)
        {
            INetworkClient network = Service.Get<INetworkClient>();
            network.Send(new PartyRemoveMemberPacket(serial));
            int index = m_PartyMembers.FindIndex(p => p.Serial == serial);
            if (index != -1)
            {
                m_PartyMembers.RemoveAt(index);
            }
        }

        public void ShowPartyHelp()
        {
            WorldModel m_world = Service.Get<WorldModel>();
            m_world.Interaction.ChatMessage("/add       - add a new member or create a party", 3, 51, true);
            m_world.Interaction.ChatMessage("/rem       - kick a member from your party", 3, 51, true);
            m_world.Interaction.ChatMessage("/accept    - join a party", 3, 51, true);
            m_world.Interaction.ChatMessage("/decline   - decline a party invitation", 3, 51, true);
            m_world.Interaction.ChatMessage("/quit      - leave your current party", 3, 51, true);
        }
    }
}