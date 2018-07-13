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

namespace JuicyUO.Ultima.Data
{
    public class Features
    {
        FeatureFlags m_Flags;
        public void SetFlags(FeatureFlags flags)
        {
            m_Flags |= flags;
        }

        public bool T2A => m_Flags.HasFlag(FeatureFlags.TheSecondAge);
        public bool UOR => m_Flags.HasFlag(FeatureFlags.Renaissance);
        public bool ThirdDawn => m_Flags.HasFlag(FeatureFlags.ThirdDawn);
        public bool LBR => m_Flags.HasFlag(FeatureFlags.LordBlackthornsRevenge);
        public bool AOS => m_Flags.HasFlag(FeatureFlags.AgeOfShadows);
        public bool CharSlots6 => m_Flags.HasFlag(FeatureFlags.CharacterSlot6);
        public bool SE => m_Flags.HasFlag(FeatureFlags.SameraiEmpire);
        public bool ML => m_Flags.HasFlag(FeatureFlags.MondainsLegacy);
        public bool Splash8th => m_Flags.HasFlag(FeatureFlags.Splash8);
        public bool Splash9th => m_Flags.HasFlag(FeatureFlags.Splash9);
        public bool TenthAge => m_Flags.HasFlag(FeatureFlags.TenthAge);
        public bool MoreStorage => m_Flags.HasFlag(FeatureFlags.MoreStorage);
        public bool CharSlots7 => m_Flags.HasFlag(FeatureFlags.TheSecondAge);
        public bool TenthAgeFaces => m_Flags.HasFlag(FeatureFlags.TenthAgeFaces);
        public bool TrialAccount => m_Flags.HasFlag(FeatureFlags.TrialAccount);
        public bool EleventhAge => m_Flags.HasFlag(FeatureFlags.EleventhAge);
        public bool SA => m_Flags.HasFlag(FeatureFlags.StygianAbyss);

        public bool TooltipsEnabled => AOS;
    }
}