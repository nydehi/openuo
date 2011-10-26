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

using System;
using Client.Collections;
using Client.Properties;
using Client.Ultima;
using SharpDX.Direct3D9;
using SharpDX;
using SharpDX.Direct3D;

namespace Client.Graphics
{
    public enum TextureType
    {
        Land,
        Static
    }

    public class TextureFactory : ITexturFactory, IUpdate
    {
        private readonly TimeSpan _cleanInterval = TimeSpan.FromMinutes(1);
        private readonly Texture _missingTexture;
        private readonly Cache<int, Texture> _landCache;
        private readonly Cache<int, Texture> _staticCache;
        private readonly Textures _textures;
        private readonly Art _art;
        private readonly Hues _hues;
        private DateTime _lastCacheClean;

        public Cache<int, Texture> LandCache
        {
            get { return _landCache; }
        }

        public TextureFactory(Engine engine)
        {
            _missingTexture = Texture.FromMemory(engine.Device, Resources.MissingTexture);
            _landCache = new Cache<int, Texture>(TimeSpan.FromMinutes(5), 0x1000);
            _staticCache = new Cache<int, Texture>(TimeSpan.FromMinutes(5), 0x10000);
            _lastCacheClean = DateTime.MinValue;
            _textures = new Textures(engine);
            _hues = new Hues(engine);
            _art = new Art(engine);
        }

        public Texture CreateHue(int index)
        {
            return _hues[index];
        }

        public Texture CreateLand(Engine engine, int index)
        {
            Texture texture = _landCache[index];

            if (texture != null)
                return texture;

            texture = _textures.CreateTexture(engine, index);

            if (texture == null)
                return _missingTexture;

            return _landCache[index] = texture;
        }

        public Texture CreateStatic(Engine engine, int index)
        {
            Texture texture = _staticCache[index];

            if (texture != null)
                return texture;

            texture = _art.CreateTexture(engine, index);

            if (texture == null)
                return _missingTexture;

            return _staticCache[index] = texture;
        }

        public void Update(UpdateState state)
        {
            DateTime now = DateTime.Now;

            if (_lastCacheClean + _cleanInterval >= now)
                return;

            _landCache.Clean();
            _lastCacheClean = now;
        }
    }
}
