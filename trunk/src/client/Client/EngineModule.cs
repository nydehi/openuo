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

using Client.Modules;
using Client.Diagnostics;
using Client.Configuration;

namespace Client
{
    public class EngineModule : ModuleBase
    {
        protected override void Initialize()
        {
            new DebugTraceListener { TraceLevel = TraceLevels.Verbose };

            Kernel.Bind<IConfigurationService>().To<ConfigurationService>();
        }
    }
}