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

using System;
using System.IO;
using Client.Configuration;
using Ninject;
using SharpDX.Direct3D9;
using Client.Core.Graphics;
using Client.Core;

namespace Client.Ultima
{
    public class Hues
    {
        private bool _filesExist;
        private Texture[] _hues;

        public Hues(Engine engine)
        {
            IConfigurationService configurationService = engine.Kernel.Get<IConfigurationService>();

            string ultimaOnlineDirectory = configurationService.GetValue<string>(ConfigSections.UltimaOnline, ConfigKeys.UltimaOnlineDirectory);

            if (!Directory.Exists(ultimaOnlineDirectory))
            {
                _filesExist = false;
                return;
            }

            string path = Path.Combine(ultimaOnlineDirectory, "hues.mul");

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                BinaryReader bin = new BinaryReader(fs);

                int blockCount = (int)fs.Length / 708;

                if (blockCount > 375)
                    blockCount = 375;

                _hues = new Texture[blockCount];

                for (int i = 0; i < blockCount; ++i)
                {
                    bin.ReadInt32();

                    Texture texture = new Texture(engine.Device, 34, 1, 0, Usage.None, Format.A1R5G5B5, Pool.Managed); ;
                    IntPtr dataPtr = texture.LockRectangle(0, LockFlags.None).DataPointer;

                    unsafe
                    {
                        ushort* line = (ushort*)dataPtr;

                        for (int j = 0; j < 34; j++)
                            (*line++) = (ushort)(bin.ReadUInt16() | 0x8000);

                        texture.UnlockRectangle(0);
                        bin.ReadBytes(20); //Don't need to know the names

                        _hues[i] = texture;
                    }
                }
            }
        }

        public Texture this[int index]
        {
            get
            {
                if (index < _hues.Length)
                    return _hues[index];

                return null;
            }
        }
    }
}
