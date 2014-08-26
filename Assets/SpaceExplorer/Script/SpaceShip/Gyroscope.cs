using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class Gyroscope : MonoBehaviour {

		private Transform cTransform;

		void Start () {
			this.cTransform = this.GetComponent<Transform> ();
		}

		void Update () {
			this.cTransform.rotation = Quaternion.identity;
		}
	}
}
