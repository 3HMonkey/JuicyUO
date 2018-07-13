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
using JuicyUO.Core.Diagnostics.Tracing;
using System.Threading;

namespace JuicyUO.Core
{
    public class DelayedAction
    {
        private volatile Action m_Action;

        private DelayedAction(Action action, int msDelay)
        {
            m_Action = action;
            dynamic timer = new Timer(TimerProc);
            timer.Change(msDelay, Timeout.Infinite);
        }

        private void TimerProc(object state)
        {
            try
            {
                // The state object is the Timer object. 
                ((Timer)state).Dispose();
                m_Action.Invoke();
            }
            catch (Exception ex)
            {
                Tracer.Error(ex);
            }
        }

        public static DelayedAction Start(Action callback, int msDelay)
        {
            return new DelayedAction(callback, msDelay);
        }
    }
}
