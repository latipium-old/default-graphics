// IObjectRenderer.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;

namespace Com.Latipium.Defaults.Graphics.Rendering {
	/// <summary>
	/// Interface for an object renderer.
	/// </summary>
	public interface IObjectRenderer : IRenderer {
		/// <summary>
		/// Starts rendering.
		/// </summary>
		/// <returns>The number of verticies</returns>
		/// <param name="args">The render arguments.</param>
		int Start(object args);

		/// <summary>
		/// Finishes rendering.
		/// </summary>
		void End();

		/// <summary>
		/// Renders a vertex.
		/// </summary>
		/// <param name="vertex">The vertex number.</param>
		void Vertex(int vertex);
	}
}

