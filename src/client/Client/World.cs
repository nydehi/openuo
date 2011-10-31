#region License Header
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
 #endregion

using System.Collections.Generic;
using Client.Core;
using Client.Ultima;

namespace Client
{
    public sealed class World
    {
        private Engine _engine;
        private Maps _maps;

        private int _currentMap;
        private Mobile _player;

        private Dictionary<Serial, Mobile> _mobiles;
        private Dictionary<Serial, Item> _items;
        
        public Mobile Player
        {
            get { return _player; }
        }

        public Dictionary<Serial, Mobile> Mobiles
        {
            get { return _mobiles; }
        }

        public Dictionary<Serial, Item> Items
        {
            get { return _items; }
        }

        public World(Engine engine)
        {
            _engine = engine;

            _mobiles = new Dictionary<Serial, Mobile>();
            _items = new Dictionary<Serial, Item>();
        }
    }
}
