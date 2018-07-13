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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using JuicyUO.Core.Diagnostics.Tracing;
using JuicyUO.Core.Patterns;
#endregion

namespace JuicyUO.Core {
    class PluginManager {
        readonly List<IModule> m_Modules = new List<IModule>();

        public PluginManager(string baseAppPath) {
            Configure(baseAppPath);
        }

        void Configure(string baseAppPath) {
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(baseAppPath, "plugins"));
            if (!directory.Exists) {
                return;
            }
            FileInfo[] assemblies = directory.GetFiles("*.dll");
            foreach (FileInfo file in assemblies) {
                try {
                    Tracer.Info("Loading plugin {0}.", file.Name);
                    Assembly assembly = Assembly.LoadFile(file.FullName);
                    IEnumerable<Type> modules = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IModule)));
                    foreach (Type module in modules) {
                        Tracer.Info("Activating module {0}.", module.FullName);
                        IModule instance = (IModule)Activator.CreateInstance(module);
                        LoadModule(instance);
                    }
                }
                catch (Exception e) {
                    Tracer.Warn("An error occurred while trying to load plugin. [{0}]", file.FullName);
                    Tracer.Warn(e);
                }
            }
        }

        void LoadModule(IModule module) {
            m_Modules.Add(module);
            module.Load();
        }
    }
}
