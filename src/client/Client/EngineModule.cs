/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   SVN revision information:
 *   $Author$:
 *   $Date$:
 *   $Revision$:
 *   $Id$:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/


using Client.Configuration;
using Client.Diagnostics;
using Client.Modules;

namespace Client
{
    public class EngineModule : ModuleBase
    {
        protected override void Initialize()
        {
            new DebugTraceListener { TraceLevel = TraceLevels.Verbose };
            new DebugLogTraceListener("debug.log");

            Kernel.Bind<IConfigurationService>().To<ConfigurationService>().InSingletonScope();
        }
    }
}