// MouseHandler.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Input;
using Com.Latipium.Core;

namespace Com.Latipium.Defaults.Graphics {
	internal class MouseHandler {
		private GameWindow _Game;
		private GraphicsModule Module;

		internal GameWindow Game {
			get {
				return _Game;
			}

			set {
				_Game = value;
				_Game.MouseDown += MouseClick;
				_Game.MouseUp += MouseUnclick;
				_Game.MouseMove += Register;
				_Game.MouseWheel += MouseScroll;
			}
		}

		private void Register(object sender, MouseMoveEventArgs e) {
			// Ignore the first movement by the mouse, because dx and dy will be wrong
			_Game.MouseMove -= Register;
			_Game.MouseMove += MouseMove;
		}

		private void MouseClick(object sender, MouseButtonEventArgs e) {
			Module.Clicked(e.X, e.Y, true);
		}

		private void MouseUnclick(object sender, MouseButtonEventArgs e) {
			Module.Clicked(e.X, e.Y, false);
		}

		private void MouseMove(object sender, MouseMoveEventArgs e) {
			Module.Moved(e.X, e.Y, e.XDelta, e.YDelta);
		}

		private void MouseScroll(object sender, MouseWheelEventArgs e) {
			Module.Scrolled(e.X, e.Y, e.ValuePrecise);
		}

		internal MouseHandler(GraphicsModule mod) {
			Module = mod;
		}
	}
}

