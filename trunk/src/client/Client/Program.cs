#region License Header
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

using System;
using Client.Core;

namespace Client
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            EngineBootstrapper boostrapper = new EngineBootstrapper();
            boostrapper.Run();

            using (Engine engine = new Engine(boostrapper.Kernel))
            {
                engine.Run();
            }
        }
    }
}
