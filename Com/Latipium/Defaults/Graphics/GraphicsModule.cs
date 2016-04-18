// GraphicsModule.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Com.Latipium.Core;
using Com.Latipium.Defaults.Graphics.Rendering;

namespace Com.Latipium.Defaults.Graphics {
	/// <summary>
	/// The default module implementation for graphics.
	/// </summary>
	public class GraphicsModule : AbstractLatipiumModule, LatipiumLoader {
		private GameWindow Game;
		private GameRenderer Renderer;
		private KeyboardHandler Keyboard;
		private MouseHandler Mouse;
		private Viewport Viewport;
		private bool CursorVisibility;

		/// <summary>
		/// Occurs when the mouse is clicked on the screen.
		/// </summary>
		[LatipiumMethod("Clicked")]
		public event Action<int, int, bool> ClickActions;
		/// <summary>
		/// Occurs when the mouse moves on the screen.
		/// </summary>
		[LatipiumMethod("MouseMoved")]
		public event Action<int, int, int, int> MoveActions;
		/// <summary>
		/// Occurs when the mouse scrolls.
		/// </summary>
		[LatipiumMethod("Scrolled")]
		public event Action<int, int, float> ScrollActions;
		/// <summary>
		/// Occurs when it is safe to add or remove objects from the world.
		/// </summary>
		[LatipiumMethod("UpdateWorld")]
		public event Action Update;

		/// <summary>
		/// Gets or sets the field of view.
		/// </summary>
		/// <returns>The field of view.</returns>
		/// <param name="val">The new field of view.</param>
		[LatipiumMethod("FoV")]
		public double FoV(double val = 180) {
			if ( val < 180 ) {
				Viewport.FoV = val;
			}
			return Viewport.FoV;
		}

		/// <summary>
		/// Gets or sets the near z value.
		/// </summary>
		/// <returns>The near z value.</returns>
		/// <param name="val">The new near z value.</param>
		[LatipiumMethod("ZNear")]
		public double ZNear(double val = 0) {
			if ( val > 0 ) {
				Viewport.ZNear = val;
			}
			return Viewport.ZNear;
		}

		/// <summary>
		/// Gets or sets the far z value.
		/// </summary>
		/// <returns>The far z value.</returns>
		/// <param name="val">The new far z value.</param>
		[LatipiumMethod("ZFar")]
		public double ZFar(double val = 0) {
			if ( val > Viewport.ZNear ) {
				Viewport.ZFar = val;
			}
			return Viewport.ZFar;
		}

		/// <summary>
		/// Gets or sets whether or not to use vsync.
		/// </summary>
		/// <returns>Whether or not vsync is being used.</returns>
		/// <param name="val">0 if not to use vsync, or 1 to use vsync.</param>
		[LatipiumMethod("VSync")]
		public bool VSync(byte val = 2) {
			if ( val == 0 ) {
				Viewport.VSync = false;
			} else if ( val == 1 ) {
				Viewport.VSync = true;
			}
			return Viewport.VSync;
		}

		/// <summary>
		/// Loads a world.
		/// </summary>
		/// <param name="world">The world to load.</param>
		[LatipiumMethod("LoadWorld")]
		public void LoadWorld(LatipiumObject world) {
			Renderer.World = world;
		}

		/// <summary>
		/// Sets which player the user is.
		/// </summary>
		/// <param name="player">The player to use.</param>
		[LatipiumMethod("SetPlayer")]
		public void SetPlayer(LatipiumObject player) {
			Renderer.Player = player;
		}

		/// <summary>
		/// Adds a renderer.
		/// </summary>
		/// <param name="renderer">The renderer to add.</param>
		[LatipiumMethod("AddRenderer")]
		public void AddRenderer(IRenderer renderer) {
			Renderer.Add(renderer);
		}

		/// <summary>
		/// Adds a binding to the keyboard.
		/// </summary>
		/// <param name="del">The action to run.</param>
		/// <param name="mask">The key mask.</param>
		[LatipiumMethod("AddKeyboardBinding")]
		public void AddKeyboardBinding(Action<int, int> del, long mask = 0) {
			Keyboard.AddHandler(del, mask);
		}

		/// <summary>
		/// Removes a binding from the keyboard.
		/// </summary>
		/// <param name="del">The action to run.</param>
		/// <param name="mask">The key mask.</param>
		[LatipiumMethod("RemoveKeyboardBinding")]
		public void RemoveKeyboardBinding(Action<int, int> del = null, long mask = 0) {
			Keyboard.RemoveHandler(del, mask);
		}

