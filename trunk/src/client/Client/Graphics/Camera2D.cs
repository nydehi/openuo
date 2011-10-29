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

using SharpDX;
using Client.Core;

namespace Client.Graphics
{
    public class Camera2D : ICamera
    {
        private readonly Engine _engine;

        private int _width;
        private int _height;
        private int _nearClip;
        private int _farClip = 1;
        private bool _projectionDirty;
        private bool _transformDirty;
        private bool _boundingFrustumDirty;
        private Vector3 _position;
        private Vector2 _halfVector;
        private Matrix _view;
        private Matrix _projection;
        private BoundingFrustum _boundingFrustum;

        public BoundingFrustum BoundingFrustum
        {
            get
            {
                if (_boundingFrustumDirty)
                {
                    _boundingFrustumDirty = false;
                    _boundingFrustum = new BoundingFrustum(Projection);
                }

                return _boundingFrustum;
            }
        }

        public Matrix Projection
        {
            get
            {
                if (_projectionDirty)
                {
                    _projectionDirty = false;
                    Matrix.OrthoRH(_width, _height, _nearClip, _farClip, out _projection);
                }

                return _projection;
            }
        }

        public Vector2 Position
        {
            get { return new Vector2(_position.X, _position.Y); }
            set
            {
                _position.X = value.X;
                _position.Y = value.Y;
                _transformDirty = true;
                _boundingFrustumDirty = true;
            }
        }

        public int NearClip
        {
            get { return _nearClip; }
            set
            {
                _nearClip = value;
                _projectionDirty = true;
                _boundingFrustumDirty = true;
            }
        }

        public int FarClip
        {
            get { return _farClip; }
            set
            {
                _farClip = value;
                _projectionDirty = true;
                _boundingFrustumDirty = true;
            }
        }

        public Vector2 HalfVector
        {
            get { return _halfVector; }
        }

        public Camera2D(Engine engine)
        {
            _projectionDirty = true;
            _transformDirty = true;
            _boundingFrustumDirty = true;

            _engine = engine;
            _engine.RenderForm.Resize += OnRenderFormResize;

            _width = _engine.RenderForm.ClientSize.Width;
            _height = engine.RenderForm.ClientSize.Height;
            _halfVector = new Vector2(1f / _width, 1f / _height);
        }

        private void OnRenderFormResize(object sender, System.EventArgs e)
        {
            _width = _engine.RenderForm.ClientSize.Width;
            _height = _engine.RenderForm.ClientSize.Height;
            _halfVector = new Vector2(1f / _width, 1f / _height);
            _projectionDirty = true;
            _boundingFrustumDirty = true;
        }
    }
}
