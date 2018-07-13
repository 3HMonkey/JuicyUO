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
using System.Collections.Generic;
using System.Text;
using JuicyUO.Core.Input;
using JuicyUO.Ultima.Player;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.World;
#endregion

namespace JuicyUO.Ultima.UI.WorldGumps
{
    class SkillsGump : Gump
    {
        // Private variables
        private ExpandableScroll m_Background;
        private HtmlGumpling m_SkillsHtml;
        private bool m_MustUpdateSkills = true;

        // Services
        private WorldModel m_World;

        public SkillsGump()
            : base(0, 0)
        {
            IsMoveable = true;

            m_World = Service.Get<WorldModel>();

            AddControl(m_Background = new ExpandableScroll(this, 0, 0, 200));
            m_Background.TitleGumpID = 0x834;

            AddControl(m_SkillsHtml = new HtmlGumpling(this, 32, 32, 240, 200 - 92, 0, 2, string.Empty));
        }

        protected override void OnInitialize()
        {
            SetSavePositionName("skills");
            PlayerState.Skills.OnSkillChanged += OnSkillChanged;
        }

        public override void Dispose()
        {
            PlayerState.Skills.OnSkillChanged -= OnSkillChanged;
            base.Dispose();
        }

        public override void Update(double totalMS, double frameMS)
        {
            base.Update(totalMS, frameMS);
            if (m_MustUpdateSkills)
            {
                InitializeSkillsList();
                m_MustUpdateSkills = false;
            }
            m_SkillsHtml.Height = Height - 92;
        }

        public override void OnHtmlInputEvent(string href, MouseEvent e)
        {
            if (e == MouseEvent.Click)
            {
                if (href.Substring(0, 6) == "skill=" || href.Substring(0, 9) == "skillbtn=")
                {
                    int skillIndex;
                    if (!int.TryParse(href.Substring(href.IndexOf('=') + 1), out skillIndex))
                        return;
                    m_World.Interaction.UseSkill(skillIndex);
                }
                else if (href.Substring(0, 10) == "skilllock=")
                {
                    int skillIndex;
                    if (!int.TryParse(href.Substring(10), out skillIndex))
                        return;
                    m_World.Interaction.ChangeSkillLock(PlayerState.Skills.SkillEntryByIndex(skillIndex));
                }
            }
            else if (e == MouseEvent.DragBegin)
            {
                if (href.Length >= 9 && href.Substring(0, 9) == "skillbtn=")
                {
                    int skillIndex;
                    if (!int.TryParse(href.Substring(9), out skillIndex))
                        return;
                    SkillEntry skill = PlayerState.Skills.SkillEntryByIndex(skillIndex);
                    IInputService input = Service.Get<IInputService>();
                    UseSkillButtonGump gump = new UseSkillButtonGump(skill);
                    UserInterface.AddControl(gump, input.MousePosition.X - 60, input.MousePosition.Y - 20);
                    UserInterface.AttemptDragControl(gump, input.MousePosition, true);
                }
            }
        }

        private void InitializeSkillsList()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<int, SkillEntry> pair in PlayerState.Skills.List)
            {
                SkillEntry skill = pair.Value;
                sb.Append(string.Format(skill.HasUseButton ? kSkillName_HasUseButton : kSkillName_NoUseButton, skill.Index, skill.Name));
                sb.Append(string.Format(kSkillValues[skill.LockType], skill.Value, skill.Index));
            }
            m_SkillsHtml.Text = sb.ToString();
        }

        private void OnSkillChanged(SkillEntry entry)
        {
            m_MustUpdateSkills = true;
        }

        // 0 = skill index, 1 = skill name
        const string kSkillName_HasUseButton = 
            "<left><a href='skillbtn={0}'><gumpimg src='2103' hoversrc='2104' activesrc='2103'/></a> " +
            "<a href='skill={0}' color='#5b4f29' hovercolor='#857951' activecolor='#402708' style='text-decoration=none'>{1}</a></left>";
        const string kSkillName_NoUseButton = "<left>   <medium color=#50422D>{1}</medium></left>";
        // 0 = skill value
        static string[] kSkillValues = {
            "<right><medium color=#50422D>{0:0.0}</medium><a href='skilllock={1}'><gumpimg src='2436'/></a> </right><br/>",
            "<right><medium color=#50422D>{0:0.0}</medium><a href='skilllock={1}'><gumpimg src='2438'/></a> </right><br/>",
            "<right><medium color=#50422D>{0:0.0}</medium><a href='skilllock={1}'><gumpimg src='2092'/></a> </right><br/>" };
    }
}
