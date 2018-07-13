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
using Microsoft.Xna.Framework;
using JuicyUO.Ultima.Network.Client;
#endregion

namespace JuicyUO.Ultima.World.Entities.Mobiles
{
    /// <summary>
    /// Mobile movement state tracking object. Receives movement packets from the server and sends client move packets to the server.
    /// TODO: This class needs a serious refactor.
    /// </summary>
    public class MobileMovement
    {
        double m_playerMobile_NextMoveInMS;

        public static Action<MoveRequestPacket> SendMoveRequestPacket;

        public static bool NewDiagonalMovement = true;

        public bool RequiresUpdate
        {
            get;
            set;
        }

        public bool IsRunning
        {
            get
            {
                return ((Facing & Direction.Running) == Direction.Running);
            }
        }

        protected Position3D CurrentPosition
        {
            get { return m_entity.Position; }
        }

        public Position3D GoalPosition
        {
            get;
            private set;
        }

        Direction m_playerMobile_NextMove = Direction.Nothing;
        Direction m_Facing = Direction.Up;
        const int m_MinimumStaminaToRun = 2;

        public Direction Facing
        {
            get { return m_Facing; }
            set { m_Facing = value; }
        }

        public bool IsMoving
        {
            get
            {
                if (GoalPosition == null)
                    return false;
                if ((CurrentPosition.Tile == GoalPosition.Tile) &&
                    !CurrentPosition.IsOffset)
                    return false;
                return true;
            }
        }

        double MoveSequence;

        internal Position3D Position { get { return CurrentPosition; } }

        MobileMoveEvents m_MoveEvents;
        AEntity m_entity;

        public MobileMovement(AEntity entity)
        {
            m_entity = entity;
            m_MoveEvents = new MobileMoveEvents();
        }

        public void PlayerMobile_MoveEventAck(int nSequence)
        {
            m_MoveEvents.AcknowledgeMoveRequest(nSequence);
        }

        public void PlayerMobile_MoveEventRej(int sequenceID, int x, int y, int z, int direction)
        {
            // immediately return to the designated tile.
            int ax, ay, az, af;
            m_MoveEvents.RejectMoveRequest(sequenceID, out ax, out ay, out az, out af);
            Move_Instant(x, y, z, direction);
            m_MoveEvents.ResetMoveSequence();
        }

        public void PlayerMobile_ChangeFacing(Direction facing)
        {
            if (!IsMoving)
            {
                PlayerMobile_SendChangeFacingMsg(facing);
                Facing = facing;
            }
        }

        /// <summary>
        /// If current Facing does not match param facing, will send a message to the server with the new facing.
        /// Does not change the actual facing value.
        /// </summary>
        /// <param name="facing"></param>
        void PlayerMobile_SendChangeFacingMsg(Direction facing)
        {
            if ((Facing & Direction.FacingMask) != (facing & Direction.FacingMask))
            {
                m_MoveEvents.AddMoveEvent(
                    CurrentPosition.X,
                    CurrentPosition.Y,
                    CurrentPosition.Z,
                    (int)(facing & Direction.FacingMask),
                    true);
            }
        }

        public void PlayerMobile_Move(Direction facing)
        {
            m_playerMobile_NextMove = facing;
        }

        public void Mobile_ServerAddMoveEvent(int x, int y, int z, int facing)
        {
            m_MoveEvents.AddMoveEvent(x, y, z, facing, false);
        }

        public void Move_Instant(int x, int y, int z, int facing)
        {
            m_MoveEvents.ResetMoveSequence();
            Facing = (Direction)facing;
            CurrentPosition.Set(x, y, z);
            GoalPosition = null;
        }

        public void Update(double frameMS)
        {
            if (!IsMoving)
            {
                if (m_entity.IsClientEntity && m_playerMobile_NextMoveInMS <= 0d)
                    PlayerMobile_CheckForMoveEvent();

                MobileMoveEvent moveEvent;
                int sequence;

                if (m_entity.IsClientEntity)
                {
                    while ((moveEvent = m_MoveEvents.GetNextMoveEvent(out sequence)) != null)
                    {
                        Facing = (Direction)moveEvent.Facing;
                        Mobile pl = (Mobile)m_entity;
                        if (pl.Stamina.Current == 0)
                            break;
                        if ((pl.Stamina.Current < m_MinimumStaminaToRun) && (pl.Stamina.Current > 0) && ((Facing & Direction.Running) == Direction.Running))
                            Facing &= Direction.FacingMask;
                
                        if (moveEvent.CreatedByPlayerInput)
                            SendMoveRequestPacket(new MoveRequestPacket((byte)Facing, (byte)sequence, moveEvent.Fastwalk));
                        Position3D p = new Position3D(moveEvent.X, moveEvent.Y, moveEvent.Z);
                        if (p != CurrentPosition)
                        {
                            GoalPosition = p;
                            break;
                        }
                    }
                }
                else
                {
                    moveEvent = m_MoveEvents.GetAndForwardToFinalMoveEvent(out sequence);
                    if (moveEvent != null)
                    {
                        Facing = (Direction)moveEvent.Facing;
                        Position3D p = new Position3D(moveEvent.X, moveEvent.Y, moveEvent.Z);
                        if (p != CurrentPosition)
                            GoalPosition = p;
                    }
                }
            }


            // Are we moving? (if our current location != our destination, then we are moving)
            if (IsMoving)
            {
                double diff = (frameMS / MovementSpeed.TimeToCompleteMove(m_entity, Facing));

                MoveSequence += diff;
                if (m_entity.IsClientEntity)
                    m_playerMobile_NextMoveInMS -= frameMS;

                if (Math.Abs(GoalPosition.X - CurrentPosition.X) > 1 || Math.Abs(GoalPosition.Y - CurrentPosition.Y) > 1)
                {
                    int x, y;
                    if (CurrentPosition.X < GoalPosition.X)
                        x = GoalPosition.X - 1;
                    else if (CurrentPosition.X > GoalPosition.X)
                        x = GoalPosition.X + 1;
                    else
                        x = GoalPosition.X;

                    if (CurrentPosition.Y < GoalPosition.Y)
                        y = GoalPosition.Y - 1;
                    else if (CurrentPosition.Y > GoalPosition.Y)
                        y = GoalPosition.Y + 1;
                    else
                        y = GoalPosition.Y;

                    CurrentPosition.Set(x, y, CurrentPosition.Z);
                }

                if (MoveSequence < 1f)
                {
                    CurrentPosition.Offset = new Vector3(
                        GoalPosition.X - CurrentPosition.X,
                        GoalPosition.Y - CurrentPosition.Y,
                        GoalPosition.Z - CurrentPosition.Z) * (float)MoveSequence;
                }
                else
                {
                    CurrentPosition.Set(GoalPosition.X, GoalPosition.Y, GoalPosition.Z);
                    CurrentPosition.Offset = Vector3.Zero;
                    MoveSequence = 0f;
                    if (m_entity.IsClientEntity)
                        m_playerMobile_NextMoveInMS = 0;
                }
            }
            else
            {
                MoveSequence = 0f;
                if (m_entity.IsClientEntity)
                {
                    m_playerMobile_NextMoveInMS = 0d;
                }
            }
        }

