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
using System.Collections.Generic;
using Client.Graphics;
using SharpDX;
using SharpDX.Direct3D9;

namespace Client
{
    public class DrawState
    {
        private readonly VertexPositionColorTexture[] _vertices;
        private readonly ushort[] _indices;

        private TimeSpan _totalGameTime;
        private TimeSpan _elapsedGameTime;
        private bool _isRunningSlowly;
        private Device _device;
        private ICamera _camera;
        private Stack<Matrix> _worldMatrixStack;
        private Stack<Matrix> _viewMatrixStack;
        private Stack<Matrix> _projectionMatrixStack;

        private List<Texture> _textures;
        private int _currentVertex;

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

        public bool IsRunningSlowly
        {
            get { return _isRunningSlowly; }
            internal set { _isRunningSlowly = value; }
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
                Flush();
                _device.Viewport = value;
            }
        }

        public Rectangle ScissorRectangle
        {
            get { return _device.ScissorRect; }
            set
            {
                Flush();
                _device.ScissorRect = value;
            }
        }


        public DrawState()
        {
            _worldMatrixStack = new Stack<Matrix>();
            _viewMatrixStack = new Stack<Matrix>();
            _projectionMatrixStack = new Stack<Matrix>();

            _worldMatrixStack.Push(Matrix.Identity);
            _viewMatrixStack.Push(Matrix.Identity);
            _projectionMatrixStack.Push(Matrix.Identity);

            _textures = new List<Texture>();
            _vertices = new VertexPositionColorTexture[16384];

            for (int i = 0; i < _vertices.Length; i += 4)
            {
                _vertices[i + 0].TextureCoordinate = new Vector2(0, 0);
                _vertices[i + 1].TextureCoordinate = new Vector2(1, 0);
                _vertices[i + 2].TextureCoordinate = new Vector2(0, 1);
                _vertices[i + 3].TextureCoordinate = new Vector2(1, 1);
            }

            _indices = new ushort[] { 0, 1, 2, 2, 1, 3 };
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

        public void DrawQuad(ref Vector2 tl, ref Vector2 tr, ref Vector2 bl, ref Vector2 br, Texture texture)
        {
            if (_currentVertex + 4 >= _vertices.Length)
                Flush();

            _vertices[_currentVertex + 0].Position.X = tl.X;
            _vertices[_currentVertex + 0].Position.Y = tl.Y;

            _vertices[_currentVertex + 1].Position.X = bl.X;
            _vertices[_currentVertex + 1].Position.Y = bl.Y;

            _vertices[_currentVertex + 2].Position.X = tr.X;
            _vertices[_currentVertex + 2].Position.Y = tr.Y;

            _vertices[_currentVertex + 3].Position.X = br.X;
            _vertices[_currentVertex + 3].Position.Y = br.Y;

            _currentVertex += 4;
            _textures.Add(texture);
        }

        public void DrawQuad(DrawState state, Vector2 v1, Vector2 v2, Texture texture)
        {
            if (_currentVertex + 4 >= _vertices.Length)
                Flush();

            _vertices[_currentVertex + 0].Position.X = v1.X;
            _vertices[_currentVertex + 0].Position.Y = v1.Y;

            _vertices[_currentVertex + 1].Position.X = v1.X;
            _vertices[_currentVertex + 1].Position.Y = v2.Y;

            _vertices[_currentVertex + 2].Position.X = v2.X;
            _vertices[_currentVertex + 2].Position.Y = v1.Y;

            _vertices[_currentVertex + 3].Position.X = v2.X;
            _vertices[_currentVertex + 3].Position.Y = v2.Y;

            _currentVertex += 4;
            _textures.Add(texture);
        }

        internal void Flush()
        {
            if (_currentVertex == 0)
                return;

            using (VertexDeclaration vertexDeclaration = new VertexDeclaration(_device, VertexPositionColorTexture.VertexElements))
            using (VertexBuffer vertexBuffer = new VertexBuffer(_device, _vertices.Length * VertexPositionColorTexture.SizeInBytes, Usage.WriteOnly, VertexFormat.None, Pool.Managed))
            using (IndexBuffer indexBuffer = new IndexBuffer(_device, sizeof(ushort) * _indices.Length, Usage.WriteOnly, Pool.Managed, true))
            {
                vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(_vertices);
                indexBuffer.Lock(0, 0, LockFlags.None).WriteRange(_indices);

                _device.VertexDeclaration = vertexDeclaration;
                _device.SetStreamSource(0, vertexBuffer, 0, VertexPositionColorTexture.SizeInBytes);
                _device.Indices = indexBuffer;

                for (int i = 0; i < _textures.Count; i++)
                {
                    _device.SetTexture(0, _textures[i]);
                    _device.DrawIndexedPrimitive(PrimitiveType.TriangleList, i * 4, 0, 4, 0, 2);
                }
            }

            _currentVertex = 0;
            _textures.Clear();
        }
    }
}
