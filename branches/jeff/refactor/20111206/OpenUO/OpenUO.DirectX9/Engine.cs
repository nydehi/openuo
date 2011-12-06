#region License Header
/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   $Id: Engine.cs 15 2011-11-02 07:16:02Z fdsprod@gmail.com $:
 *
 *   This program is free software; you can redistribute it and/or modify
 *   it under the terms of the GNU General Public License as published by
 *   the Free Software Foundation; either version 3 of the License, or
 *   (at your option) any later version.
 ***************************************************************************/
 #endregion

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Diagnostics;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using OpenUO.Core;
using OpenUO.DirectX9.Graphics;
using OpenUO.Core.Diagnostics;

namespace OpenUO.DirectX9
{
    public abstract class Engine : IDisposable
    {
        public readonly TimeSpan TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        public readonly TimeSpan MaxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 10);

        private readonly DrawState _drawState;
        private readonly UpdateState _updateState;
        private readonly GameClock _clock;
        private readonly DeviceEx _device;
        private readonly RenderForm _form;
        
        private readonly List<IResourceContainer> _resouces;
        private readonly List<IUpdatable> _updatables;
        private readonly List<IRenderable> _renderables;

        private PresentParameters _presentParameters;

        private int _targetFrameRate;

        private bool _doneFirstUpdate;
        private bool _shouldStop;
        private bool _deviceLost;
        private bool _isFormResizing;

        private TimeSpan _totalGameTime;
        private TimeSpan _lastFrameTotalGameTime;
        private TimeSpan _lastFrameElapsedGameTime;

        private IRenderer _renderer;

        public RenderForm RenderForm
        {
            get { return _form; }
        }
        
        public DeviceEx Device
        {
            get { return _device; }
        }

        public int TargetFrameRate
        {
            get { return _targetFrameRate; }
            set
            {
                Asserter.AssertInRange("value", 1, 9999);
                _targetFrameRate = value;
            }
        }

        protected Engine()
        {
            _resouces = new List<IResourceContainer>();
            _updatables = new List<IUpdatable>();
            _renderables = new List<IRenderable>();
            
            _form = deviceProvider.RenderForm;
            _form.ResizeBegin += OnResizeBegin;
            _form.ResizeEnd += OnResizeEnd;
            _form.FormClosed += OnFormClosed;
            _form.Resize += new EventHandler(OnFormResize);
            _form.KeyDown += new KeyEventHandler(_form_KeyDown);

            _device = new DeviceEx(deviceProvider.Device.NativePointer);
            _presentParameters = deviceProvider.PresentParameters;
            _totalGameTime = TimeSpan.Zero;
            _lastFrameElapsedGameTime = TimeSpan.Zero;
            _drawState = new DrawState();
            _updateState = new UpdateState();
            _clock = new GameClock();
        }

        void _form_KeyDown(object sender, KeyEventArgs e)
        {
        }

        ~Engine()
        {
            Dispose(false);
        }

        protected virtual void Initialize()
        {
            Tracer.Info("Initializing Resources...");

            foreach (IResourceContainer resource in _resouces)
                resource.CreateResources();
        }

        protected virtual void Update(UpdateState state)
        {
            foreach (IUpdatable updatable in _updatables)
                updatable.Update(state);
        }

        protected virtual void OnBeforeRender()
        {

        }

        protected virtual void Render(DrawState state)
        {
            _device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);            
            _device.BeginScene();

            foreach (IRenderable renderable in _renderables)
                renderable.Render(state);

            RenderScene(state);

