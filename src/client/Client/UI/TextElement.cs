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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D9;
using SharpDX;

namespace Client.UI
{
    public class TextElement : Element
    {
        private string _text;
        private int _font;
        private bool _isDirty;
        private Texture _texture;

        public string Text
        {
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _isDirty = true;
                    _text = value;
                } 
            }
        }

        public int Font
        {
            get { return _font; }
            set
            {
                if (_font != value)
                {
                    _isDirty = true;
                    _font = value;
                }  
            }
        }

        public TextElement(IUserInterfaceManager userInterface, int font, string text)
            : base(userInterface)
        {
            _font = font;
            _text = text;
            _isDirty = true;
        }

        public override void Update(Core.UpdateState state)
        {
            if (_isDirty)
            {
                _isDirty = false;
                _texture = UserInterface.TextureFactory.CreateUnicodeTexture(_font, _text);
            }
        }

        public override void Render(Graphics.DrawState state)
        {
            if (_texture == null)
                return;
            
            Vector2 position = Position;
            Vector2 size;

            var desc = _texture.GetLevelDescription(0);

            size.X = desc.Width;
            size.Y = desc.Height;

            state.Renderer.RenderQuad(ref position, ref size, _texture);
        }
    }
}
