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

namespace JuicyUO.Core.Audio.MP3Sharp.Decoding
{
    /// <summary>
    ///     Implementation of Bit Reservoir for Layer III.
    ///     The implementation stores single bits as a word in the buffer. If
    ///     a bit is set, the corresponding word in the buffer will be non-zero.
    ///     If a bit is clear, the corresponding word is zero. Although this
    ///     may seem waseful, this can be a factor of two quicker than
    ///     packing 8 bits to a byte and extracting.
    /// </summary>

    // REVIEW: there is no range checking, so buffer underflow or overflow
    // can silently occur.
    sealed class BitReserve
    {
        /// <summary>
        ///     Size of the internal buffer to store the reserved bits.
        ///     Must be a power of 2. And x8, as each bit is stored as a single
        ///     entry.
        /// </summary>
        private const int BUFSIZE = 4096*8;

        /// <summary>
        ///     Mask that can be used to quickly implement the
        ///     modulus operation on BUFSIZE.
        /// </summary>
        private static readonly int BUFSIZE_MASK = BUFSIZE - 1;

        private int[] buf;
        private int offset, totbit, buf_byte_idx;

        internal BitReserve()
        {
            InitBlock();

            offset = 0;
            totbit = 0;
            buf_byte_idx = 0;
        }

        private void InitBlock()
        {
            buf = new int[BUFSIZE];
        }

        /// <summary>
        ///     Return totbit Field.
        /// </summary>
        public int hsstell()
        {
            return (totbit);
        }

        /// <summary>
        ///     Read a number bits from the bit stream.
        /// </summary>
        public int ReadBits(int N)
        {
            totbit += N;

            int val = 0;

            int pos = buf_byte_idx;
            if (pos + N < BUFSIZE)
            {
                while (N-- > 0)
                {
                    val <<= 1;
                    val |= ((buf[pos++] != 0) ? 1 : 0);
                }
            }
            else
            {
                while (N-- > 0)
                {
                    val <<= 1;
                    val |= ((buf[pos] != 0) ? 1 : 0);
                    pos = (pos + 1) & BUFSIZE_MASK;
                }
            }
            buf_byte_idx = pos;
            return val;
        }

        /// <summary>
        ///     Read 1 bit from the bit stream.
        /// </summary>
        public int ReadOneBit()
        {
            totbit++;
            int val = buf[buf_byte_idx];
            buf_byte_idx = (buf_byte_idx + 1) & BUFSIZE_MASK;
            return val;
        }

        /// <summary>
        ///     Write 8 bits into the bit stream.
        /// </summary>
        public void hputbuf(int val)
        {
            int ofs = offset;
            buf[ofs++] = val & 0x80;
            buf[ofs++] = val & 0x40;
            buf[ofs++] = val & 0x20;
            buf[ofs++] = val & 0x10;
            buf[ofs++] = val & 0x08;
            buf[ofs++] = val & 0x04;
            buf[ofs++] = val & 0x02;
            buf[ofs++] = val & 0x01;

            if (ofs == BUFSIZE)
                offset = 0;
            else
                offset = ofs;
        }

        /// <summary>
        ///     Rewind n bits in Stream.
        /// </summary>
        public void RewindStreamBits(int bitCount)
        {
            totbit -= bitCount;
            buf_byte_idx -= bitCount;
            if (buf_byte_idx < 0)
                buf_byte_idx += BUFSIZE;
        }

        /// <summary>
        ///     Rewind n bytes in Stream.
        /// </summary>
        public void RewindStreamBytes(int byteCount)
        {
            int bits = (byteCount << 3);
            totbit -= bits;
            buf_byte_idx -= bits;
            if (buf_byte_idx < 0)
                buf_byte_idx += BUFSIZE;
        }
    }
}