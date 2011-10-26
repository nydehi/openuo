/***************************************************************************
 *   Copyright (c) 2011 OpenUO Software Team.
 *   All Right Reserved.
 *
 *   SVN revision information:
 *   $Author$:
 *   $Date$:
 *   $Revision$:
 *   $Id$:
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
        private readonly Stack<Matrix> _worldMatrixStack;
        private readonly Stack<Matrix> _viewMatrixStack;
        private readonly Stack<Matrix> _projectionMatrixStack;
        private readonly List<Texture> _textures;
        private readonly VertexPositionNormalTexture[] _vertices;
        private readonly ushort[] _indices;

        private TimeSpan _totalGameTime;
        private TimeSpan _elapsedGameTime;
        private bool _isRunningSlowly;
        private Device _device;
        private ICamera _camera;

        private int _drawCalls;

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

        public int DrawCalls
        {
            get { return _drawCalls; }
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
            _vertices = new VertexPositionNormalTexture[65536];

            for (int i = 0; i < _vertices.Length; i += 4)
            {
                _vertices[i + 0].TextureCoordinate = new Vector2(0, 0);
                _vertices[i + 1].TextureCoordinate = new Vector2(1, 0);
                _vertices[i + 2].TextureCoordinate = new Vector2(0, 1);
                _vertices[i + 3].TextureCoordinate = new Vector2(1, 1);
            }

            _indices = new ushort[30];

            int count = _indices.Length / 6;

            for (int i = 0; i < count; i++)
            {
                _indices[(i * 6) + 0] = (ushort)((i * 4) + 0);
                _indices[(i * 6) + 1] = (ushort)((i * 4) + 1);
                _indices[(i * 6) + 2] = (ushort)((i * 4) + 2);
                _indices[(i * 6) + 3] = (ushort)((i * 4) + 2);
                _indices[(i * 6) + 4] = (ushort)((i * 4) + 1);
                _indices[(i * 6) + 5] = (ushort)((i * 4) + 3);
            }
        }

        internal void Reset()
        {
            _worldMatrixStack.Clear();
            _viewMatrixStack.Clear();
            _projectionMatrixStack.Clear();

            _worldMatrixStack.Push(Matrix.Identity);
            _viewMatrixStack.Push(Matrix.Identity);
            _projectionMatrixStack.Push(Matrix.Identity);

            _drawCalls = 0;
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

            using (VertexDeclaration vertexDeclaration = new VertexDeclaration(_device, VertexPositionNormalTexture.VertexElements))
            using (VertexBuffer vertexBuffer = new VertexBuffer(_device, _vertices.Length * VertexPositionNormalTexture.SizeInBytes, Usage.WriteOnly, VertexFormat.None, Pool.Managed))
            using (IndexBuffer indexBuffer = new IndexBuffer(_device, sizeof(ushort) * _indices.Length, Usage.WriteOnly, Pool.Managed, true))
            {
                vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(_vertices);
                indexBuffer.Lock(0, 0, LockFlags.None).WriteRange(_indices);

                _device.VertexDeclaration = vertexDeclaration;
                _device.SetStreamSource(0, vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                _device.Indices = indexBuffer;

                int batches;

                for (int i = 0; i < _textures.Count; i += batches)
                {
                    Texture texture = _textures[i];

                    batches = 1;
                    int ix = i + batches;

                    while ((ix < _textures.Count && batches < 5) && texture == _textures[ix])
                    {
                        batches++;
                        ix++;
                    }

                    _device.SetTexture(0, texture);
                    _device.DrawIndexedPrimitive(PrimitiveType.TriangleList, i * 4, 0, batches * 4, 0, batches * 2);

                    _drawCalls++;
                }
            }

            _currentVertex = 0;
            _textures.Clear();
        }
    }
}
