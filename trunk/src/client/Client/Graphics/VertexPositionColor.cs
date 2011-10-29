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

using System.Runtime.InteropServices;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    [StructLayout(LayoutKind.Sequential)]
    public struct VertexPositionColor
    {
        public static readonly VertexElement[] VertexElements = new[]
        {
            new VertexElement(0, sizeof(float) * 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, sizeof(float) * 3, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
			VertexElement.VertexDeclarationEnd
        };

        public const int SizeInBytes = sizeof(float) * 7;

        public Vector3 Position;
        public Color4 Color;
    }
}
