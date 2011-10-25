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
using Ninject.Modules;

namespace Client.Modules
{
    public abstract class ModuleBase : INinjectModule
    {
        private IKernel _kernel;

        public string Name
        {
            get { return "Engine Module"; }
        }

        public void OnLoad(IKernel kernel)
        {
            _kernel = kernel;
            Initialize();
        }

        protected virtual void Initialize()
        {

        }

        public void OnUnload(IKernel kernel)
        {

        }

        public IKernel Kernel
        {
            get { return _kernel; }
        }
    }
}
