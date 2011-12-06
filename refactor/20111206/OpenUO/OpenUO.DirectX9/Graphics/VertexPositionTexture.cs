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
    public struct VertexPositionTexture
    {
        public static readonly VertexElement[] VertexElements = new[]
        {
            new VertexElement(0, 0, DeclarationType.Float3, DeclarationMethod.Default, DeclarationUsage.Position, 0),
            new VertexElement(0, 12, DeclarationType.Float2, DeclarationMethod.Default, DeclarationUsage.TextureCoordinate, 0),
            VertexElement.VertexDeclarationEnd
        };

        public const int SizeInBytes = sizeof(float) * 5;

        public Vector3 Position;
        public Vector2 TextureCoordinate;
    }
}
