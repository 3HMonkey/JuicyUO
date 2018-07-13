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

using System;
using System.Net;

namespace JuicyUO.Core.IO
{
    public abstract class GenericWriter
    {
        public abstract long Position
        {
            get;
        }

        public abstract void Close();

        public abstract void Write(string value);

        public abstract void Write(DateTime value);

        public abstract void Write(TimeSpan value);

        public abstract void Write(decimal value);

        public abstract void Write(long value);

        public abstract void Write(ulong value);

        public abstract void Write(int value);

        public abstract void Write(uint value);

        public abstract void Write(short value);

        public abstract void Write(ushort value);

        public abstract void Write(double value);

        public abstract void Write(float value);

        public abstract void Write(char value);

        public abstract void Write(byte value);

        public abstract void Write(byte[] value);

        public abstract void Write(sbyte value);

        public abstract void Write(bool value);

        public abstract void WriteEncodedInt(int value);

        public abstract void Write(IPAddress value);

        public abstract void WriteDeltaTime(DateTime value);

        //Stupid compiler won't notice there 'where' to differentiate the generic methods.
    }
}