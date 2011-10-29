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

using Client.Properties;
using SharpDX;
using Client.Core;
using SharpDX.Direct3D9;

namespace Client.Graphics.Shaders
{
    public class DiffuseShader : ShaderBase
    {
        public Matrix WorldViewProjection
        {
            get;
            set;
        }

        public Vector2 HalfVector
        {
            get;
            set;
        }

        public DiffuseShader(Device device)
            : base(device, Resources.DiffuseEffect)
        {

        }

        protected override void BeginOverride(DrawState state)
        {
            SetValue("WorldViewProjection", state.WorldViewProjection);
            SetValue("HalfVector", state.Camera.HalfVector);
        }
    }
}
