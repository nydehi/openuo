﻿/***************************************************************************
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

using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColorTexture
    {
        public static readonly VertexElement[] VertexElements = new[]
        {
            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
            new VertexElement(0, 24, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
			VertexElement.VertexDeclarationEnd
        };

        public const int SizeInBytes = sizeof(float) * 9;

        public Vector3 Position;
        public Color4 Color;
        public Vector2 TextureCoordinate;
    }
}
