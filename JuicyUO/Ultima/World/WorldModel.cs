﻿#region license
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

#region usings
using System;
using System.Collections.Generic;
using JuicyUO.Core;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.Network;
using JuicyUO.Core.Patterns.MVC;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.Login;
using JuicyUO.Ultima.UI;
using JuicyUO.Ultima.UI.WorldGumps;
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Ultima.World.Input;
using JuicyUO.Ultima.World.Managers;
using JuicyUO.Ultima.World.Maps;
using JuicyUO.Configuration.Properties;
#endregion

namespace JuicyUO.Ultima.World
{
    class WorldModel : AModel
    {
        // ============================================================================================================
        // Private variables
        // ============================================================================================================
        Map m_Map;
        WorldCursor m_Cursor;
        readonly INetworkClient m_Network;
        readonly UserInterfaceService m_UserInterface;
        readonly UltimaGame m_Engine;

        // ============================================================================================================
        // Public Static Properties
        // ============================================================================================================
        public static Serial PlayerSerial
        {
            get;
            set;
        }

        public static EntityManager Entities
        {
            get;
            private set;
        }

        public static EffectManager Effects
        {
            get;
            private set;
        }

        public static StaticManager Statics
        {
            get;
            private set;
        }

        // ============================================================================================================
        // Public Properties
        // ============================================================================================================
        public WorldClient Client
        {
            get;
            private set;
        }

        public WorldInput Input
        {
            get;
            private set;
        }

        public WorldInteraction Interaction
        {
            get;
            private set;
        }
        
        public WorldCursor Cursor
        {
            get { return m_Cursor; }
            set
            {
                if (m_Cursor != null)
                {
                    m_Cursor.Dispose();
                }
                m_Cursor = value;
            }
        }
        
        public Map Map
        {
            get { return m_Map; }
        }

        public uint MapIndex
        {
            get
            {
                return (m_Map == null) ? 0xFFFFFFFF : m_Map.Index;
            }
            set
            {
                if (value != MapIndex)
                {
                    // clear all entities
                    Entities.Reset(false);
                    if (m_Map != null)
                    {
                        AEntity player = Entities.GetPlayerEntity();
                        // save current player position
                        int x = player.X, y = player.Y, z = player.Z;
                        // place the player in null space (allows the map to be reloaded when we return to the same location in a different map).
                        player.SetMap(null);
                        // dispose of map
                        m_Map.Dispose();
                        m_Map = null;
                        // add new map!
                        m_Map = new Map(value);
                        player.SetMap(m_Map);
                        // restore previous player position
                        player.Position.Set(x, y, z);
                    }
                    else
                    {
                        AEntity player = Entities.GetPlayerEntity();
                        m_Map = new Map(value);
                        player.SetMap(m_Map);
                    }
                }
            }
        }

        public static bool IsInWorld // InWorld allows us to tell when our character object has been loaded in the world.
        {
            get;
            set;
        }

        // ============================================================================================================
        // Ctor, Initialization, Dispose, Update
        // ============================================================================================================
        public WorldModel()
        {
            Service.Add<WorldModel>(this);

            m_Engine = Service.Get<UltimaGame>();
            m_Network = Service.Get<INetworkClient>();
            m_UserInterface = Service.Get<UserInterfaceService>();

            Entities = new EntityManager(this);
            Entities.Reset(true);
            Effects = new EffectManager(this);
            Statics = new StaticManager();

            Input = new WorldInput(this);
            Interaction = new WorldInteraction(this);
            Client = new WorldClient(this);
        }

        protected override void OnInitialize()
        {
            m_Engine.SetupWindowForWorld();
            m_UserInterface.Cursor = Cursor = new WorldCursor(this);
            Client.Initialize();
            Player.PlayerState.Journaling.AddEntry("Welcome to Ultima Online!", 9, 0x3B4, string.Empty, false);
        }

        protected override void OnDispose()
        {
            SaveOpenGumps();
            m_Engine.SaveResolution();

            Service.Remove<WorldModel>();

            m_UserInterface.Reset();

            Entities.Reset();
            Entities = null;

            Effects = null;

            Input.Dispose();
            Input = null;

            Interaction = null;

            Client.Dispose();
            Client = null;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (!m_Network.IsConnected)
            {
                if (m_UserInterface.IsModalControlOpen == false)
                {
                    MsgBoxGump g = MsgBoxGump.Show("You have lost your connection with the server.", MsgBoxTypes.OkOnly);
                    g.OnClose = OnCloseLostConnectionMsgBox;
                }
            }
            else
            {
                Input.Update(frameMS);
                Entities.Update(frameMS);
                Effects.Update(frameMS);
                Statics.Update(frameMS);
            }
        }

        // ============================================================================================================
        // Public Methods
        // ============================================================================================================
        public void LoginToWorld()
        {
            m_UserInterface.AddControl(new WorldViewGump(), 0, 0); // world gump will restore its position on load.
            if (!Settings.UserInterface.MenuBarDisabled)
            {
                m_UserInterface.AddControl(new TopMenuGump(), 0, 0); // by default at the top of the screen.
            }

            Client.SendWorldLoginPackets();
            IsInWorld = true;
            Client.StartKeepAlivePackets();
            
            // wait until we've received information about the entities around us before restoring saved gumps.
            DelayedAction.Start(RestoreSavedGumps, 1000);
        }

        public void Disconnect()
        {
            m_Network.Disconnect(); // stops keep alive packets.
            IsInWorld = false;
            m_Engine.Models.Current = new LoginModel();
        }

        // ============================================================================================================
        // Private/Protected Methods
        // ============================================================================================================
        protected override AView CreateView()
        {
            return new WorldView(this);
        }

        void OnCloseLostConnectionMsgBox()
        {
            Disconnect();
        }

        void SaveOpenGumps()
        {
            Settings.Gumps.SavedGumps.Clear();
            foreach (AControl gump in m_UserInterface.OpenControls)
            {
                if (gump is Gump)
                {
                    if ((gump as Gump).SaveOnWorldStop)
                    {
                        Dictionary<string, object> data;
                        if ((gump as Gump).SaveGump(out data))
                        {
                            Settings.Gumps.SavedGumps.Add(new SavedGumpProperty(gump.GetType(), data));
                        }
                    }
                }
            }
        }

        void RestoreSavedGumps()
        {
            foreach (SavedGumpProperty savedGump in Settings.Gumps.SavedGumps)
            {
                try
                {
                    Type type = Type.GetType(savedGump.GumpType);
                    object gump = System.Activator.CreateInstance(type);
                    if (gump is Gump)
                    {
                        if ((gump as Gump).RestoreGump(savedGump.GumpData))
                        {
                            m_UserInterface.AddControl(gump as Gump, 0, 0);
                        }
                        else
                        {
                            Tracer.Error("Unable to restore saved gump with type {0}: Failed to restore gump.", savedGump.GumpType);
                        }
                    }
                    else
                    {
                        Tracer.Error("Unable to restore saved gump with type {0}: Type does not derive from Gump.", savedGump.GumpType);
                    }
                }
                catch
                {
                    Tracer.Error("Unable to restore saved gump with type {0}: Type cannot be Instanced.", savedGump.GumpType);
                }
            }
            Settings.Gumps.SavedGumps.Clear();
        }
    }
}
