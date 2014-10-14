//	The MIT License (MIT)
//	
//	Copyright (c) 2014 SvenFrankson
//		
//		Permission is hereby granted, free of charge, to any person obtaining a copy
//		of this software and associated documentation files (the "Software"), to deal
//		in the Software without restriction, including without limitation the rights
//		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//		copies of the Software, and to permit persons to whom the Software is
//		furnished to do so, subject to the following conditions:
//		
//		The above copyright notice and this permission notice shall be included in all
//		copies or substantial portions of the Software.
//		
//		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//		SOFTWARE.

using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	[CustomEditor(typeof(Spline))]
	[CanEditMultipleObjects]
	public class SplineInspector : Editor {
		
		private Spline storedTarget;
		private Spline Target {
			get {
				if (this.storedTarget == null) {
					this.storedTarget = (Spline) target;
				}
				
				return storedTarget;
			}
		}
		
		public override void OnInspectorGUI () {
			base.OnInspectorGUI ();
			
			if (GUILayout.Button ("Link")) {
				this.LinkBoth ();
			}
			
			if (GUILayout.Button ("Break")) {
				Target.DestroyPath ();
			}
		}

		private void LinkBoth () {
			Object[] selection = Selection.objects;
			List<Spline> splines = new List<Spline> ();

			foreach (Object o in selection) {
				if (o.GetType () == typeof (GameObject)) {
					GameObject g = (GameObject) o;
					Spline s = g.GetComponent<Spline> ();
					if (s != null) {
						splines.Add (s);
					}
				}
			}

			if (splines.Count == 2) {
				splines[0].end = splines[1];
				splines[0].BuildSpline ();
				splines[0].PopOnSpline ();
				Selection.activeObject = splines[0];
			}
			else if (splines.Count == 1) {
				if (splines[0].end != null) {
					splines[0].BuildSpline ();
					splines[0].PopOnSpline ();
				}
			}
			else {
				Debug.LogWarning ("CityBuilder : Select exactly two Path objects to link them together");
			}
		}
	}
}
