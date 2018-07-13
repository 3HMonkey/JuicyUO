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

using System.Runtime.InteropServices;

namespace JuicyUO.Core.Diagnostics
{
    /// <summary>
    /// A high resolution query performance timer.
    /// </summary>
    public class HighPerformanceTimer
    {
        #region Imported Methods
        /// <summary>
        /// The current system ticks (count).
        /// </summary>
        /// <param name="lpPerformanceCount">Current performance count of the system.</param>
        /// <returns>False on failure.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        /// <summary>
        /// Ticks per second (frequency) that the high performance counter performs.
        /// </summary>
        /// <param name="lpFrequency">Frequency the higher performance counter performs.</param>
        /// <returns>False if the high performance counter is not supported.</returns>
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
        #endregion

        #region Member Variables
        private long m_StartTime;
        #endregion

        public HighPerformanceTimer()
        {
        }

        #region Methods
        public void Start()
        {
            // Record when the timer was started.
            m_StartTime = HighPerformanceTimer.Counter;
        }

        public static double SecondsFromTicks(long ticks)
        {
            return ((double)ticks) / HighPerformanceTimer.Frequency;
        }
        #endregion

        #region Static Properties
        private static long frequency;

        static HighPerformanceTimer()
        {
            QueryPerformanceFrequency(out HighPerformanceTimer.frequency);
        }

        /// <summary>
        /// Gets the frequency that this HighPerformanceTimer performs at.
        /// </summary>
        public static long Frequency
        {
            get
            {
                return HighPerformanceTimer.frequency;
            }
        }

        /// <summary>
        /// Gets the current system ticks.
        /// </summary>
        public static long Counter
        {
            get
            {
                long ticks = 0;
                QueryPerformanceCounter(out ticks);
                return ticks;
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets the tick count of when this HighPerformanceTimer was started.
        /// </summary>
        public long StartTime
        {
            get
            {
                return m_StartTime;
            }
        }

        public long ElapsedTicks
        {
            get
            {
                return HighPerformanceTimer.Counter - m_StartTime;
            }
        }

        public double ElapsedSeconds
        {
            get
            {
                return ((double)ElapsedTicks) / HighPerformanceTimer.Frequency;
            }
        }
        #endregion
    }
}
