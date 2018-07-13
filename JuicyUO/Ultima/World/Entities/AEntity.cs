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

#region usings
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.World.EntityViews;
using JuicyUO.Ultima.World.Maps;
using System;
using System.Linq;
#endregion

namespace JuicyUO.Ultima.World.Entities
{
    /// <summary>
    /// Base class for all entities which exist in the world model.
    /// </summary>
    public abstract class AEntity
    {
        // ============================================================================================================
        // Properties
        // ============================================================================================================

        public readonly Serial Serial;

        public PropertyList PropertyList = new PropertyList();

        public bool IsDisposed;

        public bool IsClientEntity;

        public virtual int Hue
        {
            set;
            get;
        }

        public virtual string Name
        {
            get
            {
                return "AEntity";
            }
            set
            {
                // do nothing - exists only to allow inheriting class to override.
            }
        }

        // ============================================================================================================
        // Position
        // ============================================================================================================

        public Map Map
        {
            get;
            private set;
        }

        public void SetMap(Map map)
        {
            if (map != Map)
            {
                Map = map;
                Position.Tile = Position3D.NullTile;
            }
        }

        MapTile m_Tile;
        protected MapTile Tile
        {
            set
            {
                if (m_Tile != null)
                    m_Tile.OnExit(this);
                m_Tile = value;

                if (m_Tile != null)
                    m_Tile.OnEnter(this);
                else
                {
                    if (!IsClientEntity)
                        if (!IsDisposed)
                            Dispose();
                }
            }
            get
            {
                return m_Tile;
            }
        }

        protected virtual void OnTileChanged(int x, int y)
        {
            if (Map != null)
            {
                if (IsClientEntity && Map.Index >= 0)
                    Map.CenterPosition = new Point(x, y);
                Tile = Map.GetMapTile(x, y);
            }
            else
            {
                if (!IsClientEntity)
                    Dispose();
            }
        }

        public int X
        {
            get { return Position.X; }
        }

        public int Y
        {
            get { return Position.Y; }
        }

        public int Z
        {
            get { return Position.Z; }
        }

        private Position3D m_Position;
        public virtual Position3D Position { get { return m_Position; } }

        // ============================================================================================================
        // Methods
        // ============================================================================================================

        public AEntity(Serial serial, Map map)
        {
            Serial = serial;
            Map = map;
            m_Position = new Position3D(OnTileChanged);
        }

        public virtual void Update(double frameMS)
        {
            if (IsDisposed)
                return;

            InternalUpdateOverheads(frameMS);
        }

        public virtual void Dispose()
        {
            m_OnDisposed?.Invoke(this);
            m_OnDisposed = null;
            m_OnUpdated = null;
            IsDisposed = true;
            Tile = null;
        }

        public override string ToString()
        {
            return Serial.ToString();
        }

        // ============================================================================================================
        // Callbacks
        // ============================================================================================================

        protected Action<AEntity> m_OnUpdated;
        protected Action<AEntity> m_OnDisposed;

        public void SetCallbacks(Action<AEntity> onUpdate, Action<AEntity> onDispose)
        {
            if (onUpdate != null)
                m_OnUpdated += onUpdate;
            if (onDispose != null)
                m_OnDisposed += onDispose;
        }

        public void ClearCallBacks(Action<AEntity> onUpdate, Action<AEntity> onDispose)
        {
            if (m_OnUpdated.GetInvocationList().Contains(onUpdate))
                m_OnUpdated -= onUpdate;
            if (m_OnDisposed.GetInvocationList().Contains(onDispose))
                m_OnDisposed -= onDispose;
        }

        // ============================================================================================================
        // Draw and View handling code
        // ============================================================================================================

        AEntityView m_View;

        protected virtual AEntityView CreateView()
        {
            return null;
        }

        public AEntityView GetView()
        {
            if (m_View == null)
                m_View = CreateView();
            return m_View;
        }

        internal virtual void Draw(MapTile tile, Position3D position)
        {

        }

        // ============================================================================================================
        // Overhead handling code (labels, chat, etc.)
        // ============================================================================================================

        List<Overhead> m_Overheads = new List<Overhead>();
        public List<Overhead> Overheads
        {
            get { return m_Overheads; }
        }

        public Overhead AddOverhead(MessageTypes msgType, string text, int fontID, int hue, bool asUnicode)
        {
            Overhead overhead;
            text = string.Format("<outline style='font-family: {2}{0};'>{1}", fontID, text, asUnicode ? "uni" : "ascii");

            for (int i = 0; i < m_Overheads.Count; i++)
            {
                overhead = m_Overheads[i];
                // is this overhead an already active label?
                if ((msgType  == MessageTypes.Label) && (overhead.Text == text) && (overhead.MessageType == msgType) && !(overhead.IsDisposed))
                {
                    // reset the timer for the object so it lasts longer.
                    overhead.ResetTimer();
                    // update hue?
                    overhead.Hue = hue;
                    // insert it at the bottom of the queue so it displays closest to the player.
                    m_Overheads.RemoveAt(i);
                    InternalInsertOverhead(overhead);
                    return overhead;
                }
            }

            overhead = new Overhead(this, msgType, text);
            overhead.Hue = hue;
            InternalInsertOverhead(overhead);
            return overhead;
        }

        void InternalInsertOverhead(Overhead overhead)
        {
            if (m_Overheads.Count == 0 || (m_Overheads[0].MessageType != MessageTypes.Label))
                m_Overheads.Insert(0, overhead);
            else
                m_Overheads.Insert(1, overhead);
        }

        internal void InternalDrawOverheads(MapTile tile, Position3D position)
        {
            // base entities do not draw, but they can have overheads, so we draw those.
            foreach (Overhead overhead in m_Overheads)
            {
                if (!overhead.IsDisposed)
                    overhead.Draw(tile, position);
            }
        }

        void InternalUpdateOverheads(double frameMS)
        {
            // update overheads
            foreach (Overhead overhead in m_Overheads)
            {
                overhead.Update(frameMS);
            }
            // remove disposed of overheads.
            for (int i = 0; i < m_Overheads.Count; i++)
            {
                if (m_Overheads[i].IsDisposed)
                {
                    m_Overheads.RemoveAt(i);
                    i--;
                }
            }
        }

        // Update range
        public virtual int GetMaxUpdateRange()
        {
            return 18;
        }
    }
}
