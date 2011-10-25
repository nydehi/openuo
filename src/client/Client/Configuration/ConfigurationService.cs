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

using System.IO;
using Client.Diagnostics;

namespace Client.Configuration
{
    public sealed class ConfigurationService : IConfigurationService
    {
        private readonly ConfigFile _configFile;

        public ConfigurationService()
        {
            string file = "config.xml";
            _configFile = new ConfigFile(file);

            if (!File.Exists(file))
                RestoreDefaults();
        }

        public void RestoreDefaults()
        {
            SetValue(ConfigSections.Graphics, ConfigKeys.GraphicsWidth, 1024);
            SetValue(ConfigSections.Graphics, ConfigKeys.GraphicsHeight, 768);

#if DEBUG
            SetValue(ConfigSections.Debug, ConfigKeys.DebugLogLevel, TraceLevels.Verbose);
#else
            SetValue(ConfigSections.Debug, ConfigKeys.LogLevel, TraceLevels.Warning);
#endif
        }

        public T GetValue<T>(string section, string key)
        {
            return _configFile.GetValue<T>(section, key);
        }

        public T GetValue<T>(string section, string key, T defaultValue)
        {
            return _configFile.GetValue<T>(section, key, defaultValue);
        }

        public void SetValue<T>(string section, string key, T value)
        {
            _configFile.SetValue(section, key, value);
        }
    }
}
