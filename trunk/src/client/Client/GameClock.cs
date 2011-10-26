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
using System.Diagnostics;

namespace Client
{
    internal class GameClock
    {
        private long _baseRealTime;
        private long _lastRealTime;
        private bool _lastRealTimeValid;
        private int _suspendCount;
        private long _suspendStartTime;
        private long _timeLostToSuspension;
        private TimeSpan _currentTimeOffset;
        private TimeSpan _currentTimeBase;
        private TimeSpan _elapsedTime;
        private TimeSpan _elapsedAdjustedTime;

        internal TimeSpan CurrentTime
        {
            get
            {
                return _currentTimeBase + _currentTimeOffset;
            }
        }

        internal TimeSpan ElapsedTime
        {
            get
            {
                return _elapsedTime;
            }
        }

        internal TimeSpan ElapsedAdjustedTime
        {
            get
            {
                return _elapsedAdjustedTime;
            }
        }

        internal static long Counter
        {
            get
            {
                return Stopwatch.GetTimestamp();
            }
        }

        internal static long Frequency
        {
            get
            {
                return Stopwatch.Frequency;
            }
        }

        public GameClock()
        {
            Reset();
        }

        internal void Reset()
        {
            _currentTimeBase = TimeSpan.Zero;
            _currentTimeOffset = TimeSpan.Zero;
            _baseRealTime = GameClock.Counter;
            _lastRealTimeValid = false;
        }

        internal void Step()
        {
            long counter = GameClock.Counter;

            if (!_lastRealTimeValid)
            {
                _lastRealTime = counter;
                _lastRealTimeValid = true;
            }

            try
            {
                _currentTimeOffset = GameClock.CounterToTimeSpan(counter - _baseRealTime);
            }
            catch (OverflowException e)
            {
                _currentTimeBase += _currentTimeOffset;
                _baseRealTime = _lastRealTime;

                try
                {
                    _currentTimeOffset = GameClock.CounterToTimeSpan(counter - _baseRealTime);
                }
                catch (OverflowException ex)
                {
                    _baseRealTime = counter;
                    _currentTimeOffset = TimeSpan.Zero;
                }
            }

            try
            {
                _elapsedTime = GameClock.CounterToTimeSpan(counter - _lastRealTime);
            }
            catch (OverflowException e)
            {
                _elapsedTime = TimeSpan.Zero;
            }

            try
            {
                long total = _lastRealTime + _timeLostToSuspension;

                _elapsedAdjustedTime = GameClock.CounterToTimeSpan(counter - total);
                _timeLostToSuspension = 0L;
            }
            catch (OverflowException e)
            {
                _elapsedAdjustedTime = TimeSpan.Zero;
            }

            _lastRealTime = counter;
        }

        internal void Suspend()
        {
            ++_suspendCount;

            if (_suspendCount == 1)
                _suspendStartTime = GameClock.Counter;
        }

        internal void Resume()
        {
            --_suspendCount;

            if (_suspendCount <= 0)
            {
                _timeLostToSuspension += GameClock.Counter - _suspendStartTime;
                _suspendStartTime = 0L;
            }
        }

        private static TimeSpan CounterToTimeSpan(long delta)
        {
            long num = 10000000L;
            return TimeSpan.FromTicks(checked(delta * num) / GameClock.Frequency);
        }
    }
}
