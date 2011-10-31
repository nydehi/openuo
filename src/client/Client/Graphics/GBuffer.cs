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

using Ninject;
using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace Client.Graphics
{
    public class GBuffer : IResourceContainer
    {
        private Device _device;
        private RenderForm _form;
        private Texture _diffuseTarget;
        private Texture _normalTarget;
        private Texture _lightingTarget;
        private Texture _combineTarget;
        private Surface _backBuffer;

        public Texture DiffuseTexture
        {
            get { return _diffuseTarget; }
        }

        public Texture NormalTexture
        {
            get { return _normalTarget; }
        }

        public Texture LightingTexture
        {
            get { return _lightingTarget; }
        }

        public Texture FinalTexture
        {
            get { return _combineTarget; }
        }

        public GBuffer(IKernel kernel)
        {
            IDeviceProvider provider = kernel.Get<IDeviceProvider>();

            _device = provider.Device;
            _form = provider.RenderForm;
        }

        public void CreateResources()
        {
            _diffuseTarget = new Texture(_device, _form.ClientSize.Width, _form.ClientSize.Height, 0, Usage.RenderTarget, Format.A8B8G8R8, Pool.Managed);
            _normalTarget = new Texture(_device, _form.ClientSize.Width, _form.ClientSize.Height, 0, Usage.RenderTarget, Format.A8B8G8R8, Pool.Managed);
            _lightingTarget = new Texture(_device, _form.ClientSize.Width, _form.ClientSize.Height, 0, Usage.RenderTarget, Format.A8B8G8R8, Pool.Managed);
            _combineTarget = new Texture(_device, _form.ClientSize.Width, _form.ClientSize.Height, 0, Usage.RenderTarget, Format.A8B8G8R8, Pool.Managed);
        }

        public void OnDeviceLost()
        {
            Dispose();
        }

        public void OnDeviceReset()
        {
            CreateResources();
        }

        public void Begin()
        {
            _backBuffer = _device.GetRenderTarget(0);
        }

        public void End()
        {
            _device.SetRenderTarget(0, _backBuffer);
        }

        public void BeginClear()
        {
            _device.SetRenderTarget(0, _diffuseTarget.GetSurfaceLevel(0));
            _device.SetRenderTarget(0, _normalTarget.GetSurfaceLevel(0));
            _device.SetRenderTarget(0, _lightingTarget.GetSurfaceLevel(0));
            _device.SetRenderTarget(0, _combineTarget.GetSurfaceLevel(0));
        }

        public void EndClear()
        {
            _device.SetRenderTarget(0, _backBuffer);
            _device.SetRenderTarget(0, null);
            _device.SetRenderTarget(0, null);
            _device.SetRenderTarget(0, null);
        }

        public void BeginRenderPass()
        {
            _device.SetRenderTarget(0, _diffuseTarget.GetSurfaceLevel(0));
            _device.SetRenderTarget(0, _normalTarget.GetSurfaceLevel(0));
        }

        public void EndRenderPass()
        {
            _device.SetRenderTarget(0, _backBuffer);
            _device.SetRenderTarget(0, null);
        }

        public void BeginLightingPass()
        {
            _device.SetRenderTarget(0, _lightingTarget.GetSurfaceLevel(0));
        }

        public void EndLightingPass()
        {
            _device.SetRenderTarget(0, _backBuffer);
        }

        public void BeginCombine()
        {

        }

        public void Dispose()
        {
            _diffuseTarget.Dispose();
            _normalTarget.Dispose();
            _lightingTarget.Dispose();
            _combineTarget.Dispose();
        }
    }
}
