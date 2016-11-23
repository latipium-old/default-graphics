// IRenderer.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;

namespace Com.Latipium.Defaults.Graphics.Rendering {
	/// <summary>
	/// Interface for a renderer interface.
	/// </summary>
	public interface IRenderer {
		/// <summary>
		/// Determines whether this instance can render with the specified arguments.
		/// </summary>
		/// <returns><c>true</c> if this instance can render with the specified arguments; otherwise, <c>false</c>.</returns>
		/// <param name="arg">The render arguments.</param>
		bool CanRender(object arg);
	}
}

