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

namespace Client.Ultima
{
    public struct LandData
    {
        private readonly string _name;
        private readonly TileFlag _flags;

        public LandData(string name, TileFlag flags)
        {
            _name = name;
            _flags = flags;
        }

        public string Name
        {
            get { return _name; }
        }

        public TileFlag Flags
        {
            get { return _flags; }
        }
    }
}
