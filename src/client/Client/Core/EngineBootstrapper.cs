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

using Ninject;

namespace Client.Core
{
    public sealed class EngineBootstrapper
    {
        public IKernel Kernel { get; private set; }

        public void Run()
        {
            Kernel = new StandardKernel();

            ConfigureKernel();
            InitializeModules();
        }

        private void InitializeModules()
        {
            Kernel.Load(new EngineModule());
        }

        private void ConfigureKernel()
        {

        }
    }
}
