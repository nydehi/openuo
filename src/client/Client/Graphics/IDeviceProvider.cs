﻿#region License Header
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

using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace Client.Graphics
{
    public interface IDeviceProvider
    {
        Device Device { get; }
        RenderForm RenderForm { get; }
        PresentParameters PresentParameters { get; }
    }
}
