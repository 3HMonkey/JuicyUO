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

namespace JuicyUO.Ultima.Input
{
    /// <summary>
    /// A single macro that performs one thing.
    /// </summary>
    public class Macro
    {
        public readonly MacroType Type;

        ValueTypes m_ValueType =  ValueTypes.None;
        int m_ValueInteger = -1;
        string m_ValueString;

        public Macro(MacroType type)
        {
            Type = type;
        }

        public Macro(MacroType type, int value)
            : this(type)
        {
            m_ValueInteger = value;
            m_ValueType = ValueTypes.Integer;
        }

        public Macro(MacroType type, string value)
            : this(type)
        {
            m_ValueString = value;
            m_ValueType = ValueTypes.String;
        }

        public int ValueInteger
        {
            set
            {
                m_ValueType =  ValueTypes.Integer;
                m_ValueInteger = value;
            }
            get
            {
                if (m_ValueType == ValueTypes.Integer)
                    return m_ValueInteger;
                return 0;
            }
        }

        public string ValueString
        {
            set
            {
                m_ValueType = ValueTypes.String;
                m_ValueString = value;
            }
            get
            {
                if (m_ValueType == ValueTypes.String)
                    return m_ValueString;
                return null;
            }
        }

        public ValueTypes ValueType
        {
            get
            {
                return m_ValueType;
            }
        }

        public override string ToString()
        {
            string value = (m_ValueType == ValueTypes.None ? string.Empty : (m_ValueType == ValueTypes.Integer ? m_ValueInteger.ToString() : m_ValueString));
            return string.Format("{0} ({1})", Type.ToString(), value);
        }

        public enum ValueTypes
        {
            None,
            Integer,
            String
        }
    }
}
