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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Core;
using SharpDX.Direct3D9;
using Client.Graphics;
using SharpDX;

namespace Client.UI
{
    public class TextureElement : Element
    {
        private int _index;
        private Texture _texture;

        public Vector2 Size;

        public TextureElement(IUserInterface userInterface, int index)
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

            state.Renderer.RenderQuad(ref Position, ref Size, _texture);
        }
    }
}
