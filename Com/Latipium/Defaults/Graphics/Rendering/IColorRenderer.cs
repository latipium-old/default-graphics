// IColorRenderer.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;

namespace Com.Latipium.Defaults.Graphics.Rendering {
	/// <summary>
	/// Interface for a color renderer.
	/// </summary>
	public interface IColorRenderer : IRenderer {
		/// <summary>
		/// Starts rendering.
		/// </summary>
		/// <param name="args">The render arguments.</param>
		void Start(object args);

		/// <summary>
		/// Renders a vertex.
		/// </summary>
		/// <param name="vertex">The vertex number.</param>
		void Color(int vertex);
	}
}

