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
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using JuicyUO.Configuration.Properties;
using JuicyUO.Core.Extensions;
using JuicyUO.Core.Input;
using JuicyUO.Core.Network;
using JuicyUO.Core.UI;
using JuicyUO.Core.Windows;
using JuicyUO.Ultima.Data;
using JuicyUO.Ultima.Input;
using JuicyUO.Ultima.Network.Client;
using JuicyUO.Ultima.Player;
using JuicyUO.Ultima.UI.Controls;
using JuicyUO.Ultima.UI.WorldGumps;
using JuicyUO.Ultima.World.Entities;
using JuicyUO.Ultima.World.Entities.Items;
using JuicyUO.Ultima.World.Entities.Mobiles;
#endregion

namespace JuicyUO.Ultima.World.Input
{
    /// <summary>
    /// Handles all the mouse input when the mouse is over the world.
    /// </summary>
    class WorldInput
    {
        const double c_PauseBeforeMouseMovementMS = 105d;
        const double c_PauseBeforeKeyboardFacingMS = 55d; // a little more than three frames @ 60fps.
        const double c_PauseBeforeKeyboardMovementMS = 125d; // a little more than seven frames @ 60fps.
        bool m_ContinuousMouseMovementCheck;

        AEntity m_DraggingEntity;

        INetworkClient m_Network;
        UserInterfaceService m_UserInterface;
        IInputService m_Input;

        // keyboard movement variables.
        double m_PauseBeforeKeyboardMovementMS;

        double m_TimeSinceMovementButtonPressed;
        Point m_DragOffset;

        protected WorldModel World
        {
            get;
            private set;
        }

        MacroEngine m_Macros;

        public WorldInput(WorldModel world)
        {
            // parent reference
            World = world;

            // service references
            m_Network = Service.Get<INetworkClient>();
            m_UserInterface = Service.Get<UserInterfaceService>();
            m_Input = Service.Get<IInputService>();

            // local instances
            MousePick = new MousePicking();
            m_Macros = new MacroEngine();
        }

        public MousePicking MousePick
        {
            get;
            private set;
        }

        public bool ContinuousMouseMovementCheck
        {
            get { return m_ContinuousMouseMovementCheck; }
            set
            {
                if (m_ContinuousMouseMovementCheck != value)
                {
                    m_ContinuousMouseMovementCheck = value;
                    if (m_ContinuousMouseMovementCheck)
                    {
                        m_UserInterface.AddInputBlocker(this);
                    }
                    else
                    {
                        m_UserInterface.RemoveInputBlocker(this);
                    }
                }
            }
        }

        public bool IsMouseOverUI
        {
            get
            {
                if (m_UserInterface.IsMouseOverUI)
                {
                    AControl over = m_UserInterface.MouseOverControl;
                    return !(over is WorldViewport);
                }
                return false;
            }
        }

        public bool IsMouseOverWorld
        {
            get
            {
                if (m_UserInterface.IsMouseOverUI)
                {
                    AControl over = m_UserInterface.MouseOverControl;
                    return (over is WorldViewport);
                }
                return false;
            }
        }

        public Point MouseOverWorldPosition
        {
            get
            {
                WorldViewport world = Service.Get<WorldViewport>();
                Point mouse = new Point(m_Input.MousePosition.X - world.ScreenX, m_Input.MousePosition.Y - world.ScreenY);
                return mouse;
            }
        }

        public void Dispose()
        {
            m_UserInterface.RemoveInputBlocker(this);
        }

        public void Update(double frameMS)
        {
            if (WorldModel.IsInWorld && !m_UserInterface.IsModalControlOpen && m_Network.IsConnected)
            {
                // always parse keyboard. (Is it possible there are some situations in which keyboard input is blocked???)
                InternalParseKeyboard(frameMS);

                // In all cases, where we are moving and the move button was released, stop moving.
                if (ContinuousMouseMovementCheck &&
                   m_Input.HandleMouseEvent(MouseEvent.Up, Settings.UserInterface.Mouse.MovementButton))
                {
                    ContinuousMouseMovementCheck = false;
                }

                if (IsMouseOverWorld)
                {
                    InternalParseMouse(frameMS);
                }

                // PickType is the kind of objects that will show up as the 'MouseOverObject'
                if (IsMouseOverWorld)
                {
                    MousePick.PickOnly = PickType.PickEverything;
                    MousePick.Position = MouseOverWorldPosition;
                    if (Settings.UserInterface.PlayWindowPixelDoubling)
                        MousePick.Position = MousePick.Position.DivideBy(2);
                }
                else
                {
                    MousePick.PickOnly = PickType.PickNothing;
                }

                doMouseMovement(frameMS);
            }
            else
            {
                // the world is not receiving input this frame. get rid of any mouse picking data.
                MousePick.PickOnly = PickType.PickNothing;
            }

            m_Macros.Update(frameMS);
        }

