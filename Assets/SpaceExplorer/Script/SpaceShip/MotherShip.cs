using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class MotherShip : SpaceShip {

		public override void Start () {
			base.Start ();
			this.cTransform = this.GetComponent<Transform> ();
		}

		public void Update () {
			this.speed -= this.speed * 0.33f * Time.deltaTime;
			this.roll -= this.roll * 0.33f * Time.deltaTime;
			this.pitch -= this.pitch * 0.33f * Time.deltaTime;

			if (this.engineOn) {
				this.speed += this.enginePower * Time.deltaTime;
			}

			if (this.rollOn != 0) {
				this.roll += this.rollOn * this.rollPower * Time.deltaTime;
			}
			
			if (this.pitchOn != 0) {
				this.pitch += this.pitchOn * this.rollPower * Time.deltaTime;
			}

			this.cTransform.position += this.cTransform.forward * this.speed * Time.deltaTime;
			this.cTransform.localRotation = Quaternion.AngleAxis (this.roll * Time.deltaTime, this.cTransform.forward) * this.cTransform.localRotation;
			this.cTransform.localRotation = Quaternion.AngleAxis (this.pitch * Time.deltaTime, this.cTransform.right) * this.cTransform.localRotation;
		}

		public Vector3 GetVelocity () {
			return this.cTransform.forward * this.speed;
		}
	}
}