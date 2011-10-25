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

using System;

namespace Client.Diagnostics
{
    public sealed class TraceMessageEventArgs : EventArgs
    {
        public TraceMessage TraceMessage { get; private set; }

        public TraceMessageEventArgs(TraceMessage traceMessage)
        {
            Asserter.AssertIsNotNull(traceMessage, "traceMessage");

            TraceMessage = traceMessage;
        }
    }
}
