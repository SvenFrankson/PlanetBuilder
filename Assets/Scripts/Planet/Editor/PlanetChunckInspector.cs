using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	[CustomEditor (typeof (PlanetChunck))]
	public class PlanetChunckInspector : Editor {
		
		private PlanetChunck storedTarget;
		private PlanetChunck Target {
			get {
				if (this.storedTarget == null) {
					this.storedTarget = (PlanetChunck) target;
				}
				
				return storedTarget;
			}
		}
		
		public override void OnInspectorGUI () {
			if (GUILayout.Button ("Build")) {
				double t1 = EditorApplication.timeSinceStartup;
				Target.SetMesh ();
				double t2 = EditorApplication.timeSinceStartup;
				Debug.Log ("Time for build = " + (t2 - t1));
			}
		}
	}
}