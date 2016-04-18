// GameRenderer.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using Com.Latipium.Core;
using Com.Latipium.Defaults.Graphics.Rendering;

namespace Com.Latipium.Defaults.Graphics {
	internal class GameRenderer {
		private readonly List<IColorRenderer> ColorRenderers;
		private readonly List<IObjectRenderer> ObjectRenderers;
		private LatipiumObject _World;
		private LatipiumObject _Player;
		private Func<IEnumerable<LatipiumObject>> GetObjects;
		private Func<Tuple<float, float, float>, Tuple<float, float, float>> PlayerPosition;
		private Func<float[], float[]> PlayerTransform;

		internal LatipiumObject World {
			get {
				return _World;
			}

			set {
				if ( value != null ) {
					_World = value;
					IEnumerable<LatipiumObject> realms = _World.InvokeFunction<IEnumerable<LatipiumObject>>("GetRealms");
					if ( realms != null ) {
						LatipiumObject realm = realms.First();
						if ( realm != null ) {
							GetObjects = realm.GetFunction<IEnumerable<LatipiumObject>>("GetObjects");
						}
					}
				}
			}
		}

		internal LatipiumObject Player {
			get {
				return _Player;
			}

			set {
				if ( value != null ) {
					_Player = value;
					PlayerPosition = _Player.GetFunction<Tuple<float, float, float>, Tuple<float, float, float>>("Position");
					PlayerTransform = _Player.GetFunction<float[], float[]>("Transform");
				}
			}
		}

		internal void Add(IRenderer renderer) {
			if ( renderer is IColorRenderer ) {
				ColorRenderers.Add((IColorRenderer) renderer);
			}
			if ( renderer is IObjectRenderer ) {
				ObjectRenderers.Add((IObjectRenderer) renderer);
			}
		}

		private void TriangleTest() {
			GL.Begin(PrimitiveType.Triangles);
			GL.Color3(Color.Red);
			GL.Vertex3(-1, 1, -5);
			GL.Color3(Color.Green);
			GL.Vertex3(0, -1, -5);
			GL.Color3(Color.Blue);
			GL.Vertex3(1, 1, -5);
			GL.End();
		}

		private void SetupCamera() {
			GL.MatrixMode(MatrixMode.Modelview);
			Matrix4 camera;
			Tuple<float, float, float> position = null;
			if ( PlayerPosition != null ) {
				position = PlayerPosition(null);
			}
			if ( position == null ) {
				camera = Matrix4.CreateTranslation(0, -5, -5);
			} else {
				camera = Matrix4.CreateTranslation(position.Object1, position.Object2, position.Object3);
			}
			float[] transform = null;
			if ( PlayerTransform != null ) {
				transform = PlayerTransform(null);
			}
			if ( transform == null || transform.Length != 16 ) {
				camera *= Matrix4.CreateRotationX((float) Math.PI / 4);
			} else {
				camera *= new Matrix4(
					transform[ 0], transform[ 1], transform[ 2], transform[ 3],
					transform[ 4], transform[ 5], transform[ 6], transform[ 7],
					transform[ 8], transform[ 9], transform[10], transform[11],
					transform[12], transform[13], transform[14], transform[15]
				);
			}
			GL.LoadMatrix(ref camera);
		}

		private void RenderObject(LatipiumObject obj, Tuple<object, object> data, IObjectRenderer objectR, IColorRenderer colorR) {
			Tuple<float, float, float> position = obj.InvokeFunction<Tuple<float, float, float>, Tuple<float, float, float>>("Position", null);
			float[] transform = obj.InvokeFunction<float[], float[]>("Transform", null);
			if ( position != null ) {
				GL.Translate(position.Object1, position.Object2, position.Object3);
			}
			if ( transform != null ) {
				GL.MultMatrix(transform);
			}
			int len = objectR.Start(data.Object1);
			if ( colorR == null ) {
				for ( int i = 0; i < len; ++i ) {
					objectR.Vertex(i);
				}
			} else {
				colorR.Start(data.Object2);
				for ( int i = 0; i < len; ++i ) {
					colorR.Color(i);
					objectR.Vertex(i);
				}
			}
			objectR.End();
		}

		private IObjectRenderer FindObjectRenderer(Tuple<object, object> data) {
			foreach ( IObjectRenderer obj in ObjectRenderers ) {
				if ( obj.CanRender(data.Object1) ) {
					return obj;
				}
			}
			return null;
		}

		private IColorRenderer FindColorRenderer(Tuple<object, object> data) {
			foreach ( IColorRenderer color in ColorRenderers ) {
				if ( color.CanRender(data.Object2) ) {
					return color;
				}
			}
			return null;
		}

		internal void Render() {
			if ( GetObjects == null ) {
				TriangleTest();
			} else {
				GL.PushMatrix();
				SetupCamera();
				foreach ( LatipiumObject obj in GetObjects() ) {
					Tuple<object, object> data = obj.InvokeFunction<LatipiumObject>("Type")
						.InvokeFunction<LatipiumObject, Tuple<object, object>>("GetRenderData", obj);
					if ( data != null ) {
						IObjectRenderer objectR = FindObjectRenderer(data);
						if ( objectR == null ) {
							continue;
						}
						IColorRenderer colorR = FindColorRenderer(data);
						GL.PushMatrix();
						RenderObject(obj, data, objectR, colorR);
						GL.PopMatrix();
					}
				}
				GL.PopMatrix();
			}
		}

		internal GameRenderer() {
			ColorRenderers = new List<IColorRenderer>();
			ObjectRenderers = new List<IObjectRenderer>();
		}
	}
}

