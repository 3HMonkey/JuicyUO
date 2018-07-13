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

#region Usings
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using JuicyUO.Core;
using JuicyUO.Core.Diagnostics;
using JuicyUO.Core.Diagnostics.Listeners;
using JuicyUO.Core.Diagnostics.Tracing;
#endregion

namespace JuicyUO
{
    sealed class Bootstrapper
    {
        // === Main ===================================================================================================
        [STAThread]
        static void Main(string[] args)
        {
            new Bootstrapper(args).Initialize();
        }

        // === Instance ===============================================================================================
        readonly GeneralExceptionHandler m_ExecptionHandler;

        Bootstrapper(string[] args)
        {
            m_ExecptionHandler = new GeneralExceptionHandler();
        }

        void Initialize()
        {
            ConfigureTraceListeners();
            if (Settings.Debug.IsConsoleEnabled && !ConsoleManager.HasConsole)
            {
                ConsoleManager.Show();
            }
            try
            {
                StartEngine();
            }
            finally
            {
                if (ConsoleManager.HasConsole)
                {
                    ConsoleManager.Hide();
                }
            }
        }

        void StartEngine()
        {
            SetExceptionHandlers();
            using (UltimaGame engine = new UltimaGame())
            {
                Resolutions.SetWindowSize(engine.Window);
                engine.Run();
            }
            ClearExceptionHandlers();
        }

        void ConfigureTraceListeners()
        {
            if (Debugger.IsAttached)
            {
                Tracer.RegisterListener(new DebugOutputEventListener());
            }
            Tracer.RegisterListener(new FileLogEventListener("debug.log"));
            if (Settings.Debug.IsConsoleEnabled)
            {
                Tracer.RegisterListener(new ConsoleOutputEventListener());
            }
            Tracer.RegisterListener(new MsgBoxOnCriticalListener());
        }

        void SetExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        void ClearExceptionHandlers()
        {
            AppDomain.CurrentDomain.UnhandledException -= OnUnhandledException;
            TaskScheduler.UnobservedTaskException -= OnUnobservedTaskException;
        }

        void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            m_ExecptionHandler.OnError((Exception) e.ExceptionObject);
        }

        void OnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            m_ExecptionHandler.OnError(e.Exception);
        }
    }
}