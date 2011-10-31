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

namespace Client.Configuration
{
    public static class ConfigKeys
    {
        //Section : Graphics
        public const string GraphicsWidth = "width";
        public const string GraphicsHeight = "height";

        //Section : Debug
        public const string DebugLogLevel = "loglevel";

        //Section : Ultima Online
        public const string UltimaOnlineDirectory = "installdirectory";
        public const string TexidxMulPath = "texidx.mul";
        public const string TexmapsMulPath = "texmaps.mul";
    }
}
