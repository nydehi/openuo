﻿#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: EngineBootstrapper.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using Ninject;

namespace OpenUO.Ninject
{
    public abstract class StandardKernelBootstrapper
    {
        public IKernel Kernel { get; private set; }

        public void Run()
        {
            Kernel = new StandardKernel();

            ConfigureKernel();
            InitializeModules();
        }

        protected virtual void InitializeModules()
        {

        }

        protected virtual void ConfigureKernel()
        {

        }
    }
}
