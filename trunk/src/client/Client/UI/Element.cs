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

using Client.Core;
using Client.Cores;
using Client.Graphics;
using SharpDX;

namespace Client.UI
{
    public abstract class Element : IRenderable,IUpdatable
    {         
        private Vector2 _position;

        public virtual Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        protected IUserInterfaceManager UserInterface;

        public Element(IUserInterfaceManager userInterface)
        {
            UserInterface = userInterface;
        }

        public abstract void Update(UpdateState state);
        public abstract void Render(DrawState state);
    }
}
