using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	[RequireComponent(typeof(Animator))]
	[RequireComponent(typeof(Collider))]
	public class HoloScreen : Interactable {

		private Animator cAnimator;

		void Start () {
			this.cAnimator = this.GetComponent<Animator> ();
		}

		public override void OnInteraction (Object actor) {
			bool ath = this.cAnimator.GetBool ("ATH");
			this.cAnimator.SetBool ("ATH", !ath);
		}
	}
}
