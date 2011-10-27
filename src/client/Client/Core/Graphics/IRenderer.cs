﻿/***************************************************************************
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
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Core.Graphics
{
    public interface IRenderer : IResourceContainer
    {
        Vector2 MeasureString(Font font, string text);
        void RenderString(Font font, string text, int x, int y, Color4 color);
        void RenderLine(int x0, int y0, Color4 color0, int x1, int y1, Color4 color1);
        void Flush();

        void RenderQuad(ref Vector2 northVector, ref Vector2 eastVector, ref Vector2 westVector, ref Vector2 southVector, Texture texture);
    }
}