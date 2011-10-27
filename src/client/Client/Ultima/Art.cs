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
using SharpDX.Direct3D9;
using SharpDX;
using Client.Core.Graphics;
using Client.Core;

namespace Client.Ultima
{
    public class Art
    {
        private readonly FileIndex _fileIndex;

        public Art(Engine engine)
        {
            _fileIndex = new FileIndex(engine, "artidx.mul", "art.mul", 0x10000, 4);
        }

        public unsafe Texture CreateTexture(Engine engine, int index)
        {
            index &= 0x3FFF;
            index += 0x4000;

            int length, extra;
            bool patched;

            Stream stream = _fileIndex.Seek(index, out length, out extra, out patched);

            if (stream == null)
                return null;

            BinaryReader bin = new BinaryReader(stream);

            bin.ReadInt32(); // Unknown
            int width = bin.ReadInt16();
            int height = bin.ReadInt16();

            if (width <= 0 || height <= 0)
                return null;

            int[] lookups = new int[height];

            int start = (int)bin.BaseStream.Position + (height * 2);

            for (int i = 0; i < height; ++i)
                lookups[i] = (int)(start + (bin.ReadUInt16() * 2));

            Texture texture = new Texture(engine.Device, width, height, 0, Usage.None, Format.A1R5G5B5, Pool.Managed);
            DataRectangle rect = texture.LockRectangle(0, LockFlags.None);

            ushort* line = (ushort*)rect.DataPointer;
            int delta = rect.Pitch >> 1;

            for (int y = 0; y < height; ++y, line += delta)
            {
                bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                ushort* cur = line;
                ushort* end;

                int xOffset, xRun;

                while (((xOffset = bin.ReadUInt16()) + (xRun = bin.ReadUInt16())) != 0)
                {
                    cur += xOffset;
                    end = cur + xRun;

                    while (cur < end)
                        *cur++ = (ushort)(bin.ReadUInt16() ^ 0x8000);
                }
            }

            texture.UnlockRectangle(0);

            return texture;
        }
    }
}
