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

using System;
using Client.Collections;
using Client.Properties;
using Client.Ultima;
using SharpDX.Direct3D9;
using SharpDX;
using SharpDX.Direct3D;
using Ninject;
using Client.Cores;
using Client.Core;

namespace Client.Graphics
{
    public enum TextureType
    {
        Land,
        Static
    }

    public class TextureFactory : ITextureFactory 
    {
        private readonly TimeSpan _cleanInterval = TimeSpan.FromMinutes(1);
        private readonly Cache<int, Texture> _landCache;
        private readonly Cache<int, Texture> _staticCache;
        private readonly Cache<int, Texture> _gumpCache;
        private readonly Textures _textures;
        private readonly Art _art;
        private readonly Gumps _gumps;
        private readonly Hues _hues;
        private readonly Engine _engine;

        private Texture _missingTexture;
        private DateTime _lastCacheClean;

        public Cache<int, Texture> LandCache
        {
            get { return _landCache; }
        }

        public TextureFactory(Engine engine)
        {
            _engine = engine;
            _gumpCache = new Cache<int, Texture>(TimeSpan.FromMinutes(5), 0x1000);
            _landCache = new Cache<int, Texture>(TimeSpan.FromMinutes(5), 0x1000);
            _staticCache = new Cache<int, Texture>(TimeSpan.FromMinutes(5), 0x1000);
            _lastCacheClean = DateTime.MinValue;
            _textures = new Textures(engine);
            _hues = new Hues(engine);
            _gumps = new Gumps(engine);
            _art = new Art(engine);
        }

        public Texture CreateHue(int index)
        {
            return _hues[index];
        }

        public void GetGumpSize(int index, out Vector2 size)
        {
            Texture texture = _gumpCache[index];

            if (texture == null)
            {
                _gumps.Measure(index, out size);
                return;
            }

            SurfaceDescription description = texture.GetLevelDescription(0);

            size.X = description.Width;
            size.Y = description.Height;
        }

        public void GetLandSize(int index, out Vector2 size)
        {
            Texture texture = _landCache[index];

            if (texture == null)
            {
                _textures.Measure(index, out size);
                return;
            }

            SurfaceDescription description = texture.GetLevelDescription(0);

            size.X = description.Width;
            size.Y = description.Height;
        }

        public void GetStaticSize(int index, out Vector2 size)
        {
            Texture texture = _staticCache[index];

            if (texture == null)
            {
                _art.Measure(index, out size);
                return;
            }

            SurfaceDescription description = texture.GetLevelDescription(0);

            size.X = description.Width;
            size.Y = description.Height;
        }

        public Texture CreateLandTexture(int index)
        {
            Texture texture = _landCache[index];

            if (texture != null)
                return texture;

            texture = _textures.CreateTexture(index);

            if (texture == null)
                return _missingTexture;

            return _landCache[index] = texture;
        }

        public Texture CreateStaticTexture(int index)
        {
            Texture texture = _staticCache[index];

            if (texture != null)
                return texture;

            texture = _art.CreateTexture(index);

            if (texture == null)
                return _missingTexture;

            return _staticCache[index] = texture;
        }

        public Texture CreateGumpTexture(int index)
        {
            Texture texture = _gumpCache[index];

            if (texture != null)
                return texture;

            texture = _gumps.CreateTexture(index);

            if (texture == null)
                return _missingTexture;

            return _gumpCache[index] = texture;
        }

        public void Update(UpdateState state)
        {
            DateTime now = DateTime.Now;

            if (_lastCacheClean + _cleanInterval >= now)
                return;

            _landCache.Clean();
            _lastCacheClean = now;
        }

        public void CreateResources()
        {
            _missingTexture = Texture.FromMemory(_engine.Device, Resources.MissingTexture);
        }

        public void Dispose()
        {
            _landCache.Dispose();
            _gumpCache.Dispose();
            _staticCache.Dispose();
            _missingTexture.Dispose();
        }
        
        public void OnDeviceLost()
        {

        }

        public void OnDeviceReset()
        {

        }
    }
}
