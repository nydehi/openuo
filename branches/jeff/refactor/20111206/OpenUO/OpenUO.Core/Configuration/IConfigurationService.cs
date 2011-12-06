#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: IConfigurationService.cs 14 2011-10-31 07:03:12Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

namespace OpenUO.Core.Configuration
{
    public interface IConfigurationService
    {
        T GetValue<T>(string section, string key);
        T GetValue<T>(string section, string key, T defaultValue);

        void SetValue<T>(string section, string key, T value);

        void Reload();
    }
}
