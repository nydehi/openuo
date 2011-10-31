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

using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace Client.Graphics
{

    public class DeviceProvider : IDeviceProvider
    {
        private readonly Device _device;
        private readonly RenderForm _form;
        private readonly PresentParameters _presentParameters;

        public Device Device
        {
            get { return _device; }
        }

        public RenderForm RenderForm
        {
            get { return _form; }
        }

        public PresentParameters PresentParameters
        {
            get { return _presentParameters; }
        }

        public DeviceProvider()
        {
            Direct3D direct3D = new Direct3D();

            _form = new RenderForm("OpenUO - A truely open Ultima Online client");

            _presentParameters = new PresentParameters();
            _presentParameters.BackBufferFormat = Format.X8R8G8B8;
            _presentParameters.BackBufferCount = 1;
            _presentParameters.BackBufferWidth = _form.ClientSize.Width;
            _presentParameters.BackBufferHeight = _form.ClientSize.Height;
            _presentParameters.MultiSampleType = MultisampleType.None;
            _presentParameters.SwapEffect = SwapEffect.Discard;
            _presentParameters.EnableAutoDepthStencil = true;
            _presentParameters.AutoDepthStencilFormat = Format.D24S8;
            _presentParameters.PresentFlags = PresentFlags.DiscardDepthStencil;
            _presentParameters.PresentationInterval = PresentInterval.Immediate;
            _presentParameters.Windowed = true;
            _presentParameters.DeviceWindowHandle = _form.Handle;

            _device = new Device(direct3D, 0, DeviceType.Hardware, _form.Handle, CreateFlags.HardwareVertexProcessing, _presentParameters);
        }
    }
}
