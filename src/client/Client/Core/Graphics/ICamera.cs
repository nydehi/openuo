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

using SharpDX;

namespace Client.Core.Graphics
{
    public interface ICamera
    {
        int FarClip { get; set; }
        int NearClip { get; set; }
        Vector2 Position { get; set; }
        Vector2 HalfVector { get; }
        Matrix Projection { get; }
        BoundingFrustum BoundingFrustum { get; }
        //Matrix View { get; }
    }
}
