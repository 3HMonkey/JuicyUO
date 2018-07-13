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
using JuicyUO.Core.Configuration;
using JuicyUO.Configuration.Properties;
#endregion

namespace JuicyUO.Configuration
{
    public class GumpSettings : ASettingsSection
    {
        /// <summary>
        /// The list of last positions where a given gump type was located.
        /// </summary>
        public Dictionary<string, Point> LastPositions
        {
            get;
            set;
        }

        /// <summary>
        /// A list of saved gumps, and data describing the same. These are reloaded when the world is started.
        /// </summary>
        public List<SavedGumpProperty> SavedGumps
        {
            get;
            set;
        }

        public GumpSettings()
        {
            LastPositions = new Dictionary<string, Point>();
            SavedGumps = new List<SavedGumpProperty>();
        }

        public Point GetLastPosition(string gumpID, Point defaultPosition)
        {
            Point value;
            if (LastPositions.TryGetValue(gumpID, out value))
                return value;
            else
                return defaultPosition;
        }

        public void SetLastPosition(string gumpID, Point position)
        {
            if (LastPositions.ContainsKey(gumpID))
                LastPositions[gumpID] = position;
            else
                LastPositions.Add(gumpID, position);
        }
    }
}
