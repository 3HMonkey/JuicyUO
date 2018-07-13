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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Input;
using JuicyUO.Core.Resources;
using JuicyUO.Core.UI;
#endregion

namespace JuicyUO.Ultima.UI.Controls
{
    enum HSliderBarStyle
    {
        MetalWidgetRecessedBar,
        BlueWidgetNoBar
    }

    class HSliderBar : AControl
    {
        Texture2D[] m_GumpSliderBackground;
        Texture2D m_GumpWidget;

        // we use m_newValue to (a) get delta, (b) so Value only changes once per frame.
        int m_newValue, m_value;
        public int Value
        {
            get
            {
                return m_value;
            }
            set
            {
                m_value = m_newValue = value;
                if (IsInitialized)
                    RecalculateSliderX();
            }
        }

        private void RecalculateSliderX()
        {
            m_sliderX = (int)((float)(BarWidth - m_GumpWidget.Width) * ((float)(Value - MinValue) / (float)(MaxValue - MinValue)));
        }

        public int MinValue;
        public int MaxValue;
        public int BarWidth;

        private int m_sliderX;
        private HSliderBarStyle Style;

        HSliderBar(AControl parent)
            : base(parent)
        {
            HandlesMouseInput = true;
            m_pairedSliders = new List<HSliderBar>();
        }

        public HSliderBar(AControl parent, int x, int y, int width, int minValue, int maxValue, int value, HSliderBarStyle style)
            : this(parent)
        {
            BuildGumpling(x, y, width, minValue, maxValue, value, style);
        }

        void BuildGumpling(int x, int y, int width, int minValue, int maxValue, int value, HSliderBarStyle style)
        {
            Position = new Point(x, y);
            MinValue = minValue;
            MaxValue = maxValue;
            BarWidth = width;
            Value = value;
            Style = style;
        }

        public override void Update(double totalMS, double frameMS)
        {
            if (m_GumpWidget == null)
            {
                IResourceProvider provider = Service.Get<IResourceProvider>();
                switch (Style)
                {
                    default:
                    case HSliderBarStyle.MetalWidgetRecessedBar:
                        m_GumpSliderBackground = new Texture2D[3];
                        m_GumpSliderBackground[0] = provider.GetUITexture(213);
                        m_GumpSliderBackground[1] = provider.GetUITexture(214);
                        m_GumpSliderBackground[2] = provider.GetUITexture(215);
                        m_GumpWidget = provider.GetUITexture(216);
                        break;
                    case HSliderBarStyle.BlueWidgetNoBar:
                        m_GumpWidget = provider.GetUITexture(0x845);
                        break;
                }
                Size = new Point(BarWidth, m_GumpWidget.Height);
                RecalculateSliderX();
            }
            
            modifyPairedValues(m_newValue - Value);
            m_value = m_newValue;


            base.Update(totalMS, frameMS);
        }

        public override void Draw(SpriteBatchUI spriteBatch, Point position, double frameMS)
        {
            if (m_GumpSliderBackground != null)
            {
                spriteBatch.Draw2D(m_GumpSliderBackground[0], new Vector3(position.X, position.Y, 0), Vector3.Zero);
                spriteBatch.Draw2DTiled(m_GumpSliderBackground[1], new Rectangle(position.X + m_GumpSliderBackground[0].Width, position.Y, BarWidth - m_GumpSliderBackground[2].Width - m_GumpSliderBackground[0].Width, m_GumpSliderBackground[1].Height), Vector3.Zero);
                spriteBatch.Draw2D(m_GumpSliderBackground[2], new Vector3(position.X + BarWidth - m_GumpSliderBackground[2].Width, position.Y, 0), Vector3.Zero);
            }
            spriteBatch.Draw2D(m_GumpWidget, new Vector3(position.X + m_sliderX, position.Y, 0), Vector3.Zero);
            base.Draw(spriteBatch, position, frameMS);
        }

        protected override bool IsPointWithinControl(int x, int y)
        {
            if (new Rectangle(m_sliderX, 0, m_GumpWidget.Width, m_GumpWidget.Height).Contains(new Point(x, y)))
                return true;
            else
                return false;
        }

        bool m_clicked;
        Point m_clickPosition;

        protected override void OnMouseDown(int x, int y, MouseButton button)
        {
            m_clicked = true;
            m_clickPosition = new Point(x, y);
        }

        protected override void OnMouseUp(int x, int y, MouseButton button)
        {
            m_clicked = false;
        }

        protected override void OnMouseOver(int x, int y)
        {
            if (m_clicked)
            {
                m_sliderX = m_sliderX + (x - m_clickPosition.X);
                if (m_sliderX < 0)
                    m_sliderX = 0;
                if (m_sliderX > BarWidth - m_GumpWidget.Width)
                    m_sliderX = BarWidth - m_GumpWidget.Width;
                m_clickPosition = new Point(x, y);
                if (m_clickPosition.X < m_GumpWidget.Width / 2)
                    m_clickPosition.X = m_GumpWidget.Width / 2;
                if (m_clickPosition.X > BarWidth - m_GumpWidget.Width / 2)
                    m_clickPosition.X = BarWidth - m_GumpWidget.Width / 2;
                m_newValue = (int)(((float)m_sliderX / (float)(BarWidth - m_GumpWidget.Width)) * (float)((MaxValue - MinValue))) + MinValue;
            }
        }

        readonly List<HSliderBar> m_pairedSliders;
        public void PairSlider(HSliderBar s)
        {
            m_pairedSliders.Add(s);
        }

        void modifyPairedValues(int delta)
        {
            if (m_pairedSliders.Count == 0)
                return;

            bool updateSinceLastCycle = true;
            int d = (delta > 0) ? -1 : 1;
            int points = Math.Abs(delta);
            int sliderIndex = Value % m_pairedSliders.Count;
            while (points > 0)
            {
                if (d > 0)
                {
                    if (m_pairedSliders[sliderIndex].Value < m_pairedSliders[sliderIndex].MaxValue)
                    {
                        updateSinceLastCycle = true;
                        m_pairedSliders[sliderIndex].Value += d;
                        points--;
                    }
                }
                else
                {
                    if (m_pairedSliders[sliderIndex].Value > m_pairedSliders[sliderIndex].MinValue)
                    {
                        updateSinceLastCycle = true;
                        m_pairedSliders[sliderIndex].Value += d;
                        points--;
                    }
                }
                sliderIndex++;
                if (sliderIndex == m_pairedSliders.Count)
                {
                    if (!updateSinceLastCycle)
                        return;
                    updateSinceLastCycle = false;
                    sliderIndex = 0;
                }
            }
        }
    }
}
