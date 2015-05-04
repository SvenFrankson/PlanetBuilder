using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class SpeedCursor : MonoBehaviour {

		public SpaceShip spaceShip;
		private Transform cTransform;
		private float maxSpeed;
		private float cA = 0f;
		private float cB = 0f;

		void Start () {
			this.cTransform = this.GetComponent<Transform> ();
			if (this.spaceShip != null) {
				this.cA = (1f - 0.02f) / this.spaceShip.maxSpeed;
				this.cB = 0.02f;
			}
		}

		void Update () {
			if (this.spaceShip != null) {
				cTransform.localScale = (new Vector3 (0.2f, this.cA * this.spaceShip.Speed + this.cB, 1f));
			}
		}
	}
}