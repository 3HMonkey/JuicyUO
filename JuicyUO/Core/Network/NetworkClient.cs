﻿#region license
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using JuicyUO.Core.Diagnostics;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.IO;
using JuicyUO.Core.Network.Compression;
#endregion

namespace JuicyUO.Core.Network
{
    public sealed class NetworkClient : INetworkClient
    {
        readonly HuffmanDecompression m_Decompression;
        readonly List<PacketHandler>[] m_TypedHandlers;
        readonly List<PacketHandler>[][] m_ExtendedTypedHandlers;
        readonly BufferPool m_BufferPool = new BufferPool("Network Client - Buffer Pool", 8, 0x10000);
        readonly object m_SyncRoot = new object();
        readonly PacketChunk m_IncompleteDecompressionPacket;
        readonly PacketChunk m_IncompletePacket;

        Queue<QueuedPacket> m_Queue = new Queue<QueuedPacket>();
        Queue<QueuedPacket> m_WorkingQueue = new Queue<QueuedPacket>();

        Socket m_ServerSocket;
        IPAddress m_ServerAddress;
        IPEndPoint m_ServerEndPoint;

        bool m_IsDecompressionEnabled;
        bool m_IsConnected;

        public int ClientAddress
        {
            get
            {
                IPHostEntry localEntry = Dns.GetHostEntry(Dns.GetHostName());
                int address;

                if (localEntry.AddressList.Length > 0)
                {
#pragma warning disable 618
                    address = (int)localEntry.AddressList[0].Address;
#pragma warning restore 618
                }
                else
                {
                    address = 0x100007f;
                }

                return ((((address & 0xff) << 0x18) | ((address & 65280) << 8)) | ((address >> 8) & 65280)) | ((address >> 0x18) & 0xff);
            }
        }

        public IPAddress ServerAddress
        {
            get { return m_ServerAddress; }
        }

        public bool IsDecompressionEnabled
        {
            get { return m_IsDecompressionEnabled; }
            set { m_IsDecompressionEnabled = value; }
        }

        public bool IsConnected
        {
            get { return m_IsConnected; }
        }

        public NetworkClient()
        {
            m_Decompression = new HuffmanDecompression();
            m_IsDecompressionEnabled = false;

            m_IncompletePacket = new PacketChunk(m_BufferPool.AcquireBuffer());
            m_IncompleteDecompressionPacket = new PacketChunk(m_BufferPool.AcquireBuffer());

            m_TypedHandlers = new List<PacketHandler>[0x100];
            m_ExtendedTypedHandlers = new List<PacketHandler>[0x100][];

            for (int i = 0; i < m_TypedHandlers.Length; i++)
            {
                m_TypedHandlers[i] = new List<PacketHandler>();
            }
        }

        public void Register<T>(object client, int id, int length, Action<T> onReceive) where T : IRecvPacket
        {
            Type type = typeof(T);
            ConstructorInfo[] ctors = type.GetConstructors();
            bool valid = false;
            for (int i = 0; i < ctors.Length && !valid; i++)
            {
                ParameterInfo[] parameters = ctors[i].GetParameters();
                valid = (parameters.Length == 1 && parameters[0].ParameterType == typeof(PacketReader));
            }
            if (!valid)
            {
                throw new NetworkException($"Unable to register packet type {type} without a public constructor with a {typeof(PacketReader)} parameter");
            }
            if (id > byte.MaxValue)
            {
                throw new NetworkException($"Unable to register packet id 0x{id:X4} because it is greater than 0xff");
            }
            PacketHandler handler = new PacketHandler<T>(id, length, type, client, onReceive);
            if (m_TypedHandlers[id].Any())
            {
                int requiredLength = m_TypedHandlers[id][0].Length;
                Guard.Requires(requiredLength == length,
                    "Invalid packet length.  All packet handlers for 0x{0:X2} must specify a length of {1}.", id,
                    requiredLength);
            }
            m_TypedHandlers[id].Add(handler);
        }

