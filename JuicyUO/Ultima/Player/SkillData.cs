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
using JuicyUO.Ultima.Resources;
#endregion

namespace JuicyUO.Ultima.Player
{
    public class SkillData
    {
        public Action<SkillEntry> OnSkillChanged;

        private readonly Dictionary<int, SkillEntry> m_Skills = new Dictionary<int, SkillEntry>();
        private bool m_SkillsLoaded;

        public Dictionary<int, SkillEntry> List
        {
            get
            {
                if (!m_SkillsLoaded)
                {
                    m_SkillsLoaded = true;
                    foreach (Skill skill in SkillsData.List)
                        if (skill.Index == -1)
                        {
                            // do nothing.
                        }
                        else
                        {
                            m_Skills.Add(skill.ID, new SkillEntry(this, skill.ID, skill.Index, skill.UseButton, skill.Name, 0.0f, 0.0f, 0, 0.0f));
                        }
                }
                return m_Skills;
            }
        }

        public SkillEntry SkillEntry(int skillID)
        {
            if (List.Count > skillID)
                return List[skillID];
            else
                return null;
        }

        public SkillEntry SkillEntryByIndex(int index)
        {
            foreach (SkillEntry skill in m_Skills.Values)
                if (skill.Index == index)
                    return skill;
            return null;
        }
    }

    public class SkillEntry
    {
        private readonly SkillData m_DataParent;

        private int m_id;
        private int m_index;
        private bool m_hasUseButton;
        private string m_name;
        private float m_value;
        private float m_valueUnmodified;
        private byte m_lockType;
        private float m_cap;

        public int ID
        {
            get { return m_id; }
            set { m_id = value; }
        }
        public int Index
        {
            get { return m_index; }
            set { m_index = value; }
        }
        public bool HasUseButton
        {
            get { return m_hasUseButton; }
            set { m_hasUseButton = value; }
        }
        public string Name
        {
            get { return m_name; }
            set { m_name = value; }
        }
        public float Value
        {
            get { return m_value; }
            set
            {
                m_value = value;
                m_DataParent.OnSkillChanged?.Invoke(this);
            }
        }
        public float ValueUnmodified
        {
            get { return m_valueUnmodified; }
            set
            {
                m_valueUnmodified = value;
                m_DataParent.OnSkillChanged?.Invoke(this);
            }
        }
        public byte LockType
        {
            get { return m_lockType; }
            set
            {
                m_lockType = value;
                m_DataParent.OnSkillChanged?.Invoke(this);
            }
        }
        public float Cap
        {
            get { return m_cap; }
            set
            {
                m_cap = value;
                m_DataParent.OnSkillChanged?.Invoke(this);
            }
        }

        public SkillEntry(SkillData dataParent, int id, int index, bool useButton, string name, float value, float unmodified, byte locktype, float cap)
        {
            m_DataParent = dataParent;
            ID = id;
            Index = index;
            HasUseButton = useButton;
            Name = name;
            Value = value;
            ValueUnmodified = unmodified;
            LockType = locktype;
            Cap = cap;
        }
    }
}
