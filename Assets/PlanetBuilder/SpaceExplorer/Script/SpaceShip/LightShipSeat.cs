using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class LightShipSeat : Interactable {

		public LightShip lightShip = null;
		public Transform lightShipSeat = null;

		public override void OnInteraction (Object actor) {
			if (this.lightShip != null) {
				if (this.lightShipSeat != null) {
					(actor as Humanoid).TakeControl (this.lightShip, this.lightShipSeat);
				}
			}
		}
	}
}