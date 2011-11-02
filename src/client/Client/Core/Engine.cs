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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Client.Cores;
using Client.Diagnostics;
using Client.Graphics;
using Client.Graphics.Shaders;
using Client.Input;
using Client.UI;
using Client.Ultima;
using Ninject;
using SharpDX;
using SharpDX.Diagnostics;
using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace Client.Core
{
    public class Engine : IDisposable
    {
        const int TileSize = 44;
        const int TileSizeOver2 = TileSize / 2;
        const int TileStepX = 22;
        const int TileStepY = 22;

        const int MaxTileDistance = 23;
        const int MaxTileDistanceTime2 = MaxTileDistance * 2;

        public readonly TimeSpan TargetElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 60);
        public readonly TimeSpan MaxElapsedTime = TimeSpan.FromTicks(TimeSpan.TicksPerSecond / 10);

        private readonly DrawState _drawState;
        private readonly UpdateState _updateState;
        private readonly GameClock _clock;
        private readonly IKernel _kernel;
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

        private Camera2D _camera;
        private DiffuseShader _shader;
        private World _world;
        private IRenderer _renderer;

        public RenderForm RenderForm
        {
            get { return _form; }
        }

        public IKernel Kernel
        {
            get { return _kernel; }
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
        Maps _maps;
        public Engine(IKernel kernel)
        {
            _kernel = kernel;
            _resouces = new List<IResourceContainer>();
            _updatables = new List<IUpdatable>();
            _renderables = new List<IRenderable>();

            IDeviceProvider deviceProvider = kernel.Get<IDeviceProvider>();

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

        private void Initialize()
        {
            _camera = new Camera2D(this);
            _camera.NearClip = -1000;
            _camera.FarClip = 1000;
            _camera.Position = new Vector2(1496, 1624);

            _renderer = _kernel.Get<IRenderer>();

            IUserInterfaceManager userInterface = _kernel.Get<IUserInterfaceManager>();
            ITextureFactory textureFactory = _kernel.Get<ITextureFactory>();
            IUserInterfaceRenderer userInterfaceRenderer = _kernel.Get<IUserInterfaceRenderer>();
            IInputService inputService = _kernel.Get<IInputService>();
            
            BackgroundElement element = new BackgroundElement(userInterface, 5054);
            element.Position = new Vector2(50, 50);
            element.Size = new Vector2(300, 300);

            TextElement t = new TextElement(userInterface, 0, "This is a test.");
            t.Position = new Vector2(60, 60);

            userInterface.Add(element);
            userInterface.Add(t);

            Bind(_renderer);
            Bind(inputService);
            Bind(textureFactory);
            Bind(userInterface);
            Bind(userInterfaceRenderer);

            Tracer.Info("Initializing Resources...");

            foreach (IResourceContainer resource in _resouces)
                resource.CreateResources();
        }

        private void Update(UpdateState state)
        {
            foreach (IUpdatable updatable in _updatables)
                updatable.Update(state);
        }

        private void OnBeforeRender()
        {

        }

        private void Render(DrawState state)
        {
            _device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);            
            _device.BeginScene();

            foreach (IRenderable renderable in _renderables)
                renderable.Render(state);

            _device.EndScene();

            //state.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            //state.Device.SetRenderState(RenderState.AlphaTestEnable, true);
            //state.Device.SetRenderState(RenderState.SourceBlend, Blend.SourceAlpha);
            //state.Device.SetRenderState(RenderState.DestinationBlend, Blend.InverseSourceAlpha);

            //state.PushProjection(_camera.Projection);

            //_shader.Begin(state);

            //Vector2 cameraOffset = new Vector2(0.5f, 0.5f);
            //Vector2 cameraPosition = state.Camera.Position + cameraOffset;
            //Vector2 viewSize = new Vector2(_presentParameters.BackBufferWidth, _presentParameters.BackBufferHeight);
            //Vector2 tileCounts = new Vector2(viewSize.X / 22, viewSize.Y / 22);

            //Tile centerTile = _maps.Felucca.Tiles.GetLandTile((int)cameraPosition.X, (int)cameraPosition.Y);
            //int centerTileZ = centerTile._z * 4;

            //tileCounts.X = 0.5f; //Math.Min(tileCounts.X, MaxTileDistanceTime2);
            //tileCounts.Y = 0.5f;// Math.Min(tileCounts.Y, MaxTileDistanceTime2);

            //int startX = (int)(cameraPosition.X - tileCounts.X);
            //int startY = (int)(cameraPosition.Y - tileCounts.Y);
            //int endX = (int)(cameraPosition.X + tileCounts.X);
            //int endY = (int)(cameraPosition.Y + tileCounts.Y);

            //Vector2 offset, northVector, eastVector, westVector, southVector, center;

            //float widthInPixels = tileCounts.X * TileStepX;
            //float heightInPixels = tileCounts.Y * TileStepY;

            //for (int y = startY; y < endY; y++)
            //{
            //    offset.X = widthInPixels + (((-tileCounts.X) + (startY - y)) * TileStepY);
            //    offset.Y = heightInPixels + ((tileCounts.Y) + (startY - y)) * TileStepY;

            //    BoundingBox bb;

            //    for (int x = startX; x < endX; x++)
            //    {
            //        Tile tile = _maps.Felucca.Tiles.GetLandTile(x, y);
            //        Tile eastTile = _maps.Felucca.Tiles.GetLandTile(x + 1, y);
            //        Tile southTile = _maps.Felucca.Tiles.GetLandTile(x, y + 1);
            //        Tile southEastTile = _maps.Felucca.Tiles.GetLandTile(x + 1, y + 1);

            //        int tileZ = (tile._z * 4) - centerTileZ;
            //        int eastTileZ = (eastTile._z * 4) - centerTileZ;
            //        int southTileZ = (southTile._z * 4) - centerTileZ;
            //        int southEastTileZ = (southEastTile._z * 4) - centerTileZ;

            //        center.X = offset.X;
            //        center.Y = offset.Y;

            //        northVector.X = center.X;
            //        northVector.Y = center.Y + TileSizeOver2 + tileZ;

            //        eastVector.X = center.X - TileSizeOver2;
            //        eastVector.Y = center.Y + southTileZ;

            //        westVector.X = center.X + TileSizeOver2;
            //        westVector.Y = center.Y + eastTileZ;

            //        southVector.X = center.X;
            //        southVector.Y = (center.Y - TileSizeOver2) + southEastTileZ;

            //        bb.Minimum = new Vector3(eastVector.X, southVector.Y, float.MinValue);
            //        bb.Maximum = new Vector3(westVector.X, northVector.Y, float.MaxValue);

            //        if (_camera.BoundingFrustum.Contains(bb) != ContainmentType.Disjoint)
            //            state.Renderer.RenderQuad(ref northVector, ref eastVector, ref westVector, ref southVector, _textureFactory.CreateLandTexture(tile._id));

            //        HuedTile[] statics = _maps.Felucca.Tiles.GetStaticTiles(x, y);

            //        for (int i = 0; i < statics.Length; i++)
            //        {
            //            HuedTile s = statics[i];
            //            Texture texture = _textureFactory.CreateStaticTexture(s._id);

            //            SurfaceDescription description = texture.GetLevelDescription(0);

            //            int staticHeight = s._z * 4 + centerTileZ - 22;

            //            int height = description.Height;
            //            int width = description.Width;
            //            int widthOver2 = width / 2;

            //            northVector.X = center.X - widthOver2;
            //            northVector.Y = center.Y + height + staticHeight;

            //            eastVector.X = center.X - widthOver2;
            //            eastVector.Y = center.Y + staticHeight;

            //            westVector.X = center.X + widthOver2;
            //            westVector.Y = center.Y + height + staticHeight;

            //            southVector.X = center.X + widthOver2;
            //            southVector.Y = center.Y + staticHeight;

            //            bb.Minimum = new Vector3(eastVector.X, southVector.Y, 0);
            //            bb.Maximum = new Vector3(westVector.X, northVector.Y, 0);

            //            if (_camera.BoundingFrustum.Contains(bb) != ContainmentType.Disjoint)
            //                state.Renderer.RenderQuad(ref northVector, ref eastVector, ref westVector, ref southVector, texture);
            //        }

            //        offset.X += TileStepX;
            //        offset.Y -= TileStepY;
            //    }
            //}

            //state.Flush();
            //state.PopProjection();

            //_shader.End();
            //_device.EndScene();
        }

        private void OnAfterRender()
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
                _drawState.Camera = _camera;
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
                string errorMessage = ErrorManager.GetErrorMessage(ex.ResultCode.Code);

                Tracer.Error(errorMessage);
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
