using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {
	
	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Collider))]
	public class SpaceShipDoor : Interactable {
		
		private Animator cAnimator;

		void Start () {
			this.cAnimator = this.GetComponent<Animator> ();
		}
		
		public override void OnInteraction (Object actor) {
			bool open = this.cAnimator.GetBool ("Open");
			this.cAnimator.SetBool ("Open", !open);
		}
	}
}