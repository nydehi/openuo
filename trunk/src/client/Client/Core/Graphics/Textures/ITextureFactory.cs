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

using SharpDX.Direct3D9;
using Client.Cores;

namespace Client.Core.Graphics
{
    public interface ITextureFactory : IUpdate, IResourceContainer
    {
        Texture CreateLand(Engine engine, int index);
        Texture CreateStatic(Engine engine, int index);
    }
}