            _device.EndScene();
        }

        protected virtual void RenderScene(DrawState state)
        {

        }

        protected virtual void OnAfterRender()
        {

        }

        private void RenderFrame()
        {
            try
            {
                if (_deviceLost)
                {
                    _device.ResetEx(ref _presentParameters);
                    _deviceLost = false;
                    OnDeviceReset();
                    return;
                }

                _drawState.TotalGameTime = _totalGameTime;
                _drawState.ElapsedGameTime = _lastFrameElapsedGameTime;
                _drawState.Device = _device;
                //_drawState.Camera = _camera;
                _drawState.PushRenderer(_renderer);
                _drawState.Reset();

                OnBeforeRender();
                Render(_drawState);
                OnAfterRender();

                _device.Present();
            }
            catch (SharpDXException e)
            {
                if (_device.CheckDeviceState(_form.Handle) == DeviceState.DeviceLost)
                {
                    OnDeviceLost();
                    _deviceLost = true;
                }
            }
            finally
            {
                _lastFrameElapsedGameTime = TimeSpan.Zero;
            }
        }

        public void Run()
        {
            try
            {
                Tracer.Info("Initializing Engine...");
                Initialize();

                _updateState.ElapsedGameTime = TimeSpan.Zero;
                _updateState.TotalGameTime = TimeSpan.Zero;

                Update(_updateState);

                _doneFirstUpdate = true;

                Tracer.Info("Running Game Loop...");
                RenderLoop.Run(_form, Tick);
            }
            catch (SharpDXException ex)
            {
                //string errorMessage = ErrorManager.GetErrorMessage(ex.ResultCode.Code);

                //Tracer.Error(errorMessage);
                Tracer.Error(ex);
                throw;
            }
            catch (Exception e)
            {
                Tracer.Error(e);
                throw;
            }
        }

        public void Tick()
        {
            if (_shouldStop)
                return;

            _clock.Step();

            TimeSpan elapsedTime = _clock.ElapsedTime;

            if (elapsedTime < TimeSpan.Zero)
                elapsedTime = TimeSpan.Zero;

            _totalGameTime += elapsedTime;

            TimeSpan tickDelta = _totalGameTime - _lastFrameTotalGameTime;

            // TODO: Need to add vsync support.

            _updateState.ElapsedGameTime = elapsedTime;
            _updateState.TotalGameTime = _totalGameTime;

            Update(_updateState);

            _lastFrameElapsedGameTime = elapsedTime;
            _lastFrameTotalGameTime = _totalGameTime;

            if (!_isFormResizing)
                RenderFrame();
        }

        internal void Stop()
        {
            _shouldStop = true;
        }

        private void Bind(object obj)
        {
            IResourceContainer resource = obj as IResourceContainer;

            if (resource != null)
                _resouces.Add(resource);

            IRenderable renderable = obj as IRenderable;

            if (renderable != null)
                _renderables.Add(renderable);

            IUpdatable updatable = obj as IUpdatable;

            if (updatable != null)
                _updatables.Add(updatable);
        }

        private void Release(object obj)
        {
            IResourceContainer resource = obj as IResourceContainer;

            if (resource != null)
                _resouces.Remove(resource);

            IRenderable renderable = obj as IRenderable;

            if (renderable != null)
                _renderables.Remove(renderable);

            IUpdatable updatable = obj as IUpdatable;

            if (updatable != null)
                _updatables.Remove(updatable);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                Tracer.Info("Disposing Resources...");

                foreach (IResourceContainer resource in _resouces)
                    resource.Dispose();
            }
        }

        private void OnDeviceReset()
        {
            foreach (IResourceContainer resource in _resouces)
                resource.OnDeviceReset();
        }

        private void OnDeviceLost()
        {
            foreach (IResourceContainer resource in _resouces)
                resource.OnDeviceLost();
        }

        private void OnFormClosed(object sender, FormClosedEventArgs e)
        {
            Stop();
        }

        private void OnResizeBegin(object sender, EventArgs e)
        {
            _isFormResizing = true;
        }

        private void OnResizeEnd(object sender, EventArgs e)
        {
            _isFormResizing = false;
        }

        private void OnFormResize(object sender, EventArgs e)
        {
            if (_form.WindowState == FormWindowState.Minimized)
                return;

            OnDeviceLost();

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

            _device.Reset(ref _presentParameters);

            OnDeviceReset();
        }
    }
}