        void doMouseMovement(double frameMS)
        {
            Mobile player = (Mobile)WorldModel.Entities.GetPlayerEntity();
            if (player == null)
                return;

            // if the move button is pressed, change facing and move based on mouse cursor direction.
            if (ContinuousMouseMovementCheck)
            {
                ResolutionProperty resolution = Settings.UserInterface.PlayWindowGumpResolution;
                Point centerScreen = new Point(resolution.Width / 2, resolution.Height / 2);
                Direction mouseDirection = DirectionHelper.DirectionFromPoints(centerScreen, MouseOverWorldPosition);

                m_TimeSinceMovementButtonPressed += frameMS;

                if (m_TimeSinceMovementButtonPressed >= c_PauseBeforeMouseMovementMS)
                {
                    // Get the move direction.
                    Direction moveDirection = mouseDirection;

                    // add the running flag if the mouse cursor is far enough away from the center of the screen.
                    float distanceFromCenterOfScreen = Utility.DistanceBetweenTwoPoints(centerScreen, MouseOverWorldPosition);

                    if (distanceFromCenterOfScreen >= 150.0f || Settings.UserInterface.AlwaysRun)
                    {
                        moveDirection |= Direction.Running;
                    }

                    player.PlayerMobile_Move(moveDirection);
                }
                else
                {
                    // Get the move direction.
                    Direction facing = mouseDirection;
                    if (player.Facing != facing)
                    {
                        // Tell the player entity to change facing to this direction.
                        player.PlayerMobile_ChangeFacing(facing);
                        // reset the time since the mouse cursor was pressed - allows multiple facing changes.
                        m_TimeSinceMovementButtonPressed = 0d;
                    }
                }
            }
            else
            {
                m_TimeSinceMovementButtonPressed = 0d;
                // Tell the player to stop moving.
                player.PlayerMobile_Move(Direction.Nothing);
            }
        }

        void doKeyboardMovement(double frameMS)
        {
            Mobile player = (Mobile)WorldModel.Entities.GetPlayerEntity();
            if (player == null)
                return;

            if (m_PauseBeforeKeyboardMovementMS < c_PauseBeforeKeyboardMovementMS)
            {
                if (m_Input.HandleKeyboardEvent(KeyboardEvent.Up, WinKeys.Up, false, false, false))
                {
                    m_PauseBeforeKeyboardMovementMS = 0;
                }
                if (m_Input.HandleKeyboardEvent(KeyboardEvent.Up, WinKeys.Down, false, false, false))
                {
                    m_PauseBeforeKeyboardMovementMS = 0;
                }
                if (m_Input.HandleKeyboardEvent(KeyboardEvent.Up, WinKeys.Left, false, false, false))
                {
                    m_PauseBeforeKeyboardMovementMS = 0;
                }
                if (m_Input.HandleKeyboardEvent(KeyboardEvent.Up, WinKeys.Right, false, false, false))
                {
                    m_PauseBeforeKeyboardMovementMS = 0;
                }
            }

            bool up = m_Input.IsKeyDown(WinKeys.Up);
            bool left = m_Input.IsKeyDown(WinKeys.Left);
            bool right = m_Input.IsKeyDown(WinKeys.Right);
            bool down = m_Input.IsKeyDown(WinKeys.Down);
            if (up | left | right | down)
            {
                // Allow a short span of time (50ms) to get all the keys pressed.
                // Otherwise, when moving diagonally, we would only get the first key
                // in most circumstances and the second key a frame or two later - but
                // too late, we would already be moving in the non-diagonal direction :(
                m_PauseBeforeKeyboardMovementMS += frameMS;
                if (m_PauseBeforeKeyboardMovementMS >= c_PauseBeforeKeyboardFacingMS)
                {
                    Direction facing = Direction.Up;
                    if (up)
                    {
                        if (left)
                        {
                            facing = Direction.West;
                        }
                        else if (m_Input.IsKeyDown(WinKeys.Right))
                        {
                            facing = Direction.North;
                        }
                        else
                        {
                            facing = Direction.Up;
                        }
                    }
                    else if (down)
                    {
                        if (left)
                        {
                            facing = Direction.South;
                        }
                        else if (right)
                        {
                            facing = Direction.East;
                        }
                        else
                        {
                            facing = Direction.Down;
                        }
                    }
                    else
                    {
                        if (left)
                        {
                            facing = Direction.Left;
                        }
                        else if (right)
                        {
                            facing = Direction.Right;
                        }
                    }

                    // only send messages if we're not moving.
                    if (!player.IsMoving)
                    {
                        if (m_PauseBeforeKeyboardMovementMS >= c_PauseBeforeKeyboardMovementMS)
                        {
                            player.PlayerMobile_Move(facing);
                        }
                        else
                        {
                            if (player.Facing != facing)
                            {
                                player.PlayerMobile_ChangeFacing(facing);
                            }
                        }
                    }
                }
            }
            else
            {
                m_PauseBeforeKeyboardMovementMS = 0;
            }
        }

