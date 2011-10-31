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

using Client.Core;
namespace Client.Ultima
{
    public class Maps
    {
        public readonly Map Felucca;
        public readonly Map Trammel;
        public readonly Map Ilshenar;
        public readonly Map Malas;
        public readonly Map Tokuno;

        public Maps(Engine engine)
        {
            Felucca = new Map(engine, 0, 0, 6144, 4096);
            Trammel = new Map(engine, 0, 1, 6144, 4096);
            Ilshenar = new Map(engine, 2, 2, 2304, 1600);
            Malas = new Map(engine, 3, 3, 2560, 2048);
            Tokuno = new Map(engine, 4, 4, 1448, 1448);
        }
    }

    public class Map
    {
        private readonly Engine _engine;

        private TileMatrix _tiles;
        private readonly int _fileIndex, _mapID;
        private readonly int _width, _height;

        internal Map(Engine engine, int fileIndex, int mapID, int width, int height)
        {
            _engine = engine;
            _fileIndex = fileIndex;
            _mapID = mapID;
            _width = width;
            _height = height;
        }

        public bool LoadedMatrix
        {
            get { return (_tiles != null); }
        }

        public TileMatrix Tiles
        {
            get { return _tiles ?? (_tiles = new TileMatrix(_engine, _fileIndex, _mapID, _width, _height)); }
        }

        public int Width
        {
            get { return _width; }
        }

        public int Height
        {
            get { return _height; }
        }
    }
}
