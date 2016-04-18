// ColorArrayRenderer.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;
using System.Drawing;
using OpenTK.Graphics.OpenGL;

namespace Com.Latipium.Defaults.Graphics.Rendering {
	internal class ColorArrayRenderer : IColorRenderer {
		private Color[] Colors;

		public bool CanRender(object arg) {
			return arg is Color[];
		}

		public void Start(object args) {
			Colors = (Color[]) args;
		}

		public void Color(int vertex) {
			GL.Color3(Colors[vertex]);
		}
	}
}

