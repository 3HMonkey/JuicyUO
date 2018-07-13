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
using JuicyUO.Core.ComponentModel;
using JuicyUO.Core.Input;
#endregion

namespace JuicyUO.Configuration.Properties
{
    public class MouseProperty : NotifyPropertyChangedBase
    {
        MouseButton m_InteractionButton = MouseButton.Left;
        MouseButton m_MovementButton = MouseButton.Right;
        bool m_IsEnabled = true;
        float m_ClickAndPickUpMS = 800f; // this is close to what the legacy client uses.
        float m_DoubleClickMS = 400f;

        public MouseProperty()
        {

        }

        public bool IsEnabled
        {
            get { return m_IsEnabled; }
            set { SetProperty(ref m_IsEnabled, value); }
        }

        public MouseButton MovementButton
        {
            get { return m_MovementButton; }
            set { SetProperty(ref m_MovementButton, value); }
        }

        public MouseButton InteractionButton
        {
            get { return m_InteractionButton; }
            set { SetProperty(ref m_InteractionButton, value); }
        }

        public float ClickAndPickupMS
        {
            get { return m_ClickAndPickUpMS; }
            set { SetProperty(ref m_ClickAndPickUpMS, Clamp(value, 0, 2000)); }
        }

        public float DoubleClickMS
        {
            get { return m_DoubleClickMS; }
            set { SetProperty(ref m_DoubleClickMS, Clamp(value, 0, 2000)); }
        }

        
    }
}