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
using System.Collections;
using System.IO;
using System.Net;
using System.Threading;

namespace JuicyUO.Core.IO
{
    public sealed class AsyncWriter : GenericWriter
    {
        private static int m_ThreadCount;

        private readonly int BufferSize;

        private readonly bool PrefixStrings;

        private readonly FileStream m_File;

        private readonly Queue m_WriteQueue;
        private BinaryWriter m_Bin;
        private bool m_Closed;
        private long m_CurPos;
        private long m_LastPos;
        private MemoryStream m_Mem;
        private Thread m_WorkerThread;

        public AsyncWriter(string filename, bool prefix)
            : this(filename, 1048576, prefix) //1 mb buffer
        {
        }

        public AsyncWriter(string filename, int buffSize, bool prefix)
        {
            PrefixStrings = prefix;
            m_Closed = false;
            m_WriteQueue = Queue.Synchronized(new Queue());
            BufferSize = buffSize;

            m_File = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None);
            m_Mem = new MemoryStream(BufferSize + 1024);
            m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
        }

        public static int ThreadCount
        {
            get { return m_ThreadCount; }
        }

        public MemoryStream MemStream
        {
            get { return m_Mem; }
            set
            {
                if(m_Mem.Length > 0)
                {
                    Enqueue(m_Mem);
                }

                m_Mem = value;
                m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
                m_LastPos = 0;
                m_CurPos = m_Mem.Length;
                m_Mem.Seek(0, SeekOrigin.End);
            }
        }

        public override long Position
        {
            get { return m_CurPos; }
        }

        private void Enqueue(MemoryStream mem)
        {
            m_WriteQueue.Enqueue(mem);

            if(m_WorkerThread == null || !m_WorkerThread.IsAlive)
            {
                m_WorkerThread = new Thread(new WorkerThread(this).Worker);
                m_WorkerThread.Priority = ThreadPriority.BelowNormal;
                m_WorkerThread.Start();
            }
        }

        private void OnWrite()
        {
            long curlen = m_Mem.Length;
            m_CurPos += curlen - m_LastPos;
            m_LastPos = curlen;
            if(curlen >= BufferSize)
            {
                Enqueue(m_Mem);
                m_Mem = new MemoryStream(BufferSize + 1024);
                m_Bin = new BinaryWriter(m_Mem, Utility.UTF8WithEncoding);
                m_LastPos = 0;
            }
        }

        public override void Close()
        {
            Enqueue(m_Mem);
            m_Closed = true;
        }

        public override void Write(IPAddress value)
        {
            m_Bin.Write(Utility.GetLongAddressValue(value));
            OnWrite();
        }

        public override void Write(string value)
        {
            if(PrefixStrings)
            {
                if(value == null)
                {
                    m_Bin.Write((byte)0);
                }
                else
                {
                    m_Bin.Write((byte)1);
                    m_Bin.Write(value);
                }
            }
            else
            {
                m_Bin.Write(value);
            }
            OnWrite();
        }

        public override void WriteDeltaTime(DateTime value)
        {
            long ticks = value.Ticks;
            long now = DateTime.Now.Ticks;

            TimeSpan d;

            try
            {
                d = new TimeSpan(ticks - now);
            }
            catch
            {
                if(ticks < now)
                {
                    d = TimeSpan.MaxValue;
                }
                else
                {
                    d = TimeSpan.MaxValue;
                }
            }

            Write(d);
        }

        public override void Write(DateTime value)
        {
            m_Bin.Write(value.Ticks);
            OnWrite();
        }

        public override void Write(TimeSpan value)
        {
            m_Bin.Write(value.Ticks);
            OnWrite();
        }

        public override void Write(decimal value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(long value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(ulong value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void WriteEncodedInt(int value)
        {
            uint v = (uint)value;

            while(v >= 0x80)
            {
                m_Bin.Write((byte)(v | 0x80));
                v >>= 7;
            }

            m_Bin.Write((byte)v);
            OnWrite();
        }

        public override void Write(int value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(uint value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(short value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(ushort value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(double value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(float value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(char value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(byte value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(byte[] value)
        {
            for(int i = 0; i < value.Length; i++)
            {
                m_Bin.Write(value[i]);
            }
            OnWrite();
        }

        public override void Write(sbyte value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        public override void Write(bool value)
        {
            m_Bin.Write(value);
            OnWrite();
        }

        private class WorkerThread
        {
            private readonly AsyncWriter m_Parent;

            public WorkerThread(AsyncWriter parent)
            {
                m_Parent = parent;
            }

            public void Worker()
            {
                m_ThreadCount++;
                while(m_Parent.m_WriteQueue.Count > 0)
                {
                    MemoryStream mem = (MemoryStream)m_Parent.m_WriteQueue.Dequeue();

                    if(mem != null && mem.Length > 0)
                    {
                        mem.WriteTo(m_Parent.m_File);
                    }
                }

                if(m_Parent.m_Closed)
                {
                    m_Parent.m_File.Close();
                }

                m_ThreadCount--;

                if(m_ThreadCount <= 0)
                {
                    // Program.NotifyDiskWriteComplete();
                }
            }
        }
    }
}