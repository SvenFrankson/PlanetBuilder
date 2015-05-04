using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {
	
	public class PlanetUtility {

		static public int ChunckSize {
			get {
				return 32;
			}
		}

		static private PlanetBaseMesh[] meshes;

		static public PlanetBaseMesh GetBaseMesh (int degree) {
			if (degree < 3) {
				Debug.LogWarning ("Querying BaseMesh with Degree of " + degree + " failed. Degree has to be in [3, 7]");
			}

			if (PlanetUtility.meshes == null) {
				PlanetUtility.meshes = new PlanetBaseMesh[10];
				for (int i = 0; i < PlanetUtility.meshes.Length; i++) {
					PlanetUtility.meshes [i] = null;
				}
			}

			if (PlanetUtility.meshes [degree] == null) {
				PlanetUtility.meshes [degree] = new PlanetBaseMesh (degree);
			}

			return PlanetUtility.meshes [degree];
		}

		static public PlanetChunck InstantiatePlanetChunck (Vector3 orientation, Vector3 posInChunck, PlanetSide planetSide) {
			GameObject planetChunckInstance = new GameObject ();

			PlanetChunck planetChunck = planetChunckInstance.AddComponent<PlanetChunck> ();
			planetChunck.Initialize (orientation, posInChunck, planetSide);

			planetChunckInstance.AddComponent<MeshFilter> ();
			MeshRenderer planetChunckMeshRenderer = planetChunckInstance.AddComponent<MeshRenderer> ();
			planetChunckMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			planetChunckMeshRenderer.receiveShadows = true;

			// Adding MeshCollider at runtime gives much better performances.
			//planetChunckInstance.AddComponent<MeshCollider> ();

			return planetChunck;
		}
	}
}