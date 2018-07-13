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
using System;
using System.Net;
#endregion

namespace JuicyUO.Core.IO
{
    public abstract class GenericReader
    {
        public abstract string ReadString();

        public abstract DateTime ReadDateTime();

        public abstract TimeSpan ReadTimeSpan();

        public abstract DateTime ReadDeltaTime();

        public abstract decimal ReadDecimal();

        public abstract long ReadLong();

        public abstract ulong ReadULong();

        public abstract int ReadInt();

        public abstract uint ReadUInt();

        public abstract short ReadShort();

        public abstract ushort ReadUShort();

        public abstract double ReadDouble();

        public abstract float ReadFloat();

        public abstract char ReadChar();

        public abstract byte ReadByte();

        public abstract sbyte ReadSByte();

        public abstract bool ReadBool();

        public abstract int ReadEncodedInt();

        public abstract IPAddress ReadIPAddress();

        public abstract bool End();
    }
}