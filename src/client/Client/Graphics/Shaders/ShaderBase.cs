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

using System.Collections;

using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics.Shaders
{
    public abstract class ShaderBase
    {
        private readonly Hashtable _effectHandles = new Hashtable();
        private readonly Effect _effect;
        private readonly EffectHandle _technique;

        protected ShaderBase(Engine engine, byte[] _effectData)
        {
            _effect = Effect.FromMemory(engine.Device, _effectData, ShaderFlags.None);
            _technique = _effect.GetTechnique(0);
        }

        protected abstract void BeginOverride(DrawState state);

        public virtual void Begin(DrawState state)
        {
            state.Flush();
            BeginOverride(state);

            _effect.Begin();
            _effect.BeginPass(0);
        }

        public virtual void End()
        {
            _effect.EndPass();
            _effect.End();
        }

        public void SetValue(string name, DataStream val, int offset, int countInBytes)
        {
            _effect.SetRawValue(name, val, offset, countInBytes);
        }

        public void SetValue(string name, DataStream val)
        {
            _effect.SetRawValue(name, val);
        }

        public void SetValue(string name, Texture val)
        {
            _effect.SetTexture(name, val);
        }

        public void SetValue(string name, string val)
        {
            _effect.SetString(name, val);
        }

        public void SetValue(string name, bool val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue(string name, bool[] val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue(string name, Color4 val)
        {
            _effect.SetValue(name, val.ToVector4());
        }

        public void SetValue(string name, Color4[] val)
        {
            Vector4[] colors = new Vector4[val.Length];

            for (int i = 0; i < colors.Length; i++)
                colors[i] = val[i].ToVector4();

            _effect.SetValue(name, colors);
        }

        public void SetValue(string name, int val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue(string name, int[] val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue(string name, Matrix val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue(string name, Matrix[] val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue(string name, float val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue(string name, float[] val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue(string name, Vector4 val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue(string name, Vector4[] val)
        {
            _effect.SetValue(name, val);
        }

        public void SetValue<T>(string name, T val) where T : struct
        {
            _effect.SetValue(name, val);
        }
    }
}
