// VertexArrayRenderer.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;
using OpenTK.Graphics.OpenGL;

namespace Com.Latipium.Defaults.Graphics.Rendering {
	internal class VertexArrayRenderer : IObjectRenderer {
		private float[] Verticies;

		public bool CanRender(object arg) {
			return arg is float[];
		}

		public int Start(object args) {
			Verticies = (float[]) args;
			GL.Begin(PrimitiveType.Triangles);
			return Verticies.Length / 3;
		}

		public void End() {
			GL.End();
		}

		public void Vertex(int vertex) {
			GL.Vertex3(Verticies[3 * vertex], Verticies[3 * vertex + 1], Verticies[3 * vertex + 2]);
		}
	}
}

