// KeyboardHandler.cs
//
// Copyright (c) 2016 Zach Deibert.
// All Rights Reserved.
using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Input;
using Com.Latipium.Core;

namespace Com.Latipium.Defaults.Graphics {
	internal class KeyboardHandler {
		private readonly List<Tuple<long, Action<int, int>>> Actions;
		private GameWindow _Game;

		internal GameWindow Game {
			get {
				return _Game;
			}

			set {
				_Game = value;
				_Game.KeyDown += KeyDown;
				_Game.KeyPress += KeyPress;
				_Game.KeyUp += KeyUp;
			}
		}

		private void ExtractKey(KeyboardKeyEventArgs e, int type, out int key, out int mods) {
			key = (int) e.ScanCode;
			// Mods:
			// 00000000 00000000 00000TTT 0000SRCA
			mods = (e.Alt ? 0x01 : 0) |
				(e.Control ? 0x02 : 0) |
				(e.IsRepeat ? 0x04 : 0) |
				(e.Shift ? 0x08 : 0) |
				(type << 8);
		}

		private void ExtractKey(KeyPressEventArgs e, int type, out int key, out int mods) {
			key = e.KeyChar;
			mods = (type << 8);
		}

		private void DispatchEvent(int key, int mods) {
			long mask = (long) ((((ulong) key) << 32) | (ulong) mods);
			foreach ( Tuple<long, Action<int, int>> tuple in Actions ) {
				if ( (tuple.Object1 & mask) == tuple.Object1 ) {
					tuple.Object2(key, mods);
				}
			}
		}

		internal void AddHandler(Action<int, int> action, long mask) {
			Actions.Add(new Tuple<long, Action<int, int>>(mask, action));
		}

		internal void RemoveHandler(Action<int, int> action, long mask) {
			if ( action == null ) {
				Actions.RemoveAll(tuple => (tuple.Object1 & mask) == mask);
			} else {
				Actions.RemoveAll(tuple => tuple.Object2 == action && (tuple.Object1 & mask) == mask);
			}
		}

		private void KeyDown(object sender, KeyboardKeyEventArgs e) {
			int key;
			int mods;
			ExtractKey(e, 0x01, out key, out mods);
			DispatchEvent(key, mods);
		}

		private void KeyPress(object sender, KeyPressEventArgs e) {
			int key;
			int mods;
			ExtractKey(e, 0x04, out key, out mods);
			DispatchEvent(key, mods);
		}

		private void KeyUp(object sender, KeyboardKeyEventArgs e) {
			int key;
			int mods;
			ExtractKey(e, 0x02, out key, out mods);
			DispatchEvent(key, mods);
		}

		internal KeyboardHandler() {
			Actions = new List<Tuple<long, Action<int, int>>>();
		}
	}
}

