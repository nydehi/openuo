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
using Client.Graphics;
using Client.Core;

namespace Client.Ultima
{
    public class Gumps
    {
        private readonly FileIndex _fileIndex;
        private readonly Device _device;

        public Gumps(Engine engine)
        {
            _device = engine.Device;
            _fileIndex = new FileIndex(engine, "Gumpidx.mul", "Gumpart.mul", 0x10000, 12);
        }

        public unsafe Texture CreateTexture(int index)
        {
            if (!_fileIndex.FilesExist)
                return null;

            int length, extra;
            bool patched;

            Stream stream = _fileIndex.Seek(index, out length, out extra, out patched);

            if (stream == null)
                return null;

            int width = (extra >> 16) & 0xFFFF;
            int height = extra & 0xFFFF;

            if (width == 0 || height == 0)
                return null;

            Texture texture = new Texture(_device, width, height, 0, Usage.None, Format.A1R5G5B5, Pool.Managed);
            DataRectangle rect = texture.LockRectangle(0, LockFlags.None);
            BinaryReader bin = new BinaryReader(stream);

            int[] lookups = new int[height];
            int start = (int)bin.BaseStream.Position;

            for (int i = 0; i < height; ++i)
                lookups[i] = start + (bin.ReadInt32() * 4);

            ushort* line = (ushort*)rect.DataPointer;
            int delta = rect.Pitch >> 1;

            for (int y = 0; y < height; ++y, line += delta)
            {
                bin.BaseStream.Seek(lookups[y], SeekOrigin.Begin);

                ushort* cur = line;
                ushort* end = line + width;

                while (cur < end)
                {
                    ushort color = bin.ReadUInt16();
                    ushort* next = cur + bin.ReadUInt16();

                    if (color == 0)
                    {
                        cur = next;
                    }
                    else
                    {
                        color ^= 0x8000;

                        while (cur < next)
                            *cur++ = color;
                    }
                }
            }

            texture.UnlockRectangle(0); 

            return texture;
        }

        public void Measure(int index, out Vector2 size)
        {
            int length, extra;
            bool patched;

            Stream stream = _fileIndex.Seek(index, out length, out extra, out patched);

            if (stream == null)
            {
                size = Vector2.Zero;
                return;
            }

            size.X = (extra >> 16) & 0xFFFF;
            size.Y = extra & 0xFFFF;
        }
    }
}
