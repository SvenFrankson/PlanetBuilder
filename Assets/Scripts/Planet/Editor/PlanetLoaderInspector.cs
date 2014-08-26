using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SvenFrankson.Game.SphereCraft {

	[CustomEditor (typeof(PlanetLoader))]
	public class PlanetLoaderInspector : Editor {

		private PlanetLoader storedTarget;
		private PlanetLoader Target {
			get {
				if (this.storedTarget == null) {
					this.storedTarget = (PlanetLoader) target;
				}
				
				return storedTarget;
			}
		}

		public override void OnInspectorGUI () {
			if (GUILayout.Button ("Find Chuncks")) {
				Target.FindAllChuncks ();
			}
		}
	}
}
