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

using System;

namespace Client.Ultima
{
    [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential, Pack = 1)]
    public struct Tile : IComparable
    {
        internal short _id;
        internal sbyte _z;

        public int ID
        {
            get { return _id; }
        }

        public int Z
        {
            get { return _z; }
            set { _z = (sbyte)value; }
        }

        public bool Ignored
        {
            get { return (_id == 2 || _id == 0x1DB || (_id >= 0x1AE && _id <= 0x1B5)); }
        }

        public Tile(short id, sbyte z)
        {
            _id = id;
            _z = z;
        }

        public void Set(short id, sbyte z)
        {
            _id = id;
            _z = z;
        }

        public int CompareTo(object x)
        {
            if (x == null)
                return 1;

            if (!(x is Tile))
                throw new ArgumentNullException();

            Tile a = (Tile)x;

            if (_z > a._z)
                return 1;

            if (a._z > _z)
                return -1;

            ItemData ourData = TileData.Instance.ItemTable[_id & 0x3FFF];
            ItemData theirData = TileData.Instance.ItemTable[a._id & 0x3FFF];

            if (ourData.Height > theirData.Height)
                return 1;

            if (theirData.Height > ourData.Height)
                return -1;

            if (ourData.Background && !theirData.Background)
                return -1;

            if (theirData.Background && !ourData.Background)
                return 1;

            return 0;
        }
    }
}
