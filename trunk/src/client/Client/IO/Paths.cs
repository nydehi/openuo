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


using System;
using System.IO;
namespace Client.IO
{
    public static class Paths
    {
        public static string StorageDirectory
        {
            get
            {
                return Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    "OpenUO");
            }
        }

        public static string LogsDirectory
        {
            get { return Path.Combine(StorageDirectory, "logs"); }
        }

        public static string ConfigFile
        {
            get { return Path.Combine(StorageDirectory, "config.xml"); }
        }
    }
}
