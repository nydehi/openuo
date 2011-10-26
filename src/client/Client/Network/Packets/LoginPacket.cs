/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   SVN revision information:
 *   $Author$:
 *   $Date$:
 *   $Revision$:
 *   $Id$:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/


namespace Client.Network.Packets
{
    public class LoginPacket : Packet
    {
        public LoginPacket(string username, string password)
            : base(0x80, 62)
        {
            Stream.WriteAsciiFixed(username, 30);
            Stream.WriteAsciiFixed(password, 30);
            Stream.Write((byte)0x5D);
        }
    }
}
