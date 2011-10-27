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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D9;
using SharpDX;
using Ninject;

namespace Client.Core.Graphics
{
    public sealed class Renderer : IRenderer
    {
        private readonly List<Texture> _textures;
        private readonly VertexPositionNormalTexture[] _vertices;

        private Device _device;
        private IndexBuffer _indexBuffer;
        private VertexDeclaration _vertexDeclaration;

        private int _currentVertex;
        private int _drawCalls;
        private int _maxBatchCount;
        private int _batchDraws;
        private int _maxBatches;

        public int DrawCalls
        {
            get { return _drawCalls; }
        }

        public int MaxBatchCount
        {
            get { return _maxBatchCount; }
        }

        public int BatchDraws
        {
            get { return _batchDraws; }
        }

        [Inject]
        public Renderer(IDeviceProvider provider)
        {
            _device = provider.Device;
            _textures = new List<Texture>();
            _vertices = new VertexPositionNormalTexture[65536];

            for (int i = 0; i < _vertices.Length; i += 4)
            {
                _vertices[i + 0].TextureCoordinate = new Vector2(0, 0);
                _vertices[i + 1].TextureCoordinate = new Vector2(1, 0);
                _vertices[i + 2].TextureCoordinate = new Vector2(0, 1);
                _vertices[i + 3].TextureCoordinate = new Vector2(1, 1);
            }
        }

        public void CreateResources()
        {
            ushort[] indices = new ushort[6*20];

            int count = indices.Length / 6;

            for (int i = 0; i < count; i++)
            {
                indices[(i * 6) + 0] = (ushort)((i * 4) + 0);
                indices[(i * 6) + 1] = (ushort)((i * 4) + 1);
                indices[(i * 6) + 2] = (ushort)((i * 4) + 2);
                indices[(i * 6) + 3] = (ushort)((i * 4) + 2);
                indices[(i * 6) + 4] = (ushort)((i * 4) + 1);
                indices[(i * 6) + 5] = (ushort)((i * 4) + 3);
            }

            _maxBatches = indices.Length / 6;
            _vertexDeclaration = new VertexDeclaration(_device, VertexPositionNormalTexture.VertexElements);
            _indexBuffer = new IndexBuffer(_device, sizeof(ushort) * indices.Length, Usage.WriteOnly, Pool.Managed, true);
        }

        public void Dispose()
        {
            _textures.Clear();
            _indexBuffer.Dispose();
            _vertexDeclaration.Dispose();
        }

        public void RenderQuad(ref Vector2 tl, ref Vector2 tr, ref Vector2 bl, ref Vector2 br, Texture texture)
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

        public void RenderQuad(DrawState state, Vector2 v1, Vector2 v2, Texture texture)
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

        public void Flush()
        {
            if (_currentVertex == 0)
                return;

            using (VertexBuffer vertexBuffer = new VertexBuffer(_device, _vertices.Length * VertexPositionNormalTexture.SizeInBytes, Usage.WriteOnly, VertexFormat.None, Pool.Managed))
            {
                vertexBuffer.Lock(0, 0, LockFlags.None).WriteRange(_vertices);

                _device.VertexDeclaration = _vertexDeclaration;
                _device.SetStreamSource(0, vertexBuffer, 0, VertexPositionNormalTexture.SizeInBytes);
                _device.Indices = _indexBuffer;

                int batches;

                for (int i = 0; i < _textures.Count; i += batches)
                {
                    Texture texture = _textures[i];

                    batches = 1;
                    int ix = i + batches;

                    while ((ix < _textures.Count && batches < _maxBatches) && texture == _textures[ix])
                    {
                        batches++;
                        ix++;
                    }

                    if (batches > 1)
                        _batchDraws++;

                    _maxBatchCount = Math.Max(batches, _maxBatchCount);

                    _device.SetTexture(0, texture);
                    _device.DrawIndexedPrimitive(PrimitiveType.TriangleList, i * 4, 0, batches * 4, 0, batches * 2);

                    _drawCalls++;
                }
            }

            _currentVertex = 0;
            _textures.Clear();
        }

        public Vector2 MeasureString(Font font, string text)
        {
            throw new NotImplementedException();
        }

        public void RenderString(Font font, string text, int x, int y, Color4 color)
        {
            throw new NotImplementedException();
        }

        public void RenderLine(int x0, int y0, Color4 color0, int x1, int y1, Color4 color1)
        {
            throw new NotImplementedException();
        }
        
        public void OnDeviceLost()
        {

        }        
        public void OnDeviceReset()
        {

        }
    }
}
