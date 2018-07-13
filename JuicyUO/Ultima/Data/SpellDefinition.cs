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

using System.Text;

namespace JuicyUO.Ultima.Data
{
    public struct SpellDefinition
    {
        public static SpellDefinition EmptySpell = new SpellDefinition();

        public readonly string Name;
        public readonly int ID;
        public readonly int GumpIconID;
        public readonly int GumpIconSmallID;
        public readonly Reagents[] Regs;

        public SpellDefinition(string name, int index, int gumpIconID, params Reagents[] regs)
        {
            Name = name;
            ID = index;
            GumpIconID = gumpIconID;
            GumpIconSmallID = gumpIconID - 0x1298;
            Regs = regs;
        }

        public string CreateReagentListString(string separator)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < Regs.Length; i++)
            {
                switch (Regs[i])
                {
                    // britanian reagents
                    case Reagents.BlackPearl:
                        sb.Append("Black Pearl");
                        break;
                    case Reagents.Bloodmoss:
                        sb.Append("Bloodmoss");
                        break;
                    case Reagents.Garlic:
                        sb.Append("Garlic");
                        break;
                    case Reagents.Ginseng:
                        sb.Append("Ginseng");
                        break;
                    case Reagents.MandrakeRoot:
                        sb.Append("Mandrake Root");
                        break;
                    case Reagents.Nightshade:
                        sb.Append("Nightshade");
                        break;
                    case Reagents.SulfurousAsh:
                        sb.Append("Sulfurous Ash");
                        break;
                    case Reagents.SpidersSilk:
                        sb.Append("Spiders' Silk");
                        break;
                    // pagan reagents
                    case Reagents.BatWing:
                        sb.Append("Bat Wing");
                        break;
                    case Reagents.GraveDust:
                        sb.Append("Grave Dust");
                        break;
                    case Reagents.DaemonBlood:
                        sb.Append("Daemon Blood");
                        break;
                    case Reagents.NoxCrystal:
                        sb.Append("Nox Crystal");
                        break;
                    case Reagents.PigIron:
                        sb.Append("Pig Iron");
                        break;
                    default:
                        sb.Append("Unknown reagent");
                        break;
                }
                if (i < Regs.Length - 1)
                    sb.Append(separator);
            }
            return sb.ToString();
        }
    }
}
