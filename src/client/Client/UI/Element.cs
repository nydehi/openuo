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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Client.Graphics;
using SharpDX.Direct3D9;
using SharpDX;
using Client.Core;
using Client.Cores;

namespace Client.UI
{
    public abstract class Element : IRenderable,IUpdatable
    {         
        protected IUserInterface UserInterface;

        public Vector2 Position;

        public Element(IUserInterface userInterface)
        {
            UserInterface = userInterface;
        }

        public abstract void Update(UpdateState state);
        public abstract void Render(DrawState state);
    }
}
