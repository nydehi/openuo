using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX.Direct3D9;
using SharpDX;

namespace OpenUO.DirectX9.Graphics
{
    public sealed class BlendState : IDisposable
    {
        private Blend _colorSourceBlend;
        private Blend _colorDestinationBlend;
        private BlendOperation _colorBlendFunction;
        private Blend _alphaSourceBlend;
        private Blend _alphaDestinationBlend;
        private BlendOperation _alphaBlendFunction;
        private Channel _colorWriteChannels0;
        private Channel _colorWriteChannels1;
        private Channel _colorWriteChannels2;
        private Channel _colorWriteChannels3;
        private Color4 _blendFactor;
        private int _multiSampleMask;
        private bool _isBound;
        private bool _blendEnabled;
        private bool _seperateAlphaBlend;
        private string _name;

        public BlendState()
        {
            try
            {
                SetDefaults();
                _isBound = false;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        public BlendState(Blend sourceBlend, Blend destinationBlend, string name)
        {
            try
            {
                SetDefaults();
                _colorSourceBlend = sourceBlend;
                _colorDestinationBlend = destinationBlend;
                _alphaSourceBlend = sourceBlend;
                _alphaDestinationBlend = destinationBlend;
                _name = name;
                _isBound = true;
            }
            catch
            {
                Dispose();
                throw;
            }
        }

        private void SetDefaults()
        {
            _colorSourceBlend = Blend.One;
            _colorDestinationBlend = Blend.Zero;
            _colorBlendFunction = BlendOperation.Add;
            _alphaSourceBlend = Blend.One;
            _alphaDestinationBlend = Blend.Zero;
            _alphaBlendFunction = BlendOperation.Add;
            _colorWriteChannels0 = AllChannels();
            _colorWriteChannels1 = AllChannels();
            _colorWriteChannels2 = AllChannels();
            _colorWriteChannels3 = AllChannels();
            _blendFactor = new Color4(1f);
            _multiSampleMask = -1;
        }

        public void Apply(Device device)
        {
            device.SetRenderState(RenderState.AlphaBlendEnable, _blendEnabled);

            if (_blendEnabled)
            {
                device.SetRenderState(RenderState.SourceBlend, _colorSourceBlend);
                device.SetRenderState(RenderState.DestinationBlend, _colorDestinationBlend);
                device.SetRenderState(RenderState.BlendOperation, _colorBlendFunction);
                device.SetRenderState(RenderState.SeparateAlphaBlendEnable, _seperateAlphaBlend);

                if (_seperateAlphaBlend)
                {
                    device.SetRenderState(RenderState.SourceBlendAlpha, _alphaSourceBlend);
                    device.SetRenderState(RenderState.DestinationBlendAlpha, _alphaDestinationBlend);
                    device.SetRenderState(RenderState.BlendOperationAlpha, _alphaBlendFunction);
                }
            }

            device.SetRenderState(RenderState.ColorWriteEnable, _colorWriteChannels0);
            device.SetRenderState(RenderState.ColorWriteEnable1, _colorWriteChannels1);
            device.SetRenderState(RenderState.ColorWriteEnable2, _colorWriteChannels2);
            device.SetRenderState(RenderState.ColorWriteEnable3, _colorWriteChannels3);
            device.SetRenderState(RenderState.BlendFactor, _blendFactor.ToArgb());
            device.SetRenderState(RenderState.MultisampleMask, _multiSampleMask);
        }

        private Channel AllChannels()
        {
            return (Channel)(1 | 2 | 4 | 8 | 16);
        }
        
        public void Dispose()
        {

        }
    }
}
