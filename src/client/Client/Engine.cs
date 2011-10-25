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
using System.Drawing;
using System.Windows.Forms;
using Client.Diagnostics;
using Client.Graphics;
using Client.Graphics.Shaders;
using Client.Ultima;
using Ninject;
using SharpDX;
using SharpDX.Diagnostics;
using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace Client
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
        private readonly Device _device;
        private readonly RenderForm _form;

        private PresentParameters _presentParameters;

        private int _targetFrameRate;

        private bool _drawRunningSlowly;
        private bool _doneFirstUpdate;
        private bool _shouldStop;

        private TimeSpan _totalGameTime;
        private TimeSpan _lastFrameTotalGameTime;
        private TimeSpan _lastFrameElapsedGameTime;

        private Camera2D _camera;
        //private IInputService _inputService;
        private TextureFactory _textureFactory;
        private DiffuseShader _shader;
        private Maps _maps;

        public RenderForm RenderForm
        {
            get { return _form; }
        }

        public IKernel Kernel
        {
            get { return _kernel; }
        }

        public Device Device
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

        public Engine(IKernel kernel)
        {
            Direct3D direct3D = new Direct3D();

            _form = new RenderForm("OpenUO - A truely open Ultima Online client");
            _form.Size = new System.Drawing.Size(100, 100);
            _form.UserResized += _form_UserResized;

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
            _presentParameters.PresentationInterval = PresentInterval.Default;
            _presentParameters.Windowed = true;
            _presentParameters.DeviceWindowHandle = _form.Handle;

            _device = new Device(direct3D, 0, DeviceType.Hardware, _form.Handle, CreateFlags.HardwareVertexProcessing, _presentParameters);
            _kernel = kernel;
            _totalGameTime = TimeSpan.Zero;
            _lastFrameElapsedGameTime = TimeSpan.Zero;
            _drawState = new DrawState();
            _updateState = new UpdateState();
            _clock = new GameClock();
        }

        void _form_UserResized(object sender, EventArgs e)
        {
            if (_form.WindowState == FormWindowState.Minimized)
                return;

            _presentParameters.BackBufferWidth = _form.ClientSize.Width;
            _presentParameters.BackBufferHeight = _form.ClientSize.Height;

            _device.Reset(ref _presentParameters);
        }

        ~Engine()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Run()
        {
            try
            {
                Tracer.Info("Initializing Engine...");
                Initialize();

                _updateState.ElapsedGameTime = TimeSpan.Zero;
                _updateState.TotalGameTime = _totalGameTime;
                _updateState.IsRunningSlowly = false;

                Update(_updateState);

                _doneFirstUpdate = true;

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
            _updateState.IsRunningSlowly = _drawRunningSlowly;

            Update(_updateState);

            _lastFrameElapsedGameTime = elapsedTime;
            _lastFrameTotalGameTime = _totalGameTime;

            DrawFrame();
        }

        private void Initialize()
        {
            _camera = new Camera2D(this);
            _camera.NearClip = -1000;
            _camera.FarClip = 1000;
            _camera.Position = new Vector2(1496, 1624);

            //_inputService = (IInputService)Services.GetService(typeof(IInputService));
            //_inputService.MouseMove += _inputService_MouseMove;
            //_inputService.KeyDown += _inputService_KeyDown;

            //_textureFactory = new TextureFactory(this);
            _maps = new Maps(this);
            _shader = new DiffuseShader(this);
            //_renderer = new Renderer(this);

            Tracer.Info("Loading Content...");
            LoadContent();
        }

        private void Update(UpdateState state)
        {

        }

        private bool BeginDraw()
        {
            return true;
        }

        private void Draw(DrawState state)
        {
            if (_textureFactory == null)
                _textureFactory = new TextureFactory(this);

            state.PushProjection(_camera.Projection);

            state.Device.SetRenderState(RenderState.AlphaBlendEnable, true);
            state.Device.SetRenderState(RenderState.AlphaTestEnable, true);

            _shader.Begin(state);

            Vector2 cameraOffset = new Vector2(0.5f, 0.5f);
            Vector2 cameraPosition = state.Camera.Position + cameraOffset;
            Vector2 viewSize = new Vector2(_presentParameters.BackBufferWidth, _presentParameters.BackBufferHeight);
            Vector2 tileCounts = new Vector2(viewSize.X / 22, viewSize.Y / 22);

            Tile centerTile = _maps.Felucca.Tiles.GetLandTile((int)cameraPosition.X, (int)cameraPosition.Y);
            int centerTileZ = centerTile._z * 4;

            tileCounts.X = Math.Min(tileCounts.X, MaxTileDistanceTime2);
            tileCounts.Y = Math.Min(tileCounts.Y, MaxTileDistanceTime2);

            int startX = (int)(cameraPosition.X - tileCounts.X);
            int startY = (int)(cameraPosition.Y - tileCounts.Y);
            int endX = (int)(cameraPosition.X + tileCounts.X);
            int endY = (int)(cameraPosition.Y + tileCounts.Y);

            Vector2 offset, northVector, eastVector, westVector, southVector, center;
            int tileZ, eastTileZ, southTileZ, southEastTileZ;

            float widthInPixels = tileCounts.X * TileStepX;
            float heightInPixels = tileCounts.Y * TileStepY;

            for (int y = startY; y < endY; y++)
            {
                offset.X = widthInPixels + (((-tileCounts.X) + (startY - y)) * TileStepY);
                offset.Y = heightInPixels + ((tileCounts.Y) + (startY - y)) * TileStepY;

                //BoundingBox bb;

                for (int x = startX; x < endX; x++)
                {
                    Tile tile = _maps.Felucca.Tiles.GetLandTile(x, y);
                    Tile eastTile = _maps.Felucca.Tiles.GetLandTile(x + 1, y);
                    Tile southTile = _maps.Felucca.Tiles.GetLandTile(x, y + 1);
                    Tile southEastTile = _maps.Felucca.Tiles.GetLandTile(x + 1, y + 1);

                    offset.X += TileStepX;
                    offset.Y -= TileStepY;

                    tileZ = (tile._z * 4) + centerTileZ;
                    eastTileZ = (eastTile._z * 4) + centerTileZ;
                    southTileZ = (southTile._z * 4) + centerTileZ;
                    southEastTileZ = (southEastTile._z * 4) + centerTileZ;

                    center.X = offset.X;
                    center.Y = offset.Y;

                    northVector.X = center.X;
                    northVector.Y = center.Y + TileSizeOver2 + tileZ;

                    eastVector.X = center.X - TileSizeOver2;
                    eastVector.Y = center.Y + southTileZ;

                    westVector.X = center.X + TileSizeOver2;
                    westVector.Y = center.Y + eastTileZ;

                    southVector.X = center.X;
                    southVector.Y = (center.Y - TileSizeOver2) + southEastTileZ;

                    //bb.Max = new Vector3(eastVector.X, southVector.Y, float.MinValue);
                    //bb.Max = new Vector3(eastVector.X, southVector.Y, float.MinValue);

                    //if (_camera.BoundingFrustum.Contains(bb) != ContainmentType.Disjoint)
                    state.DrawQuad(ref northVector, ref eastVector, ref westVector, ref southVector, _textureFactory.CreateLand(this, tile._id));

                    //HuedTile[] statics = _maps.Felucca.Tiles.GetStaticTiles(x, y);

                    //for (int i = 0; i < statics.Length; i++)
                    //{
                    //    HuedTile s = statics[i];
                    //    Texture2D texture = _textureFactory.CreateStatic(s._id);

                    //    int staticHeight = s._z * 4 + centerTileZ - 22;

                    //    int height = texture.Height;
                    //    int width = texture.Width;
                    //    int widthOver2 = width / 2;

                    //    northVector.X = center.X - widthOver2;
                    //    northVector.Y = center.Y + height + staticHeight;

                    //    eastVector.X = center.X - widthOver2;
                    //    eastVector.Y = center.Y + staticHeight;

                    //    westVector.X = center.X + widthOver2;
                    //    westVector.Y = center.Y + height + staticHeight;

                    //    southVector.X = center.X + widthOver2;
                    //    southVector.Y = center.Y + staticHeight;

                    //    bb.Min = new Vector3(eastVector.X, southVector.Y, float.MinValue);
                    //    bb.Max = new Vector3(westVector.X, northVector.Y, float.MaxValue);

                    //    if (_camera.BoundingFrustum.Contains(bb) != ContainmentType.Disjoint)
                    //        state.DrawQuad(ref northVector, ref eastVector, ref westVector, ref southVector, texture);
                    //}
                }
            }

            state.Flush();
            state.PopProjection();

            _shader.End();
        }

        private void EndDraw()
        {

        }

        private void LoadContent()
        {

        }

        private void UnloadContent()
        {

        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                lock (this)
                {
                    Tracer.Info("Unloading Content...");
                    UnloadContent();
                }
            }
        }

        private void DrawFrame()
        {
            try
            {
                if (_doneFirstUpdate && (BeginDraw()))
                {
                    _drawState.TotalGameTime = _totalGameTime;
                    _drawState.ElapsedGameTime = _lastFrameElapsedGameTime;
                    _drawState.IsRunningSlowly = _drawRunningSlowly;
                    _drawState.Device = _device;
                    _drawState.Camera = _camera;
                    _drawState.Reset();

                    _device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.CornflowerBlue, 1.0f, 0);
                    _device.BeginScene();

                    Draw(_drawState);

                    _device.EndScene();
                    _device.Present();

                    EndDraw();
                }
            }
            finally
            {
                _lastFrameElapsedGameTime = TimeSpan.Zero;
            }
        }

        internal void Stop()
        {
            _shouldStop = true;
        }
    }
}
