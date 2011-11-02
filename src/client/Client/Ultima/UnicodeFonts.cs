#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D9;
using Ninject;
using Client.Configuration;
using System.IO;
using Client.Diagnostics;
using Client.Graphics;
using SharpDX;
using Client.Core;

namespace Client.Ultima
{
    public class UnicodeFonts
    {
        private static string[] _files = new string[]
        {
            "unifont.mul",
            "unifont1.mul",
            "unifont2.mul",
            "unifont3.mul",
            "unifont4.mul",
            "unifont5.mul",
            "unifont6.mul",
            "unifont7.mul",
            "unifont8.mul",
            "unifont9.mul",
            "unifont10.mul",
            "unifont11.mul",
            "unifont12.mul"
        };

        private Device _device;
        private UnicodeFont[] _fonts = new UnicodeFont[_files.Length];

        public UnicodeFonts(Engine engine)
        {
            IConfigurationService configurationService = engine.Kernel.Get<IConfigurationService>();
            IDeviceProvider provider = engine.Kernel.Get<IDeviceProvider>();

            _device = provider.Device;

            string ultimaOnlineDirectory = configurationService.GetValue<string>(ConfigSections.UltimaOnline, ConfigKeys.UltimaOnlineDirectory);

            if (!Directory.Exists(ultimaOnlineDirectory))
            {
                Tracer.Warn("UnicodeFonts: Ultima Online directory not found!");
                return;
            }

            for (int i = 0; i < _files.Length; i++)
            {
                string filePath = Path.Combine(ultimaOnlineDirectory, _files[i]);

                if (!File.Exists(filePath))
                {
                    Tracer.Warn("UnicodeFonts: Unable to find file {0}", filePath);
                    continue;
                }

                _fonts[i] = new UnicodeFont();

                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    int length = (int)fs.Length;
                    byte[] buffer = new byte[length];

                    int read = fs.Read(buffer, 0, buffer.Length);

                    using (MemoryStream stream = new MemoryStream(buffer))
                    {
                        using (BinaryReader bin = new BinaryReader(stream))
                        {                            
                            for (int c = 0; c < 0x10000; ++c)
                            {
                                _fonts[i].Chars[c] = new UnicodeChar();
                                stream.Seek((long)((c) * 4), SeekOrigin.Begin);

                                int index = bin.ReadInt32();

                                if ((index >= fs.Length) || (index <= 0))
                                    continue;

                                stream.Seek((long)index, SeekOrigin.Begin);

                                sbyte xOffset = bin.ReadSByte();
                                sbyte yOffset = bin.ReadSByte();

                                int Width = bin.ReadByte();
                                int Height = bin.ReadByte();

                                _fonts[i].Chars[c].XOffset = xOffset;
                                _fonts[i].Chars[c].YOffset = yOffset;
                                _fonts[i].Chars[c].Width = Width;
                                _fonts[i].Chars[c].Height = Height;

                                if (!((Width == 0) || (Height == 0)))
                                    _fonts[i].Chars[c].Bytes = bin.ReadBytes(Height * (((Width - 1) / 8) + 1));
                            }
                        }
                    }
                }
            }
        }

        public unsafe Texture GetTexture(int fontId, string text)
        {
            UnicodeFont font = _fonts[fontId];

            if (font == null)
                return null;

            int width = font.GetWidth(text) + 2;
            int height = font.GetHeight(text) + 2;

            Texture result = new Texture(_device, font.GetWidth(text) + 2, font.GetHeight(text) + 2, 0, Usage.None, Format.A1R5G5B5, Pool.Managed);
            DataRectangle rect = result.LockRectangle(0, LockFlags.None);

            ushort* line = (ushort*)rect.DataPointer;
            int delta = rect.Pitch >> 1;

            int dx = 2;
            int dy = 2;

            int index = 0;

            for (int i = 0; i < text.Length; ++i)
            {
                int c = (int)text[i] % 0x10000;
                UnicodeChar ch = font.Chars[c];

                int charWidth = ch.Width;
                int charHeight = ch.Height;

                byte[] data = ch.Bytes;

                if (c == 32)
                {
                    dx += 5;
                    continue;
                }
                else
                {
                    dx += ch.XOffset;
                }

                ushort linesSkipped = (ushort)(height - (charHeight + ch.YOffset));

                for (int y = 0; y < charHeight; ++y)
                {
                    ushort* cur = line + ((y + ch.YOffset) * delta) + dx;

                    for (int x = 0; x < charWidth; ++x)
                    {
                        int offset = x / 8 + y * ((charWidth + 7) / 8);

                        if (offset > data.Length)
                            continue;

                        if ((data[offset] & (1 << (7 - (x % 8)))) != 0)
                            *cur++ = 0x8000;
                        else
                            *cur++ = 0x0000;
                    }
                }

                dx += ch.Width;
            }

            return result;
        }
    }
}
