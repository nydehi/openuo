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

using Ninject;

namespace Client
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
