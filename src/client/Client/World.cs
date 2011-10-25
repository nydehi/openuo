/***************************************************************************
 *   Copyright (c) 2010 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   SVN revision information:
 *   $Author: $:
 *   $Date: $:
 *   $Revision: $:
 *   $Id: $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/

using System.Collections.Generic;

namespace Client
{
    public sealed class World
    {
        private Engine _engine;

        private Dictionary<Serial, Mobile> _mobiles;
        private Dictionary<Serial, Item> _items;

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
