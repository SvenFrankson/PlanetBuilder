using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public class SpaceShipSeat : Interactable {

		public SpaceShip spaceShip = null;
		public Transform spaceShipSeat = null;

		public override void OnInteraction (Object actor) {
			if (this.spaceShip != null) {
				if (this.spaceShipSeat != null) {
					(actor as Humanoid).TakeControl (this.spaceShip, this.spaceShipSeat);
				}
			}
		}
	}
}