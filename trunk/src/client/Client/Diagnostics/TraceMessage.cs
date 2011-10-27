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

namespace Client.Diagnostics
{
    public sealed class TraceMessage
    {
        const string MessageString = "{0} {1} [{2}] {3}";

        public TraceLevels Type
        {
            get;
            private set;
        }

        public DateTime DateTime
        {
            get;
            private set;
        }

        public string Message
        {
            get;
            private set;
        }

        public string ThreadId
        {
            get;
            private set;
        }

        public TraceMessage(TraceLevels type, DateTime dateTime, string message, string threadId)
        {
            Type = type;
            DateTime = dateTime;
            Message = message;
            ThreadId = threadId;
        }

        public override string ToString()
        {
            return string.Format(MessageString, DateTime, Type.ToString().ToUpper(), ThreadId, Message);
        }
    }
}
