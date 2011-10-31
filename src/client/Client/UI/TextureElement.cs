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
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.UI
{
    public class TextureElement : Element
    {
        private int _index;
        private Texture _texture;
        private Vector2 _size;

        public virtual Vector2 Size
        {
            get { return _size; }
            set { _size = value; }
        }

        public TextureElement(IUserInterfaceManager userInterface, int index)
            : base(userInterface)
        {
            _index = index;
        }

        public override void Update(UpdateState state)
        {
            if (_texture == null)
                _texture = UserInterface.TextureFactory.CreateGumpTexture(_index);
        }

        public override void Render(Graphics.DrawState state)
        {
            if (_texture == null)
                return;

            Vector2 position = Position;
            Vector2 size = Size;

            state.Renderer.RenderQuad(ref position, ref size, _texture);
        }
    }
}
