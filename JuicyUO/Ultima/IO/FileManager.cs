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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.IO.UOP;
#endregion

namespace JuicyUO.Ultima.IO
{
    class FileManager
    {
        static bool m_isDataPresent;
        static public bool IsUODataPresent => m_isDataPresent;

        static readonly string[] m_knownRegkeys = {
                @"Origin Worlds Online\Ultima Online\KR Legacy Beta",
                @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000",
                @"Origin Worlds Online\Ultima Online\1.0",
                @"Origin Worlds Online\Ultima Online Third Dawn\1.0",
                @"EA GAMES\Ultima Online Samurai Empire",
                @"EA Games\Ultima Online: Mondain's Legacy",
                @"EA GAMES\Ultima Online Samurai Empire\1.0",
                @"EA GAMES\Ultima Online Samurai Empire\1.00.0000",
                @"EA GAMES\Ultima Online: Samurai Empire\1.0",
                @"EA GAMES\Ultima Online: Samurai Empire\1.00.0000",
                @"EA Games\Ultima Online: Mondain's Legacy\1.0",
                @"EA Games\Ultima Online: Mondain's Legacy\1.00.0000",
                @"Origin Worlds Online\Ultima Online Samurai Empire BETA\2d\1.0",
                @"Origin Worlds Online\Ultima Online Samurai Empire BETA\3d\1.0",
                @"Origin Worlds Online\Ultima Online Samurai Empire\2d\1.0",
                @"Origin Worlds Online\Ultima Online Samurai Empire\3d\1.0",
                @"Electronic Arts\EA Games\Ultima Online Stygian Abyss Classic",
                @"Electronic Arts\EA Games\Ultima Online Classic",
                @"Electronic Arts\EA Games\"
            };

        static string m_FileDirectory;

        public static bool Is64Bit => IntPtr.Size == 8;
        public static int ItemIDMask => ClientVersion.InstallationIsUopFormat ? 0xffff : 0x3fff;

        static FileManager() {
            Tracer.Debug("Initializing UOData. Is64Bit = {0}", Is64Bit);
            Tracer.Debug("Looking for UO Installation:");

            if (Settings.UltimaOnline.DataDirectory != null && Directory.Exists(Settings.UltimaOnline.DataDirectory)) {
                Tracer.Debug("Settings: {0}", Settings.UltimaOnline.DataDirectory);

                m_FileDirectory = Settings.UltimaOnline.DataDirectory;
                m_isDataPresent = true;
            }
            else {
                for (int i = 0; i < m_knownRegkeys.Length; i++) {
                    string exePath = GetExePath(Is64Bit ? $"Wow6432Node\\{m_knownRegkeys[i]}" : m_knownRegkeys[i]);
                    if (exePath != null && Directory.Exists(exePath)) {
                        if (IsClientIsCompatible(exePath)) {
                            Tracer.Debug($"Compatible: {exePath}");
                            Settings.UltimaOnline.DataDirectory = exePath;
                            m_FileDirectory = exePath;
                            m_isDataPresent = true;
                        }
                        else {
                            Tracer.Debug($"Incompatible: {exePath}");
                        }
                    }
                }
            }

            if (m_FileDirectory == null) {
                m_isDataPresent = false;
            }
            else {
                Tracer.Debug(string.Empty);
                Tracer.Debug($"Selected: {m_FileDirectory}");
                string clientVersion = string.Join(".", ClientVersion.ClientExe);
                string patchVersion = string.Join(".", Settings.UltimaOnline.PatchVersion);
                Tracer.Debug($"Client.Exe version: {clientVersion}; Patch version reported to server: {patchVersion}");
                if (!ClientVersion.EqualTo(Settings.UltimaOnline.PatchVersion, ClientVersion.DefaultVersion))
                {
                    Tracer.Warn("Note from ZaneDubya: I will not support any code where the Patch version is not {0}", string.Join(".", ClientVersion.DefaultVersion));
                }
            }
        }

        static bool IsClientIsCompatible(string path) {
            IEnumerable<string> files = Directory.EnumerateFiles(path);
            foreach (string filepath in files) {
                string extension = Path.GetExtension(filepath).ToLower();
                if (extension == ".uop") {
                    return false;
                }
            }
            return true;
        }

        static string GetExePath(string subName) {
            try {
                RegistryKey key = Registry.LocalMachine.OpenSubKey($"SOFTWARE\\{subName}");
                if (key == null) {
                    key = Registry.CurrentUser.OpenSubKey($"SOFTWARE\\{subName}");
                    if (key == null) {
                        return null;
                    }
                }
                string path = key.GetValue("ExePath") as string;
                if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
                    path = key.GetValue("Install Dir") as string;
                    if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) {
                        path = key.GetValue("InstallDir") as string;
                        if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) {
                            return null;
                        }
                    }
                }
                if (File.Exists(path)) {
                    path = Path.GetDirectoryName(path);
                }
                if (string.IsNullOrEmpty(path) || !Directory.Exists(path)) {
                    return null;
                }
                return path;
            }
            catch {
                return null;
            }
        }

        public static string GetFilePath(string path)
        {
            if (m_FileDirectory != null)
            {
                path = Path.Combine(m_FileDirectory, path);
                if (File.Exists(path))
                    return path;
            }
            return null;
        }

        public static string[] GetFilePaths(string searchPattern)
        {
            string[] files = Directory.GetFiles(m_FileDirectory, searchPattern);
            return files;
        }

        public static bool Exists(string name) {
            try {
                name = Path.Combine(m_FileDirectory, name);
                Tracer.Debug($"Checking if file exists [{name}]");
                if (File.Exists(name)) {
                    return true;
                }
                return false;
            }
            catch {
                return false;
            }
        }

        public static bool Exists(string name, int index, string type) => Exists($"{name}{index}.{type}");

        public static bool Exists(string name, string type) => Exists($"{name}.{type}");

        public static FileStream GetFile(string path) {
            try {
                path = Path.Combine(m_FileDirectory, path);
                return new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            }
            catch {
                return null;
            }
        }

        public static FileStream GetFile(string name, uint index, string type) => GetFile($"{name}{index}.{type}");

        public static FileStream GetFile(string name, string type) => GetFile($"{name}.{type}");

        public static string GetPath(string name) => Path.Combine(m_FileDirectory, name);


        public static AFileIndex CreateFileIndex(string uopFile, int length, bool hasExtra, string extension) {
            uopFile = GetPath(uopFile);
            AFileIndex fileIndex = new UopFileIndex(uopFile, length, hasExtra, extension);
            return fileIndex;
        }

        public static AFileIndex CreateFileIndex(string idxFile, string mulFile, int length, int patch_file) {
            idxFile = GetPath(idxFile);
            mulFile = GetPath(mulFile);
            AFileIndex fileIndex = new MulFileIndex(idxFile, mulFile, length, patch_file);
            return fileIndex;
        }
    }
}