        public void Unregister(object client)
        {
            for (int id = 0; id < byte.MaxValue; id++)
            {
                if (m_TypedHandlers[id] != null)
                {
                    for (int i = 0; i < m_TypedHandlers[id].Count; i++)
                    {
                        PacketHandler handler = m_TypedHandlers[id][i] as PacketHandler;
                        if (handler.Client == client)
                        {
                            m_TypedHandlers[id].RemoveAt(i);
                            i--;
                        }
                    }
                }
            }
        }

        public void Unregister(object client, int id)
        {
            for (int i = 0; i < m_TypedHandlers[id].Count; i++)
            {
                PacketHandler handler = m_TypedHandlers[id][i] as PacketHandler;
                if (handler.Client == client)
                {
                    m_TypedHandlers[id].RemoveAt(i);
                    break;
                }
            }
        }

        public void RegisterExtended<T>(object client, int extendedId, int subId, int length, Action<T> onReceive) where T : IRecvPacket
        {
            Type type = typeof(T);
            ConstructorInfo[] ctors = type.GetConstructors();

            bool valid = false;

            for (int i = 0; i < ctors.Length && !valid; i++)
            {
                ParameterInfo[] parameters = ctors[i].GetParameters();
                valid = (parameters.Length == 1 && parameters[0].ParameterType == typeof(PacketReader));
            }

            if (!valid)
            {
                throw new NetworkException(string.Format("Unable to register packet type {0} without a public constructor with a {1} parameter", type, typeof(PacketReader)));
            }

            if (extendedId > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet extendedId {0:X2} because it is greater than byte.MaxValue", extendedId));
            }

            if (subId > byte.MaxValue)
            {
                throw new NetworkException(string.Format("Unable to register packet subId {0:X2} because it is greater than byte.MaxValue", subId));
            }

            if (m_ExtendedTypedHandlers[extendedId] == null)
            {
                m_ExtendedTypedHandlers[extendedId] = new List<PacketHandler>[0x100];

                for (int i = 0; i < m_ExtendedTypedHandlers[extendedId].Length; i++)
                {
                    m_ExtendedTypedHandlers[extendedId][i] = new List<PacketHandler>();
                }
            }

            Tracer.Debug($"Registering Extended Command: id: 0x{extendedId:X2} subCommand: 0x{subId:X2} Length: {length}");

            PacketHandler handler = new PacketHandler<T>(subId, length, type, client, onReceive);
            m_ExtendedTypedHandlers[extendedId][subId].Add(handler);
        }

        public bool Connect(string ipAddressOrHostName, int port)
        {
            lock (m_SyncRoot)
            {
                m_WorkingQueue.Clear();
            }

            m_IncompletePacket.Clear();
            m_IncompleteDecompressionPacket.Clear();

            if (IsConnected)
            {
                Disconnect();
            }

            bool success = true;

            try
            {
                if (!IPAddress.TryParse(ipAddressOrHostName, out m_ServerAddress))
                {
                    IPAddress[] ipAddresses = Dns.GetHostAddresses(ipAddressOrHostName);

                    if (ipAddresses.Length == 0)
                    {
                        throw new NetworkException("Host address was unreachable or invalid, unable to obtain an ip address.");
                    }

                    // On Vista and later, the first ip address is an empty one '::1'.
                    // This makes sure we choose the first valid ip address.
                    foreach (IPAddress address in ipAddresses)
                    {
                        if (address.ToString().Length <= 7)
                        {
                            continue;
                        }

                        m_ServerAddress = address;
                        break;
                    }
                }

                m_ServerEndPoint = new IPEndPoint(m_ServerAddress, port);

                Tracer.Debug("Connecting to {0}:{1}...", m_ServerAddress, port);

                m_ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                m_ServerSocket.Connect(m_ServerEndPoint);

                if (m_ServerSocket.Connected)
                {
                    Tracer.Debug("Connected.");
                    SocketState state = new SocketState(m_ServerSocket, m_BufferPool.AcquireBuffer());
                    m_ServerSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }

            }
            catch
            {
                success = false;
            }

            m_IsConnected = success;
            return success;
        }

