using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class Altitude : MonoBehaviour {

		private TextMesh cTextMesh;
		private Transform cSpaceShipTransform;
		public SpaceShip spaceShip;

		void Start () {
			this.cTextMesh = this.GetComponent<TextMesh> ();
			if (this.spaceShip != null) {
				this.cSpaceShipTransform = this.spaceShip.GetComponent<Transform> ();
			}
		}

		void Update () {
			this.cTextMesh.text = Mathf.FloorToInt(cSpaceShipTransform.localPosition.y) + " m";
		}
	}
}
