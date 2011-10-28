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

using System;
using System.Collections.Generic;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client.Core.Graphics
{
    public class DrawState
    {
        private readonly Stack<Matrix> _worldMatrixStack;
        private readonly Stack<Matrix> _viewMatrixStack;
        private readonly Stack<Matrix> _projectionMatrixStack;

        private TimeSpan _totalGameTime;
        private TimeSpan _elapsedGameTime;
        private Device _device;
        private ICamera _camera;
        private IRenderer _renderer;
        private float _frameAccumulator;
        private int _frameCount;
        private float _framesPerSecond;

        public IRenderer Renderer
        {
            get { return _renderer; }
            set { _renderer = value; }
        }

        public Device Device
        {
            get { return _device; }
            internal set { _device = value; }
        }

        public TimeSpan TotalGameTime
        {
            get { return _totalGameTime; }
            internal set { _totalGameTime = value; }
        }

        public TimeSpan ElapsedGameTime
        {
            get { return _elapsedGameTime; }
            internal set { _elapsedGameTime = value; }
        }

        public ICamera Camera
        {
            get { return _camera; }
            internal set { _camera = value; }
        }

        public Matrix World
        {
            get { return _worldMatrixStack.Peek(); }
        }

        public Matrix WorldView
        {
            get { return World * View; }
        }

        public Matrix WorldViewProjection
        {
            get { return World * View * Projection; }
        }

        public Matrix ViewProjection
        {
            get { return View * Projection; }
        }

        public Matrix View
        {
            get { return _viewMatrixStack.Peek(); }
        }

        public Matrix Projection
        {
            get { return _projectionMatrixStack.Peek(); }
        }

        //public DepthStencilState DepthStencilState
        //{
        //    get { return _graphicsDevice.DepthStencilState; }
        //    set
        //    {
        //        Flush();
        //        _graphicsDevice.DepthStencilState = value;
        //    }
        //}

        //public BlendState BlendState
        //{
        //    get { return _graphicsDevice.BlendState; }
        //    set
        //    {
        //        Flush();
        //        _graphicsDevice.BlendState = value;
        //    }
        //}

        //public Color BlendFactor
        //{
        //    get { return _graphicsDevice.BlendFactor; }
        //    set
        //    {
        //        Flush();
        //        _graphicsDevice.BlendFactor = value;
        //    }
        //}

        //public RasterizerState RasterizerState
        //{
        //    get { return _graphicsDevice.RasterizerState; }
        //    set
        //    {
        //        Flush();
        //        _graphicsDevice.RasterizerState = value;
        //    }
        //}

        public Viewport Viewport
        {
            get { return _device.Viewport; }
            set
            {
                _renderer.Flush();
                _device.Viewport = value;
            }
        }

        public Rectangle ScissorRectangle
        {
            get { return _device.ScissorRect; }
            set
            {
                _renderer.Flush();
                _device.ScissorRect = value;
            }
        }

        public DrawState()
        {
            _worldMatrixStack = new Stack<Matrix>();
            _viewMatrixStack = new Stack<Matrix>();
            _projectionMatrixStack = new Stack<Matrix>();
        }

        internal void Reset()
        {
            _worldMatrixStack.Clear();
            _viewMatrixStack.Clear();
            _projectionMatrixStack.Clear();

            _worldMatrixStack.Push(Matrix.Identity);
            _viewMatrixStack.Push(Matrix.Identity);
            _projectionMatrixStack.Push(Matrix.Identity);
        }

        public void PushWorld(Matrix world)
        {
            _worldMatrixStack.Push(world);
        }

        public void PushView(Matrix view)
        {
            _viewMatrixStack.Push(view);
        }

        public void PushProjection(Matrix projection)
        {
            _projectionMatrixStack.Push(projection);
        }

        public void PopWorld()
        {
            _worldMatrixStack.Pop();
        }

        public void PopProjection()
        {
            _projectionMatrixStack.Pop();
        }

        public void PopView()
        {
            _viewMatrixStack.Pop();
        }

        internal void Flush()
        {
            _renderer.Flush();
        }

        //internal void BeginDraw()
        //{
        //    _frameAccumulator += (float)_elapsedGameTime.TotalSeconds;
        //    _frameCount++;

        //    if (_frameAccumulator >= 1.0f)
        //    {
        //        _framesPerSecond = _frameCount / _frameAccumulator;
        //        _frameAccumulator = 0.0f;
        //        _frameCount = 0;
        //    }
        //}

    //    internal void EndDraw()
    //    {

    //    }
    }
}
