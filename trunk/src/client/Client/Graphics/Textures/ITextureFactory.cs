﻿#region License Header
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
 #endregion

using Client.Cores;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics
{
    public interface ITextureFactory : IUpdatable, IResourceContainer
    {
        void GetGumpSize(int index, out Vector2 size);
        void GetLandSize(int index, out Vector2 size);
        void GetStaticSize(int index, out Vector2 size);
        Texture CreateGumpTexture(int index);
        Texture CreateLandTexture(int index);
        Texture CreateStaticTexture(int index);
    }
}
