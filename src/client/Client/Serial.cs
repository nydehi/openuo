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

namespace Client
{
    public struct Serial : IComparable, IComparable<Serial>
    {
        private int _value;
        
        public static readonly Serial MinusOne = new Serial(-1);
        public static readonly Serial Zero = new Serial(0);

        private Serial(int value)
        {
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        public bool IsMobile
        {
            get { return (_value > 0 && _value < 0x40000000); }
        }

        public bool IsItem
        {
            get { return (_value >= 0x40000000 && _value <= 0x7FFFFFFF); }
        }

        public bool IsValid
        {
            get { return (_value > 0); }
        }

        public override int GetHashCode()
        {
            return _value;
        }

        public int CompareTo(Serial other)
        {
            return _value.CompareTo(other._value);
        }

        public int CompareTo(object other)
        {
            if (other is Serial)
                return this.CompareTo((Serial)other);
            else if (other == null)
                return -1;

            throw new ArgumentException();
        }

        public override bool Equals(object o)
        {
            if (o == null || !(o is Serial)) return false;

            return ((Serial)o)._value == _value;
        }

        public static bool operator ==(Serial l, Serial r)
        {
            return l._value == r._value;
        }

        public static bool operator !=(Serial l, Serial r)
        {
            return l._value != r._value;
        }

        public static bool operator >(Serial l, Serial r)
        {
            return l._value > r._value;
        }

        public static bool operator <(Serial l, Serial r)
        {
            return l._value < r._value;
        }

        public static bool operator >=(Serial l, Serial r)
        {
            return l._value >= r._value;
        }

        public static bool operator <=(Serial l, Serial r)
        {
            return l._value <= r._value;
        }

        /*public static Serial operator ++ ( Serial l )
        {
            return new Serial( l + 1 );
        }*/

        public override string ToString()
        {
            return String.Format("0x{0:X8}", _value);
        }

        public static implicit operator int(Serial a)
        {
            return a._value;
        }

        public static implicit operator Serial(int a)
        {
            return new Serial(a);
        }
    }
}