        public void Disconnect()
        {
            if (m_ServerSocket != null)
            {
                try
                {
                    m_ServerSocket.Shutdown(SocketShutdown.Both);
                    m_ServerSocket.Close();
                }
                catch
                {
                    // empty catch.
                }
                m_ServerSocket = null;
                m_ServerEndPoint = null;
                m_IsDecompressionEnabled = false;
                Tracer.Debug("Disconnected.");
                m_IsConnected = false;
            }
        }

        public bool Send(ISendPacket packet)
        {
            byte[] buffer = packet.Compile();

            if (IsConnected)
            {
                bool success = Send(buffer, 0, packet.Length, packet.Name);
                if (!success)
                {
                    Disconnect();
                }
                return success;
            }

            return false;
        }

        public bool Send(byte[] buffer, int offset, int length, string name)
        {
            bool success = true;
            if (buffer == null || buffer.Length == 0)
            {
                throw new NetworkException("Unable to send, buffer was null or empty");
            }
            LogPacket(buffer, length, false);
            try
            {
                lock (m_ServerSocket)
                {
                    m_ServerSocket.Send(buffer, offset, length, SocketFlags.None);
                }
            }
            catch (Exception e)
            {
                Tracer.Debug(e.ToString());
                success = false;
            }
            return success;
        }

        void OnReceive(IAsyncResult result)
        {
            SocketState state = result.AsyncState as SocketState;

            if (state == null)
            {
                Tracer.Warn("Socket state was null.");
                return;
            }

            try
            {
                Socket socket = state.Socket;
                if (socket.Connected == false)
                {
                    Disconnect();
                    return;
                }
                int length = socket.EndReceive(result);
                if (length > 0)
                {
                    byte[] buffer = state.Buffer;
                    if (m_IsDecompressionEnabled)
                    {
                        DecompressBuffer(ref buffer, ref length);
                    }

                    if (m_IncompletePacket.Length > 0)
                    {
                        m_IncompletePacket.Prepend(buffer, length);
                        m_IncompletePacket.Clear();
                    }

                    int offset = 0;

                    ProcessBuffer(buffer, ref offset, length);

                    // Not all the data was processed, due to an incomplete packet
                    if (offset < length)
                    {
                        m_IncompletePacket.Write(buffer, offset, length - offset);
                    }
                }

                if (m_ServerSocket != null)
                {
                    m_ServerSocket.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None, OnReceive, state);
                }
            }
            catch (Exception e)
            {
                Tracer.Debug(e.ToString());
                Disconnect();
            }
        }

        void DecompressBuffer(ref byte[] buffer, ref int length)
        {
            byte[] source = m_BufferPool.AcquireBuffer();

            int incompleteLength = m_IncompleteDecompressionPacket.Length;
            int sourceLength = incompleteLength + length;

            if (incompleteLength > 0)
            {
                m_IncompleteDecompressionPacket.Prepend(source, 0);
                m_IncompleteDecompressionPacket.Clear();
            }

            Buffer.BlockCopy(buffer, 0, source, incompleteLength, length);

            int outSize;
            int processedOffset = 0;
            int sourceOffset = 0;
            int offset = 0;

            while (m_Decompression.DecompressChunk(ref source, ref sourceOffset, sourceLength, ref buffer, offset, out outSize))
            {
                processedOffset = sourceOffset;
                offset += outSize;
            }

            length = offset;

            // We've run out of data to parse, or the packet was incomplete. If the packet was incomplete,
            // we should save what's left for the next socket receive event.
            if (processedOffset >= sourceLength)
            {
                m_BufferPool.ReleaseBuffer(source);
                return;
            }

            m_IncompleteDecompressionPacket.Write(source, processedOffset, sourceLength - processedOffset);
        }

