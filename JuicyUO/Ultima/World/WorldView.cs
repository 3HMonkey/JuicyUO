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
using JuicyUO.Core.Graphics;
using JuicyUO.Core.Patterns.MVC;
using JuicyUO.Core.UI;
using JuicyUO.Ultima.UI.WorldGumps;
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Ultima.World.Entities.Mobiles;
using JuicyUO.Ultima.World.EntityViews;
using JuicyUO.Ultima.World.WorldViews;
#endregion

namespace JuicyUO.Ultima.World
{
    class WorldView : AView
    {
        public IsometricRenderer Isometric
        {
            get;
            private set;
        }

        public MiniMapTexture MiniMap
        {
            get;
            private set;
        }

        protected new WorldModel Model
        {
            get { return (WorldModel)base.Model; }
        }

        /// <summary>
        ///  When AllLabels is true, all entites should display their name above their object.
        /// </summary>
        public static bool AllLabels
        {
            get;
            set;
        }

        public static int MouseOverHue = 0x038;

        public WorldView(WorldModel model)
            : base(model)
        {
            Isometric = new IsometricRenderer();
            Isometric.Lighting.LightDirection = -0.6f;

            MiniMap = new MiniMapTexture();
            MiniMap.Initialize();

            m_UI = Service.Get<UserInterfaceService>();
        }

        public override void Draw(double frameTime)
        {
            AEntity player = WorldModel.Entities.GetPlayerEntity();
            if (player == null)
                return;
            if (Model.Map == null)
                return;

            Position3D center = player.Position;
            if ((player as Mobile).IsAlive)
            {
                AEntityView.Technique = Techniques.Default;
                m_ShowingDeathEffect = false;
                if (m_YouAreDead != null)
                {
                    m_YouAreDead.Dispose();
                    m_YouAreDead = null;
                }
            }
            else
            {
                if (!m_ShowingDeathEffect)
                {
                    m_ShowingDeathEffect = true;
                    m_DeathEffectTime = 0;
                    m_LightingGlobal = Isometric.Lighting.OverallLightning;
                    m_LightingPersonal = Isometric.Lighting.PersonalLightning;
                    m_UI.AddControl(m_YouAreDead = new YouAreDeadGump(), 0, 0);
                }

                double msFade = 2000d;
                double msHold = 1000d;

                if (m_DeathEffectTime < msFade)
                {
                    AEntityView.Technique = Techniques.Default;
                    Isometric.Lighting.OverallLightning = (int)(m_LightingGlobal + (0x1f - m_LightingGlobal) * ((m_DeathEffectTime / msFade)));
                    Isometric.Lighting.PersonalLightning = (int)(m_LightingPersonal * (1d - (m_DeathEffectTime / msFade)));
                }
                else if (m_DeathEffectTime < msFade + msHold)
                {
                    Isometric.Lighting.OverallLightning = 0x1f;
                    Isometric.Lighting.PersonalLightning = 0x00;
                }
                else
                {
                    AEntityView.Technique = Techniques.Grayscale;
                    Isometric.Lighting.OverallLightning = (int)m_LightingGlobal;
                    Isometric.Lighting.PersonalLightning = (int)m_LightingPersonal;
                    if (m_YouAreDead != null)
                    {
                        m_YouAreDead.Dispose();
                        m_YouAreDead = null;
                    }
                }

                m_DeathEffectTime += frameTime;
            }

            Isometric.Update(Model.Map, center, Model.Input.MousePick);
            MiniMap.Update(Model.Map, center);
        }

        private bool m_ShowingDeathEffect;
        private double m_DeathEffectTime;
        private double m_LightingGlobal;
        private double m_LightingPersonal;
        private YouAreDeadGump m_YouAreDead;

        private UserInterfaceService m_UI;
    }
}
