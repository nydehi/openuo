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
using Client.Core;
using Client.Graphics;
using Client.Graphics.Shaders;
using Ninject;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.UI
{
    public sealed class UserInterfaceManager : IUserInterfaceManager
    {
        private List<Element> _elements;
        private DiffuseShader _shader;
        private Device _device;
        private bool _projectionDirty;
        private Matrix _projection;

        private ITextureFactory _textureFactory;
        private IRenderer _renderer;

        public ITextureFactory TextureFactory
        {
            get { return _textureFactory; }
        }

        [Inject]
        public UserInterfaceManager(IKernel kernel)
        {
            IDeviceProvider provider = kernel.Get<IDeviceProvider>();

            provider.RenderForm.Resize += OnRenderFormResize;

            _device = provider.Device;
            _textureFactory = kernel.Get<ITextureFactory>();
            _renderer = kernel.Get<IUserInterfaceRenderer>();
            _elements = new List<Element>();
            _projectionDirty = true;
        }

        private void OnRenderFormResize(object sender, EventArgs e)
        {
            _projectionDirty = true;
        }

        public void Clear()
        {
            _elements.Clear();
        }

        public void Add(Element element)
        {
            _elements.Add(element);
        }

        public void Remove(Element element)
        {
            _elements.Add(element);
        }

        public void Update(UpdateState state)
        {
            foreach (Element element in _elements)
                element.Update(state);
        }

        public void Render(DrawState state)
        {
            if (_projectionDirty)
            {
                _projectionDirty = false;
                Matrix.OrthoOffCenterLH(0, _device.Viewport.Width, _device.Viewport.Height, 0, 0, 1, out _projection);
            }

            state.PushRenderer(_renderer);

            state.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            state.Device.SetRenderState(RenderState.AlphaTestEnable, true);
            state.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            state.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            state.PushProjection(_projection);

            _shader.Begin(state);

            foreach (Element element in _elements)
                element.Render(state);

            state.PopRenderer();
            state.PopProjection();

            _shader.End();
        }

        public void CreateResources()
        {
            _shader = new DiffuseShader(_device);
            _shader.CreateResources();
        }

        public void OnDeviceLost()
        {
            _shader.OnDeviceLost();
        }

        public void OnDeviceReset()
        {
            _shader.OnDeviceReset();
        }

        public void Dispose()
        {
            _shader.Dispose();
        }
    }
}
