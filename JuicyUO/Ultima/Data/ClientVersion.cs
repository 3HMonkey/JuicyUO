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

using System.Diagnostics;
using System.IO;
using JuicyUO.Ultima.IO;

namespace JuicyUO.Ultima.Data {
    public static class ClientVersion {
        // NOTE FROM ZaneDubya: DO NOT change DefaultVersion from 6.0.6.2.
        // We are focusing our efforts on getting a specific version of the client working.
        // Once we have this version working, we will attempt to support additional versions.
        // We will not support any issues you experience after changing this value.
        public static readonly byte[] DefaultVersion = { 6, 0, 6, 2 };

        static readonly byte[] m_UnknownClientVersion = { 0, 0, 0, 0 };
        static readonly byte[] m_ExtendedAddItemToContainer = { 6, 0, 1, 7 };
        static readonly byte[] m_ExtendedFeaturesVersion = { 6, 0, 14, 2 };
        static readonly byte[] m_ConvertedToUOPVersion = { 7, 0, 24, 0 };
        static byte[] m_ClientExeVersion;

        public static byte[] ClientExe {
            get {
                if (m_ClientExeVersion == null) {
                    string path = FileManager.GetPath("client.exe");
                    if (File.Exists(path)) {
                        FileVersionInfo exe = FileVersionInfo.GetVersionInfo(path);
                        m_ClientExeVersion = new byte[] {
                            (byte)exe.FileMajorPart, (byte)exe.FileMinorPart,
                            (byte)exe.FileBuildPart, (byte)exe.FilePrivatePart };
                    }
                    else {
                        m_ClientExeVersion = m_UnknownClientVersion;
                    }
                }
                return m_ClientExeVersion;
            }
        }

        public static bool InstallationIsUopFormat => GreaterThanOrEqualTo(ClientExe, m_ConvertedToUOPVersion);

        public static bool HasExtendedFeatures(byte[] version) => GreaterThanOrEqualTo(version, m_ExtendedFeaturesVersion);

        public static bool HasExtendedAddItemPacket(byte[] version) => GreaterThanOrEqualTo(version, m_ExtendedAddItemToContainer);

        public static bool EqualTo(byte[] a, byte[] b) {
            if (a == null || b == null) {
                return false;
            }
            if (a.Length != b.Length) {
                return false;
            }
            int index = 0;
            while (index < a.Length) {
                if (a[index] != b[index]) {
                    return false;
                }
                index++;
            }
            return true;
        }

        /// <summary> Compare two arrays of equal size. Returns true if first parameter array is greater than or equal to second. </summary>
        static bool GreaterThanOrEqualTo(byte[] a, byte[] b) {
            if (a == null || b == null) {
                return false;
            }
            if (a.Length != b.Length) {
                return false;
            }
            int index = 0;
            while (index < a.Length) {
                if (a[index] > b[index]) {
                    return true;
                }
                if (a[index] < b[index]) {
                    return false;
                }
                index++;
            }
            return true;
        }
    }
}