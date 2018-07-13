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

using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace JuicyUO.Core.Diagnostics
{
    static class Metrics
    {
        static readonly List<NameValuePair> m_dataReadList = new List<NameValuePair>();
        public static int TotalDataRead
        {
            get
            {
                int total = 0;
                foreach (NameValuePair p in m_dataReadList)
                    total += p.Value;
                return total;
            }
        }

        static string m_dataReadBreakdown;
        static bool m_dataReadBreakdown_MustUpdate = true;
        public static string DataReadBreakdown
        {
            get
            {
                if (m_dataReadBreakdown_MustUpdate)
                {
                    m_dataReadBreakdown_MustUpdate = false;
                    m_dataReadBreakdown = "Data Read from HDD:";
                    foreach (NameValuePair p in m_dataReadList)
                        m_dataReadBreakdown += '\n' + p.Name + ": " + p.Value;
                }
                return m_dataReadBreakdown;
            }
        }

        public static void ReportDataRead(int dataAmount)
        {
            string name;
#if DEBUG
            StackTrace stackTrace = new StackTrace();
            StackFrame stackFrame = stackTrace.GetFrame(1);
            MethodBase methodBase = stackFrame.GetMethod();
            name = methodBase.DeclaringType.FullName;
#else
            name = "Total";
#endif
            bool mustAddPair = true;
            foreach (NameValuePair p in m_dataReadList)
            {
                if (p.Name == name)
                {
                    mustAddPair = false;
                    p.Value += dataAmount;
                }
            }
            if (mustAddPair)
            {
                m_dataReadList.Add(new NameValuePair(name, dataAmount));
            }
            m_dataReadBreakdown_MustUpdate = true;
        }
    }

    class NameValuePair
    {
        public string Name = string.Empty;
        public int Value;

        public NameValuePair(string name, int value)
        {
            Name = name;
            Value = value;
        }
    }
}