        void onMoveButton(InputEventMouse e)
        {
            if (e.EventType == MouseEvent.Down)
            {
                // keep moving as long as the move button is down.
                ContinuousMouseMovementCheck = true;
            }
            else if (e.EventType == MouseEvent.Up)
            {
                // If the movement mouse button has been released, stop moving.
                ContinuousMouseMovementCheck = false;
            }

            e.Handled = true;
        }

        void onInteractButton(InputEventMouse e, AEntity overEntity, Point overEntityPoint)
        {
            if (e.EventType == MouseEvent.Down)
            {
                // prepare to pick this item up.
                m_DraggingEntity = overEntity;
                m_DragOffset = overEntityPoint;
            }
            else if (e.EventType == MouseEvent.Click)
            {
                if (overEntity is Ground)
                {
                    // no action.
                }
                else if (overEntity is StaticItem)
                {
                    // pop up name of item.
                    overEntity.AddOverhead(MessageTypes.Label, overEntity.Name, 3, 0, false);
                    WorldModel.Statics.AddStaticThatNeedsUpdating(overEntity as StaticItem);
                }
                else if (overEntity is Item)
                {
                    World.Interaction.SingleClick(overEntity);
                }
                else if (overEntity is Mobile)
                {
                    World.Interaction.SingleClick(overEntity);
                }
            }
            else if (e.EventType == MouseEvent.DoubleClick)
            {
                if (overEntity is Ground)
                {
                    // no action.
                }
                else if (overEntity is StaticItem)
                {
                    // no action.
                }
                else if (overEntity is Item)
                {
                    // request context menu
                    World.Interaction.DoubleClick(overEntity);
                }
                else if (overEntity is Mobile)
                {
                    // Send double click packet.
                    // Set LastTarget == targeted Mobile.
                    // If in WarMode, set Attacking == true.
                    Mobile mobile = overEntity as Mobile;
                    World.Interaction.LastTarget = overEntity.Serial;

                    if (WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode)
                    {
                        World.Interaction.AttackRequest(mobile);
                    }
                    else
                    {
                        World.Interaction.DoubleClick(overEntity);
                    }
                }
            }
            else if (e.EventType == MouseEvent.DragBegin)
            {
                if (overEntity is Ground)
                {
                    // no action.
                }
                else if (overEntity is StaticItem)
                {
                    // no action.
                }
                else if (overEntity is Item)
                {
                    // attempt to pick up item.
                    World.Interaction.PickupItem((Item)overEntity, new Point((int)m_DragOffset.X, (int)m_DragOffset.Y));
                }
                else if (overEntity is Mobile)
                {
                    if (PlayerState.Partying.GetMember(overEntity.Serial) != null)//is he in your party// number of 0x11 packet dont have information about stamina/mana k(IMPORTANT!!!)
                        return;
                    // request basic stats - gives us the name rename flag
                    m_Network.Send(new MobileQueryPacket(MobileQueryPacket.StatusType.BasicStatus, overEntity.Serial));
                    // drag off a status gump for this mobile.
                    MobileHealthTrackerGump gump = new MobileHealthTrackerGump(overEntity as Mobile);
                    m_UserInterface.AddControl(gump, e.X - 77, e.Y - 30);
                    m_UserInterface.AttemptDragControl(gump, new Point(e.X, e.Y), true);
                }
            }

            e.Handled = true;
        }

