using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class Satellite : MonoBehaviour {

		public Transform origin;
		public float rotationPeriod;
		public float revolutionPeriod;

		private float angularRotationSpeed;
		private float angularRevolutionSpeed;

		private Transform cTransform;

		void Start () {
			this.cTransform = this.GetComponent<Transform> ();
			this.angularRotationSpeed = 360f / this.rotationPeriod;
			this.angularRevolutionSpeed = 360f / this.revolutionPeriod;
		}

		void Update () {
			this.cTransform.RotateAround (origin.position, origin.transform.up, angularRevolutionSpeed * Time.deltaTime);
			this.cTransform.Rotate (this.cTransform.up, angularRotationSpeed * Time.deltaTime);
		}
	}
}
