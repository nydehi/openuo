#region License Header
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
 #endregion

using System.Collections;

using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Graphics.Shaders
{
    public abstract class ShaderBase : IResourceContainer
    {
        private readonly Device _device;
        private readonly Hashtable _effectHandles = new Hashtable();
        private readonly byte[] _effectData;

        private Effect _effect;  

        protected ShaderBase(Device device, byte[] effectData)
        {
            _device = device;
            _effectData = effectData;
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

        public void Dispose()
        {
            _effect.Dispose();            
        }

        public void CreateResources()
        {
            _effect = Effect.FromMemory(_device, _effectData, ShaderFlags.None);
        }

        public void OnDeviceLost()
        {
            _effect.Dispose();
        }

        public void OnDeviceReset()
        {
            CreateResources();
        }
    }
}
