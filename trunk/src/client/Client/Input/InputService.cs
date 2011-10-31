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

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using Client.Cores;
using Client.Graphics;
using Ninject;
using SharpDX.Windows;

namespace Client.Input
{
    public class InputService : IInputService, IUpdatable
    {
        private Dictionary<Keys, InputBinding> _keyBindings;
        private Dictionary<MouseButtons, InputBinding> _mouseBindings;

        public static Keys ModifierKeys
        {
            get
            {
                Keys none = Keys.None;

                if (GetKeyState(0x10) < 0)
                {
                    none |= Keys.Shift;
                }

                if (GetKeyState(0x11) < 0)
                {
                    none |= Keys.Control;
                }

                if (GetKeyState(0x12) < 0)
                {
                    none |= Keys.Alt;
                }

                return none;
            }
        }

        public static MouseButtons MouseButtons
        {
            get
            {
                MouseButtons none = MouseButtons.None;

                if (GetKeyState(1) < 0)
                {
                    none |= MouseButtons.Left;
                }

                if (GetKeyState(2) < 0)
                {
                    none |= MouseButtons.Right;
                }

                if (GetKeyState(4) < 0)
                {
                    none |= MouseButtons.Middle;
                }

                if (GetKeyState(5) < 0)
                {
                    none |= MouseButtons.XButton1;
                }

                if (GetKeyState(6) < 0)
                {
                    none |= MouseButtons.XButton2;
                }

                return none;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, ExactSpelling = true)]
        public static extern short GetKeyState(int keyCode);

        public event EventHandler<HandledKeyEventArgs> KeyDown;
        public event EventHandler<HandledKeyEventArgs> KeyUp;

        public event EventHandler<HandledMouseEventArgs> MouseClick;
        public event EventHandler<HandledMouseEventArgs> MouseDoubleClick;
        public event EventHandler<HandledMouseEventArgs> MouseDown;
        public event EventHandler<HandledMouseEventArgs> MouseUp;
        public event EventHandler<HandledMouseEventArgs> MouseMove;
        public event EventHandler<HandledMouseEventArgs> MouseWheel;

        [Inject]
        public InputService(IKernel kernel)
        {
            IDeviceProvider provider = kernel.Get<IDeviceProvider>();

            RenderForm form = provider.RenderForm;

            form.KeyDown += OnKeyDown;
            form.KeyUp += OnKeyUp;
            form.MouseClick += OnMouseClick;
            form.MouseDoubleClick += OnMouseDoubleClick;
            form.MouseDown += OnMouseDown;
            form.MouseMove += OnMouseMove;
            form.MouseUp += OnMouseUp;
            form.MouseWheel += OnMouseWheel;

            _keyBindings = new Dictionary<Keys, InputBinding>();
            _mouseBindings = new Dictionary<MouseButtons, InputBinding>();
        }

        public InputBinding AddBinding(string name, bool shift, bool control, bool alt, Keys key, EventHandler handler)
        {
            return AddBinding(name, shift, control, alt, key, handler, null);
        }

        public InputBinding AddBinding(string name, bool shift, bool control, bool alt, Keys key, EventHandler beginHandler, EventHandler endHandler)
        {
            InputBinding binding = new InputBinding(name, shift, control, alt);

            binding.BeginExecution = beginHandler;
            binding.EndExecution = endHandler;

            key |= shift ? Keys.Shift : Keys.None;
            key |= control ? Keys.Control : Keys.None;
            key |= alt ? Keys.Alt : Keys.None;

            _keyBindings.Add(key, binding);

            return binding;
        }

        public InputBinding AddBinding(string name, MouseButtons buttons, EventHandler handler)
        {
            return AddBinding(name, buttons, handler, null);
        }

        public InputBinding AddBinding(string name, MouseButtons buttons, EventHandler beginHandler, EventHandler endHandler)
        {
            InputBinding binding = new InputBinding(name, false, false, false);

            binding.BeginExecution = beginHandler;
            binding.EndExecution = endHandler;

            _mouseBindings.Add(buttons, binding);

            return binding;
        }

        public void Update(Core.UpdateState state)
        {
            HandleKeyBindings();
            HandleMouseBindings();
        }

        private void HandleKeyBindings()
        {
            foreach (Keys keys in _keyBindings.Keys)
            {
                Keys key = keys;
                InputBinding binding = _keyBindings[keys];

                Keys modifiers = binding.ModifierKeys;

                //Remove any modifiers so we can 
                //get the exact key...
                key = keys & ~Keys.Shift;
                key = keys & ~Keys.Alt;
                key = keys & ~Keys.Control;

                binding.IsExecuting = ((GetKeyState((int)key) < 0) && ((ModifierKeys & modifiers) == modifiers));
            }
        }

        private void HandleMouseBindings()
        {
            foreach (MouseButtons button in _mouseBindings.Keys)
            {
                _mouseBindings[button].IsExecuting = ((MouseButtons & button) == button);
            }
        }

        private void OnMouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseWheel;

            if (handler != null)
                handler(sender, args);
        }

        private void OnMouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseUp;

            if (handler != null)
                handler(sender, args);
        }

        private void OnMouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseMove;

            if (handler != null)
                handler(sender, args);
        }

        private void OnMouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseDown;

            if (handler != null)
                handler(sender, args);
        }

        private void OnMouseDoubleClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseDoubleClick;

            if (handler != null)
                handler(sender, args);
        }

        private void OnMouseClick(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            HandledMouseEventArgs args = CreateArgs(e);

            var handler = MouseClick;

            if (handler != null)
                handler(sender, args);
        }

        private void OnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            HandledKeyEventArgs args = CreateArgs(e);

            var handler = KeyUp;

            if (handler != null)
                handler(sender, args);
        }

        private void OnKeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            HandledKeyEventArgs args = CreateArgs(e);

            var handler = KeyDown;

            if (handler != null)
                handler(sender, args);
        }

        private HandledMouseEventArgs CreateArgs(MouseEventArgs e)
        {
            return new HandledMouseEventArgs(e.Button, e.Clicks, e.X, e.Y, e.Delta, false);
        }

        private HandledKeyEventArgs CreateArgs(KeyEventArgs e)
        {
            return new HandledKeyEventArgs(e.KeyData);
        }
    }
}