        void InternalParseMouse(double frameMS)
        {
            List<InputEventMouse> events = m_Input.GetMouseEvents();
            foreach (InputEventMouse e in events)
            {
                if (e.Button == Settings.UserInterface.Mouse.MovementButton)
                {
                    onMoveButton(e);
                }
                else if (e.Button == Settings.UserInterface.Mouse.InteractionButton)
                {
                    if (e.EventType == MouseEvent.Click)
                    {
                        InternalQueueSingleClick(e, MousePick.MouseOverObject, MousePick.MouseOverObjectPoint);
                        continue;
                    }
                    if (e.EventType == MouseEvent.DoubleClick)
                    {
                        ClearQueuedClick();
                    }
                    onInteractButton(e, MousePick.MouseOverObject, MousePick.MouseOverObjectPoint);
                }
            }

            InternalCheckQueuedClick(frameMS);
        }

        void InternalParseKeyboard(double frameMS)
        {
            // macros
            doMacroInput(m_Input.GetKeyboardEvents());

            // all names mode
            WorldView.AllLabels = (m_Input.IsShiftDown && m_Input.IsCtrlDown);

            // Warmode toggle: (maybe moving to macro manager)
            if (m_Input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.Tab, false, false, false))
            {
                m_Network.Send(new RequestWarModePacket(!WorldModel.Entities.GetPlayerEntity().Flags.IsWarMode));
            }

            // Toggle minimap. Default is Alt-R.
            if (m_Input.HandleKeyboardEvent(KeyboardEvent.Down, WinKeys.R, false, true, false))
            {
                MiniMapGump.Toggle();
            }

            // movement with arrow keys if the player is not moving and the mouse isn't moving the player.
            if (!ContinuousMouseMovementCheck)
            {
                doKeyboardMovement(frameMS);
            }

            // FPS limiting
            if (m_Input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.F, false, true, false))
            {
                Settings.Engine.IsFixedTimeStep = !Settings.Engine.IsFixedTimeStep;
            }

            // Display FPS
            if (m_Input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.F, false, true, true))
            {
                Settings.Debug.ShowFps = !Settings.Debug.ShowFps;
            }

            // Mouse enable / disable
            if (m_Input.HandleKeyboardEvent(KeyboardEvent.Press, WinKeys.M, false, true, false))
            {
                Settings.UserInterface.Mouse.IsEnabled = !Settings.UserInterface.Mouse.IsEnabled;
            }
        }

        void doMacroInput(List<InputEventKeyboard> events)
        {
            foreach (InputEventKeyboard e in events)
            {
                foreach (Action action in Macros.Player.All)
                {
                    if (e.EventType == KeyboardEvent.Press &&
                        action.Keystroke == e.KeyCode &&
                        action.Alt == e.Alt &&
                        action.Ctrl == e.Control &&
                        action.Shift == e.Shift)
                    {
                        m_Macros.Run(action);
                        e.Handled = true;
                    }
                }
            }
        }

        #region QueuedClicks

        // Legacy Client waits about 0.5 seconds before sending a click event when you click in the world.
        // This allows time for the player to potentially double-click on an object.
        // If the player does so, this will cancel the single-click event.
        AEntity m_QueuedEntity;
        Point m_QueuedEntityPosition;
        InputEventMouse m_QueuedEvent;
        double m_QueuedEvent_DequeueAt;
        bool m_QueuedEvent_InQueue;

        void ClearQueuedClick()
        {
            m_QueuedEvent_InQueue = false;
            m_QueuedEvent = null;
            m_QueuedEntity = null;
        }

        void InternalCheckQueuedClick(double frameMS)
        {
            if (m_QueuedEvent_InQueue)
            {
                m_QueuedEvent_DequeueAt -= frameMS;
                if (m_QueuedEvent_DequeueAt <= 0d)
                {
                    onInteractButton(m_QueuedEvent, m_QueuedEntity, m_QueuedEntityPosition);
                    ClearQueuedClick();
                }
            }
        }

        void InternalQueueSingleClick(InputEventMouse e, AEntity overEntity, Point overEntityPoint)
        {
            m_QueuedEvent_InQueue = true;
            m_QueuedEntity = overEntity;
            m_QueuedEntityPosition = overEntityPoint;
            m_QueuedEvent_DequeueAt = Settings.UserInterface.Mouse.DoubleClickMS;
            m_QueuedEvent = e;
        }

        #endregion
    }
}