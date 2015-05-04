using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public abstract class SpaceShip : MonoBehaviour {

		[HideInInspector]
		public Transform cTransform;
		
		public float speed = 0f;
		public float Speed {
			get {
				return this.speed;
			}
		}

		public bool engineOn = false;
		public float enginePower;
		public float maxSpeed;

		public float roll = 0f;
		public int rollOn = 0;
		public float rollPower;
		
		public float pitch = 0f;
		public int pitchOn = 0;
		public float pitchPower;

		public virtual void Start () {
			this.cTransform = this.GetComponent<Transform> ();
		}
	}
}