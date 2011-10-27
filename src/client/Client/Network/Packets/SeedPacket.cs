﻿/***************************************************************************
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

namespace Client.Network.Packets
{
    public class SeedPacket : Packet
    {
        public SeedPacket() :
            base(0x01, 4)
        {
            Stream.Write(new byte[] { 0x02, 0x03, 0x04 }, 0, 3);
        }
    }
}
