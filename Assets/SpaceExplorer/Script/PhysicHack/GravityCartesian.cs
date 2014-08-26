using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class GravityCartesian : Gravity {

		public override GravityType GType {
			get {
				return GravityType.Cartesian;
			}
		}
	}
}
