using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	public class PlanetLoader : MonoBehaviour {

		public List<Planet> planets;
		public List<PlanetChunck> chuncks;

		void Start () {
			this.FindAllChuncks ();
		}

		public void OnGUI () {
			GUI.TextArea (new Rect (10, 10, 100, 25), this.chuncks.Count.ToString ());
		}

		public void Update () {
			if (this.chuncks.Count > 0) {
				MeshCollider mc = this.chuncks [0].gameObject.GetComponent<MeshCollider> ();
				if (mc == null) {
					mc = this.chuncks [0].gameObject.AddComponent<MeshCollider> ();
				}

				mc.sharedMesh = this.chuncks [0].meshCollider;

				this.chuncks.RemoveAt (0);
			}
		}

		public void FindAllChuncks () {
			this.chuncks = new List<PlanetChunck> (GameObject.FindObjectsOfType<PlanetChunck> ());
		}
	}
}
