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

using SharpDX;
using SharpDX.Direct3D9;

namespace OpenUO.DirectX9.Graphics
{
    public interface IRenderer : IResourceContainer
    {
        //Vector2 MeasureString(Font font, string text);
        //void RenderString(Font font, string text, int x, int y, Color4 color);
        //void RenderLine(int x0, int y0, Color4 color0, int x1, int y1, Color4 color1);
        //void Flush();

        //void RenderQuad(ref Vector2 position, ref Vector2 size, Texture texture);
        //void RenderQuad(ref Vector2 position, ref Vector2 size, ref Vector2 texCoords, Texture texture);
        //void RenderQuad(ref Vector2 topLeft, ref Vector2 topRight, ref Vector2 bottomLeft, ref Vector2 bottomRight, Texture texture);
    }
}
