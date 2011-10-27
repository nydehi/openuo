/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id$:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/

namespace Client.Network
{
    public delegate void OnPacketReceive(NetState state, PacketReader reader);

    public class PacketHandler
    {
        private readonly int _packetId;
        private readonly int _length;
        private readonly OnPacketReceive _onReceive;

        public PacketHandler(int packetId, int length, OnPacketReceive onReceive)
        {
            _packetId = packetId;
            _length = length;
            _onReceive = onReceive;
        }

        public int PacketID
        {
            get
            {
                return _packetId;
            }
        }

        public int Length
        {
            get
            {
                return _length;
            }
        }

        public OnPacketReceive OnReceive
        {
            get
            {
                return _onReceive;
            }
        }
    }
}