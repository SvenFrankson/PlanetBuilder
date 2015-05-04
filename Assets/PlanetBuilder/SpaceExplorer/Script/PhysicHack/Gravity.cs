using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public abstract class Gravity : MonoBehaviour {
		
		public enum GravityType {
			Cartesian,
			Spherical
		}

		public float gravity = 10f;

		public abstract GravityType GType {
			get;
		}
	}
}
