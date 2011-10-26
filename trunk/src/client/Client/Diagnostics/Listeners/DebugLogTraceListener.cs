﻿/***************************************************************************
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
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Client.IO;

namespace Client.Diagnostics
{
    public sealed class DebugLogTraceListener : TraceListener
    {
        private static readonly Dictionary<string, object> _lockTable = new Dictionary<string, object>();
        private readonly string _filename;

        public DebugLogTraceListener(string filename)
        {
            _filename = filename;

            object syncRoot;

            if (!_lockTable.TryGetValue(filename, out syncRoot))
            {
                syncRoot = new object();
                _lockTable.Add(filename, syncRoot);

                if (File.Exists(_filename))
                    File.Delete(_filename);

                OnTraceReceived(new TraceMessage(TraceLevels.Verbose, DateTime.UtcNow, "Logging Started",
                    string.IsNullOrEmpty(Thread.CurrentThread.Name) ? Thread.CurrentThread.ManagedThreadId.ToString() : Thread.CurrentThread.Name));
            }
        }

        protected override void OnTraceReceived(TraceMessage message)
        {
            try
            {
                object syncRoot = _lockTable[_filename];

                lock (syncRoot)
                {
                    string directory = Path.GetDirectoryName(_filename);

                    FileSystemHelper.EnsureDirectoryExists(directory);

                    using (StreamWriter writer = new StreamWriter(_filename))
                    {
                        writer.WriteLine(message);
                        writer.Flush();
                    }
                }
            }
            catch (Exception e)
            {
                Dispose(true);
                Tracer.Error(e);
            }
        }
    }
}
