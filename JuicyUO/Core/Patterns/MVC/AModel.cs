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

namespace JuicyUO.Core.Patterns.MVC
{
    /// <summary>
    /// Abstract Model. Maintains the state, core data, and update logic of a model.
    /// </summary>
    public abstract class AModel
    {
        bool m_IsInitialized;
        AView m_View;
        AController m_Controller;

        public AView GetView()
        {
            if (m_View == null)
            {
                m_View = CreateView();
            }
            return m_View;
        }
        
        public AController GetController()
        {
            if (m_Controller == null)
            {
                m_Controller = CreateController();
            }
            return m_Controller;
        }

        public void Initialize()
        {
            if (m_IsInitialized)
            {
                return;
            }
            m_IsInitialized = true;
            OnInitialize();
        }

        public void Dispose()
        {
            OnDispose();
        }

        public abstract void Update(double totalTime, double frameTime);

        protected abstract AView CreateView();
        protected abstract void OnInitialize();
        protected abstract void OnDispose();

        protected virtual AController CreateController()
        {
            throw new NotImplementedException();
        }
    }
}
