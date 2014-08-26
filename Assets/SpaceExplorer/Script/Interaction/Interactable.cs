using UnityEngine;
using System.Collections;

namespace SvenFrankson.Game.SpaceExplorer {

	public abstract class Interactable : MonoBehaviour {

		public abstract void OnInteraction (Object actor);
	}
}