		/// <summary>
		/// Converts client coordinates to screen coordinates.
		/// </summary>
		/// <returns>The screen coordinates.</returns>
		/// <param name="client">The client coordinates.</param>
		[LatipiumMethod("ClientToScreen")]
		public Point ClientToScreen(Point client) {
			return Game.PointToScreen(client);
		}

		/// <summary>
		/// Converts screen coordinates to client coordinates.
		/// </summary>
		/// <returns>The client coordinates.</returns>
		/// <param name="screen">The screen coordinates.</param>
		[LatipiumMethod("ScreenToClient")]
		public Point ScreenToClient(Point screen) {
			return Game.PointToClient(screen);
		}

		/// <summary>
		/// Enters fullscreen mode.
		/// </summary>
		[LatipiumMethod("EnterFullscreen")]
		public void EnterFullscreen() {
			Game.WindowState = WindowState.Fullscreen;
		}

		/// <summary>
		/// Shows the cursor.
		/// </summary>
		[LatipiumMethod("ShowCursor")]
		public void ShowCursor() {
			CursorVisibility = true;
			if ( Game != null ) {
				Game.Cursor = MouseCursor.Default;
			}
		}

		/// <summary>
		/// Hides the cursor.
		/// </summary>
		[LatipiumMethod("HideCursor")]
		public void HideCursor() {
			CursorVisibility = false;
			if ( Game != null ) {
				Game.Cursor = MouseCursor.Empty;
			}
		}

		/// <summary>
		/// Initializes the graphics module
		/// </summary>
		[LatipiumMethod("Initialize")]
		public void Init() {
			Game = new GameWindow();
			Game.Load += (object sender, EventArgs e) => {
				Keyboard.Game = Game;
				Mouse.Game = Game;
				Viewport = new Viewport(Game);
				Game.Title = "Latipium";
				Game.WindowState = WindowState.Maximized;
				Game.Cursor = CursorVisibility ? MouseCursor.Default : MouseCursor.Empty;
				GL.Enable(EnableCap.DepthTest);
			};
			Game.UpdateFrame += (object sender, FrameEventArgs e) => {
				if ( Update != null ) {
					Update();
				}
			};
			Game.RenderFrame += (object sender, FrameEventArgs e) => {
				GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
				Renderer.Render();
				Game.SwapBuffers();
			};
		}

		/// <summary>
		/// Runs the main rendering loop. This must be ran on the same thread as Initialize was called on.
		/// </summary>
		[LatipiumMethod("Loop")]
		public void Loop() {
			Game.Run(60);
		}

		/// <summary>
		/// Destroys the resources used by the graphics module
		/// </summary>
		[LatipiumMethod("Destroy")]
		public void Deinit() {
			ShowCursor();
			Game.Exit();
			Game.Close();
			Game.Dispose();
		}

		internal void Clicked(int x, int y, bool down) {
			if ( ClickActions != null ) {
				ClickActions(x, y, down);
			}
		}

		internal void Moved(int x, int y, int dx, int dy) {
			if ( MoveActions != null ) {
				MoveActions(x, y, dx, dy);
			}
		}

		internal void Scrolled(int x, int y, float val) {
			if ( ScrollActions != null ) {
				ScrollActions(x, y, val);
			}
		}

		private void LoadObject(LatipiumObject obj) {
			obj.InvokeProcedure<Action<IEnumerable<LatipiumObject>>>("Initialize", (IEnumerable<LatipiumObject> o) => {
				
			});
		}

		public override void Load(string name) {
			Renderer = new GameRenderer();
			Renderer.Add(new ColorArrayRenderer());
			Renderer.Add(new VertexArrayRenderer());
			Keyboard = new KeyboardHandler();
			Mouse = new MouseHandler(this);
			CursorVisibility = true;
			LatipiumModule objectModule = ModuleFactory.FindModule("Com.Latipium.Modules.World.Objects");
			if ( objectModule != null ) {
				objectModule.AddEvent("ObjectLoaded", (Action<LatipiumObject>) LoadObject);
			}
		}

		public void Load() {
			// Ensure this is loaded before any of the objects are added
			ModuleFactory.FindModule("Com.Latipium.Modules.Graphics");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Com.Latipium.Defaults.Graphics.GraphicsModule"/> class.
		/// </summary>
		public GraphicsModule() : base(new string[] { "Com.Latipium.Modules.Graphics" }) {
		}
	}
}

