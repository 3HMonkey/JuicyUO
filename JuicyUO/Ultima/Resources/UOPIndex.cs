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
using System.Collections.Generic;
using System.IO;

namespace JuicyUO.Ultima.Resources
{
public class UOPIndex
        {
            private readonly UOPEntry[] _entries;
            private readonly int _length;
            private readonly BinaryReader _reader;
            private readonly int _version;

            public UOPIndex(FileStream stream)
            {
                _reader = new BinaryReader(stream);
                _length = (int)stream.Length;

                if (_reader.ReadInt32() != 0x50594D)
                {
                    throw new ArgumentException("Invalid UOP file.");
                }

                _version = _reader.ReadInt32();
                _reader.ReadInt32();
                var nextTable = _reader.ReadInt32();

                var entries = new List<UOPEntry>();

                do
                {
                    stream.Seek(nextTable, SeekOrigin.Begin);
                    var count = _reader.ReadInt32();
                    nextTable = _reader.ReadInt32();
                    _reader.ReadInt32();

                    for (var i = 0; i < count; ++i)
                    {
                        var offset = _reader.ReadInt32();

                        if (offset == 0)
                        {
                            stream.Seek(30, SeekOrigin.Current);
                            continue;
                        }

                        _reader.ReadInt64();
                        var length = _reader.ReadInt32();

                        entries.Add(new UOPEntry(offset, length));

                        stream.Seek(18, SeekOrigin.Current);
                    }
                } while (nextTable != 0 && nextTable < _length);

                entries.Sort(OffsetComparer.Instance);

                for (var i = 0; i < entries.Count; ++i)
                {
                    stream.Seek(entries[i].Offset + 2, SeekOrigin.Begin);

                    int dataOffset = _reader.ReadInt16();
                    entries[i].Offset += 4 + dataOffset;

                    stream.Seek(dataOffset, SeekOrigin.Current);
                    entries[i].Order = _reader.ReadInt32();
                }

                entries.Sort();
                _entries = entries.ToArray();
            }

            private class OffsetComparer : IComparer<UOPEntry>
            {
                public static readonly IComparer<UOPEntry> Instance = new OffsetComparer();

                public int Compare(UOPEntry x, UOPEntry y)
                {
                    return x.Offset.CompareTo(y.Offset);
                }
            }

            private class UOPEntry : IComparable<UOPEntry>
            {
                public readonly int Length;
                public int Offset;
                public int Order;

                public UOPEntry(int offset, int length)
                {
                    Offset = offset;
                    Length = length;
                    Order = 0;
                }

                public int CompareTo(UOPEntry other)
                {
                    return Order.CompareTo(other.Order);
                }
            }

            public int Version
            {
                get { return _version; }
            }

            public int Lookup(int offset)
            {
                var total = 0;

                for (var i = 0; i < _entries.Length; ++i)
                {
                    var newTotal = total + _entries[i].Length;

                    if (offset < newTotal)
                    {
                        return _entries[i].Offset + (offset - total);
                    }

                    total = newTotal;
                }

                return _length;
            }

            public void Close()
            {
                _reader.Close();
            }
        }
}