        void ProcessBuffer(byte[] buffer, ref int offset, int length)
        {
            int index = offset;

            while (index < length)
            {
                int realLength;
                PacketHandler packetHandler;
                List<PacketHandler> packetHandlers = GetHandlers(buffer[index], buffer[index + 1]);

                // Ensure we have the proper handler for the size of this packet
                if (!GetPacketSizeAndHandler(packetHandlers, buffer, index, out realLength, out packetHandler))
                {
                    byte[] formatBuffer = m_BufferPool.AcquireBuffer();
                    Buffer.BlockCopy(buffer, index, formatBuffer, 0, length - index);
                    Tracer.Warn($"Unhandled packet with id: 0x{buffer[index]:x2}, possible subid: 0x{buffer[index + 1]:x2}{Environment.NewLine}{Utility.FormatBuffer(formatBuffer, length - index)}");
                    index += length - index;
                    break;
                }

                // Entire packet exist, so we can process it
                if ((length - index) >= realLength)
                {
                    // TODO: Move this to a buffer pool, need to investigate max byte[].length and pool size
                    byte[] packetBuffer = new byte[realLength];
                    Buffer.BlockCopy(buffer, index, packetBuffer, 0, realLength);
                    AddPacket(packetHandler, packetBuffer, realLength);
                    index += realLength;
                }
                else
                {
                    //Need more data
                    break;
                }
            }

            offset = index;
        }

        void AddPacket(PacketHandler packetHandler, byte[] packetBuffer, int realLength)
        {
            lock (m_SyncRoot)
            {
                m_WorkingQueue.Enqueue(new QueuedPacket(packetHandler, packetBuffer, realLength));
            }
        }

        public void Slice()
        {
            if (!IsConnected)
            {
                return;
            }

            lock (m_SyncRoot)
            {
                Queue<QueuedPacket> temp = m_WorkingQueue;
                m_WorkingQueue = m_Queue;
                m_Queue = temp;
            }

            while (m_Queue.Count > 0)
            {
                QueuedPacket packet = m_Queue.Dequeue();
                LogPacket(packet.PacketBuffer, packet.RealLength);
                InvokeHandler(packet.PacketHandler, packet.PacketBuffer, packet.RealLength);
            }
        }

        /// <summary>
        /// Determines the correct packet size of the packet, and if there is a packetHandler that will handle this packet.
        /// </summary>
        /// <param name="packetHandlers">List of possible packet handlers for this packet. A packet handler with length of -1 must be first, if any.</param>
        /// <param name="realLength">The real length of the packet.</param>
        /// <returns>True if there is a packetHandler that will handle this packet.</returns>
        bool GetPacketSizeAndHandler(List<PacketHandler> packetHandlers, byte[] buffer, int offset, out int realLength, out PacketHandler packetHandler)
        {
            realLength = 0;
            packetHandler = null;

            if (packetHandlers.Count == 0)
            {
                return false;
            }
            foreach (PacketHandler ph in packetHandlers)
            {
                if (ph.Length == -1)
                {
                    realLength = buffer[offset + 2] | (buffer[offset + 1] << 8);
                    packetHandler = ph;
                    return true;
                }
                realLength = ph.Length;
                packetHandler = ph;
                return true;
            }
            return false;
        }

        void LogPacket(byte[] buffer, int length, bool servertoclient = true)
        {
            if (Settings.Debug.LogPackets)
            {
                Tracer.Debug(servertoclient ? "Server - > Client" : "Client - > Server");
                Tracer.Debug($"Id: 0x{buffer[0]:X2} Length: {length}");
                Tracer.Debug($"{Utility.FormatBuffer(buffer, length)}{Environment.NewLine}");
            }
        }

        void InvokeHandler(PacketHandler packetHandler, byte[] buffer, int length)
        {
            if (packetHandler == null)
            {
                return;
            }
            PacketReader reader = PacketReader.CreateInstance(buffer, length, packetHandler.Length != -1);
            packetHandler.Invoke(reader);
        }

        List<PacketHandler> GetHandlers(byte cmd, byte subcommand)
        {
            List<PacketHandler> packetHandlers = new List<PacketHandler>();
            packetHandlers.AddRange(m_TypedHandlers[cmd]);
            if (m_ExtendedTypedHandlers[cmd] != null)
            {
                packetHandlers.AddRange(m_ExtendedTypedHandlers[cmd][subcommand]);
            }
            return packetHandlers;
        }
    }
}
