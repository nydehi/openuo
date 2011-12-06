#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: ModuleBase.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using Ninject;
using Ninject.Modules;

namespace OpenUO.Ninject
{
    public abstract class ModuleBase : INinjectModule
    {
        private IKernel _kernel;

        public abstract string Name { get; }

        public void OnLoad(IKernel kernel)
        {
            _kernel = kernel;
            Initialize();
        }

        protected virtual void Initialize()
        {

        }

        public virtual void OnUnload(IKernel kernel)
        {

        }

        public IKernel Kernel
        {
            get { return _kernel; }
        }
    }
}
