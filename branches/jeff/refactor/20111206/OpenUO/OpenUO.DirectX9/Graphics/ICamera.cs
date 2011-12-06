#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: ICamera.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using SharpDX;

namespace OpenUO.DirectX9.Graphics
{
    public interface ICamera
    {
        int FarClip { get; set; }
        int NearClip { get; set; }
        Vector3 Position { get; set; }
        Vector3 Rotation { get; set; }
        BoundingFrustum BoundingFrustum { get; }
        Matrix Projection { get; }
        Matrix View { get; }
    }
}