        bool PlayerMobile_CheckForMoveEvent()
        {
            if (m_playerMobile_NextMove != Direction.Nothing)
            {
                Direction nextMove = m_playerMobile_NextMove;
                m_playerMobile_NextMove = Direction.Nothing;

                // get the next tile and the facing necessary to reach it.
                Direction facing;
                Point targetTile = MobileMovementCheck.OffsetTile(CurrentPosition, nextMove);

                int nextZ;
                Point nextTile;
                if (PlayerMobile_GetNextTile(CurrentPosition, targetTile, out facing, out nextTile, out nextZ))
                {
                    // Check facing and send change facing message to server if necessary.
                    PlayerMobile_SendChangeFacingMsg(facing);

                    // copy the running flag to our local facing if we are running,
                    // zero it out if we are not.
                    if ((nextMove & Direction.Running) != 0)
                        facing |= Direction.Running;
                    else
                        facing &= Direction.FacingMask;

                    if ((CurrentPosition.X != nextTile.X) ||
                        (CurrentPosition.Y != nextTile.Y) ||
                        (CurrentPosition.Z != nextZ))
                    {
                        m_MoveEvents.AddMoveEvent(nextTile.X, nextTile.Y, nextZ, (int)(facing), true);
                        return true;
                    }
                }
                else
                {
                    // blocked
                    return false;
                }
            }
            return false;
        }

        bool PlayerMobile_GetNextTile(Position3D current, Point goal, out Direction facing, out Point nextPosition, out int nextZ)
        {
            bool moveIsOkay;

            // attempt to move in the direction specified.
            facing = getNextFacing(current, goal);
            Direction initialFacing = facing;
            nextPosition = MobileMovementCheck.OffsetTile(current, facing);
            moveIsOkay = MobileMovementCheck.CheckMovement((Mobile)m_entity, current, facing, out nextZ);

            // The legacy client only allows alternative direction checking when moving in a cardinal (NSEW) direction.
            // This is checked by only checked alterate directions when the initial facing modulo 2 is 1.
            // By contrast, this client allows, when enabled, alternative direction checking in any direction.
            if (NewDiagonalMovement || ((int)initialFacing % 2 == 1))
            {
                // if blocked, attempt moving in the direction 1/8 counterclockwise to the direction specified.
                if (!moveIsOkay)
                {
                    facing = (facing - 1) & Direction.ValueMask;
                    nextPosition = MobileMovementCheck.OffsetTile(current, facing);
                    moveIsOkay = MobileMovementCheck.CheckMovement((Mobile)m_entity, current, facing, out nextZ);
                }

                // if blocked, attempt moving in the direction 1/8 clockwise to the direction specified.
                if (!moveIsOkay)
                {
                    facing = (facing + 2) & Direction.ValueMask;
                    nextPosition = MobileMovementCheck.OffsetTile(current, facing);
                    moveIsOkay = MobileMovementCheck.CheckMovement((Mobile)m_entity, current, facing, out nextZ);
                }
            }

            // if we were able to move, then set the running flag (if necessary) and return true.
            if (moveIsOkay)
            {
                if (IsRunning)
                    facing |= Direction.Running;
                return true;
            }
            else
            {
                // otherwise return false, indicating that the player is blocked.
                return false;
            }
        }

        Direction getNextFacing(Position3D current, Point goal)
        {
            Direction facing;

            if (goal.X < current.X)
            {
                if (goal.Y < current.Y)
                    facing = Direction.Up;
                else if (goal.Y > current.Y)
                    facing = Direction.Left;
                else
                    facing = Direction.West;
            }
            else if (goal.X > current.X)
            {
                if (goal.Y < current.Y)
                    facing = Direction.Right;
                else if (goal.Y > current.Y)
                    facing = Direction.Down;
                else
                    facing = Direction.East;
            }
            else
            {
                if (goal.Y < current.Y)
                    facing = Direction.North;
                else if (goal.Y > current.Y)
                    facing = Direction.South;
                else
                {
                    // We should never reach 
                    facing = Facing & Direction.FacingMask;
                }
            }

            return facing;
        }
    }
}
