#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using Client.Core;
using Client.Graphics;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.UI
{
    public class BackgroundElement : Element
    {
        private int _index;
        private Texture[] _textures;
        private Vector2[] _textureSizes;
        private Vector2[] _positions;
        private Vector2[] _sizes;
        private Vector2[] _texCoords;
        private bool _isDirty;
        private bool _isValid;
            
        private Vector2 _size;

        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                if (base.Position != value)
                {
                    base.Position = value;
                    _isDirty = true;
                }
            }
        }

        public virtual Vector2 Size
        {
            get { return _size; }
            set
            {
                if (_size != value)
                {
                    _size = value;
                    _isDirty = true;
                }
            }
        }

        public BackgroundElement(IUserInterfaceManager userInterface, int index)
            : base(userInterface)
        {
            _index = index;
            _isDirty = true;
            _isValid = true;
        }

        public override void Update(UpdateState state)
        {
            if (_textures == null)
            {
                _textures = new Texture[] {
                    UserInterface.TextureFactory.CreateGumpTexture(_index),
                    UserInterface.TextureFactory.CreateGumpTexture(_index + 1),
                    UserInterface.TextureFactory.CreateGumpTexture(_index + 2),
                    UserInterface.TextureFactory.CreateGumpTexture(_index + 3),
                    UserInterface.TextureFactory.CreateGumpTexture(_index + 4),
                    UserInterface.TextureFactory.CreateGumpTexture(_index + 5),
                    UserInterface.TextureFactory.CreateGumpTexture(_index + 6),
                    UserInterface.TextureFactory.CreateGumpTexture(_index + 7),
                    UserInterface.TextureFactory.CreateGumpTexture(_index + 8),
                };

                Vector2 tl;
                UserInterface.TextureFactory.GetGumpSize(_index, out tl);
                Vector2 t;
                UserInterface.TextureFactory.GetGumpSize(_index + 1, out t);
                Vector2 tr;
                UserInterface.TextureFactory.GetGumpSize(_index + 2, out tr);
                Vector2 ml;
                UserInterface.TextureFactory.GetGumpSize(_index + 3, out ml);
                Vector2 m;
                UserInterface.TextureFactory.GetGumpSize(_index + 4, out m);
                Vector2 mr;
                UserInterface.TextureFactory.GetGumpSize(_index + 5, out mr);
                Vector2 bl;
                UserInterface.TextureFactory.GetGumpSize(_index + 6, out bl);
                Vector2 b;
                UserInterface.TextureFactory.GetGumpSize(_index + 7, out b);
                Vector2 br;
                UserInterface.TextureFactory.GetGumpSize(_index + 8, out br);

                _isValid = 
                    tl != Vector2.Zero && 
                    t != Vector2.Zero && 
                    tr != Vector2.Zero && 
                    ml != Vector2.Zero && 
                    m != Vector2.Zero && 
                    mr != Vector2.Zero && 
                    bl != Vector2.Zero && 
                    b != Vector2.Zero && 
                    br != Vector2.Zero;

                _textureSizes = new Vector2[]{
                    tl, t, tr,
                    ml, m, mr,
                    bl, b, br
                };
            }

            if (_isDirty && _isValid)
            {
                _isDirty = false;

                Vector2 row1 = Position;
                Vector2 row2 = Position + new Vector2(0, _textureSizes[0].Y);
                Vector2 row3 = Position + new Vector2(0, _size.Y - _textureSizes[6].Y);
                
                Vector2 col1 = Position;
                Vector2 col2 = Position + new Vector2(_textureSizes[0].X, 0);
                Vector2 col3 = Position + new Vector2(_size.X - _textureSizes[6].X, 0);

                _positions = new Vector2[] {
                    new Vector2(col1.X, row1.Y),
                    new Vector2(col2.X, row1.Y),
                    new Vector2(col3.X, row1.Y),
                    new Vector2(col1.X, row2.Y),
                    new Vector2(col2.X, row2.Y),
                    new Vector2(col3.X, row2.Y),
                    new Vector2(col1.X, row3.Y),
                    new Vector2(col2.X, row3.Y),
                    new Vector2(col3.X, row3.Y),
                };

                float stretchX = col3.X - col2.X;
                float stretchY = row3.Y - row2.Y;

                _sizes = new Vector2[] {
                    _textureSizes[0],
                    new Vector2(stretchX, _textureSizes[1].Y),
                    _textureSizes[2],
                    new Vector2(_textureSizes[3].X, stretchY),
                    new Vector2(stretchX, stretchY), 
                    new Vector2(_textureSizes[5].X, stretchY),
                    _textureSizes[6],
                    new Vector2(stretchX, _textureSizes[7].Y),
                    _textureSizes[8]
                };

                _texCoords = new Vector2[] {
                    new Vector2(_sizes[1].X / _textureSizes[1].X, 1f),
                    new Vector2(1f, _sizes[3].Y / _textureSizes[3].Y),
                    new Vector2(_sizes[4].X / _textureSizes[4].X, _sizes[4].Y / _textureSizes[4].Y)
                };
            }
        }

        private float ensureNotZero(float p)
        {
            if(p == 0)
                return 1;

            return p;
        }

        public override void Render(DrawState state)
        {
            if (_textures == null || !_isValid)
                return;

            IRenderer renderer = state.Renderer;

            Vector2 position = _positions[0];
            Vector2 size = _sizes[0];
            Vector2 texCoords = Vector2.One;

            renderer.RenderQuad(ref position, ref size, ref texCoords, _textures[0]);

            position = _positions[1];
            size = _sizes[1];
            texCoords = _texCoords[0];

            renderer.RenderQuad(ref position, ref size, ref texCoords, _textures[1]);

            position = _positions[2];
            size = _sizes[2];
            texCoords = Vector2.One;

            renderer.RenderQuad(ref position, ref size, ref texCoords, _textures[2]);

            position = _positions[3];
            size = _sizes[3];
            texCoords = _texCoords[1];

            renderer.RenderQuad(ref position, ref size, ref texCoords, _textures[3]);

            position = _positions[4];
            size = _sizes[4];
            texCoords = _texCoords[2];

            renderer.RenderQuad(ref position, ref size, ref texCoords, _textures[4]);

            position = _positions[5];
            size = _sizes[5];
            texCoords = _texCoords[1];

            renderer.RenderQuad(ref position, ref size, ref texCoords, _textures[5]);

            position = _positions[6];
            size = _sizes[6];
            texCoords = Vector2.One;

            renderer.RenderQuad(ref position, ref size, ref texCoords, _textures[6]);

            position = _positions[7];
            size = _sizes[7];
            texCoords = _texCoords[0];

            renderer.RenderQuad(ref position, ref size, ref texCoords, _textures[7]);

            position = _positions[8];
            size = _sizes[8];
            texCoords = Vector2.One;

            renderer.RenderQuad(ref position, ref size, ref texCoords, _textures[8]);
        }
    }
}
