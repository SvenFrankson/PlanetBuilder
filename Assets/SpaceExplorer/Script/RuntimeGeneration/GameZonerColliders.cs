using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SvenFrankson.Game.SphereCraft;

namespace SvenFrankson.Game.SpaceExplorer {

	[RequireComponent(typeof(SphereCollider))]
	public class GameZonerColliders : MonoBehaviour {

		private SphereCollider cCollider;

		private List<PlanetChunck> pChuncks = new List<PlanetChunck> ();

		public void Start () {
			this.cCollider = this.GetComponent<SphereCollider> ();
		}

		void Update () {
			if (this.pChuncks.Count > 0) {
				MeshCollider mc = this.pChuncks [0].GetComponent<MeshCollider> ();
				if (mc == null) {
					Collider c = this.pChuncks[0].GetComponent<Collider> ();
					if (c != null) {
						DestroyImmediate (c);
					}
					mc = this.pChuncks [0].gameObject.AddComponent<MeshCollider> ();
				}
				
				mc.sharedMesh = this.pChuncks [0].meshCollider;
				
				this.pChuncks.RemoveAt (0);
			}
		}

		public void OnTriggerEnter (Collider c) {
			PlanetChunck pChunck = c.GetComponent<PlanetChunck> ();
			if (pChunck != null) {
				this.pChuncks.Add (pChunck);
			}
		}
	}
}