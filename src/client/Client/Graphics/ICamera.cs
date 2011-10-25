/***************************************************************************
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

using SharpDX;

namespace Client.Graphics
{
    public interface ICamera
    {
        int FarClip { get; set; }
        int NearClip { get; set; }
        Vector2 Position { get; set; }
        Vector2 HalfVector { get; }
        Matrix Projection { get; }
        //Matrix View { get; }
    }
}
