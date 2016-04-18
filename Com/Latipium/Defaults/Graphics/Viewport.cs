// Viewport.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Com.Latipium.Defaults.Graphics {
	internal class Viewport {
		private GameWindow Game;
		private double _FoV;
		private double _ZNear;
		private double _ZFar;
		private bool _VSync;

		internal double FoV {
			get {
				return _FoV;
			}

			set {
				_FoV = value;
				Frustum();
			}
		}

		internal double ZNear {
			get {
				return _ZNear;
			}

			set {
				_ZNear = value;
				Frustum();
			}
		}

		internal double ZFar {
			get {
				return _ZFar;
			}

			set {
				_ZFar = value;
				Frustum();
			}
		}

		internal bool VSync {
			get {
				return _VSync;
			}

			set {
				_VSync = value;
				Game.VSync = value ? VSyncMode.On : VSyncMode.Off;
			}
		}

		private void Frustum() {
			double frustumHeight = Math.Tan(_FoV / 360 * Math.PI) * _ZNear;
			double frustumWidth = frustumHeight * Game.Width / Game.Height;
			GL.Frustum(-frustumWidth, frustumWidth, -frustumHeight, frustumHeight, _ZNear, _ZFar);
		}

		internal void Resize(object sender, EventArgs e) {
			GL.Viewport(0, 0, Game.Width, Game.Height);
			GL.MatrixMode(MatrixMode.Projection);
			GL.LoadIdentity();
			Frustum();
		}

		internal Viewport(GameWindow game) {
			Game = game;
			_FoV = 45;
			_ZNear = 0.01;
			_ZFar = 1000;
			Frustum();
			VSync = true;
			Game.Resize += Resize;
		}
	}
}

