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

using Client.Configuration;
using Client.Diagnostics;
using Client.Graphics;
using Client.Input;
using Client.Modules;
using Client.UI;

namespace Client.Core
{
    public class EngineModule : ModuleBase
    {
        protected override void Initialize()
        {
            new DebugTraceListener { TraceLevel = TraceLevels.Verbose };
            new DebugLogTraceListener("debug.log");

            Kernel.Bind<IConfigurationService>().To<ConfigurationService>().InSingletonScope();
            Kernel.Bind<IInputService>().To<InputService>().InSingletonScope();
            Kernel.Bind<IDeviceProvider>().To<DeviceProvider>().InSingletonScope();
            Kernel.Bind<IRenderer>().To<Renderer>().InSingletonScope();
            Kernel.Bind<IUserInterfaceRenderer>().To<UserInterfaceRenderer>().InSingletonScope();
            Kernel.Bind<ITextureFactory>().To<TextureFactory>().InSingletonScope();
            Kernel.Bind<IUserInterfaceManager>().To<UserInterfaceManager>().InSingletonScope();
        }
    }
}