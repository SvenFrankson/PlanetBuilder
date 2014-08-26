using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	[System.Serializable]
	public class VegetationData {

		public int frequency;
		public GameObject[] prefabs;
		public int[] frequencyOne;
		public int[] minHeight;
		public int[] maxHeight;
		
		public List<GameObject> [] vegetationPrefabsHeight;
		public List<float> [] vegetationRange;

		public VegetationData (int frequency, GameObject[] prefabs, int[] frequencyOne, int[] minHeight, int[] maxHeight) {
			this.frequency = frequency;
			this.prefabs = prefabs;
			this.frequencyOne = frequencyOne;
			this.minHeight = minHeight;
			this.maxHeight = maxHeight;
		}

		public void ComputeVegetationRange () {
			this.vegetationPrefabsHeight = new List<GameObject> [32];
			this.vegetationRange = new List<float> [32];
			
			for (int h = 0; h < 32; h++) {
				this.vegetationPrefabsHeight [h] = new List<GameObject> ();
				this.vegetationRange [h] = new List<float> ();
				
				int fSum = 0;
				
				for (int i = 0; i < this.prefabs.Length; i++) {
					if (this.minHeight [i] <= h) {
						if (this.maxHeight [i] >= h) {
							fSum += this.frequencyOne [i];
						}
					}
				}
				
				float f = 0;
				
				for (int i = 0; i < this.prefabs.Length; i++) {
					if (this.minHeight [i] <= h) {
						if (this.maxHeight [i] >= h) {
							f += (float) this.frequencyOne [i] / (float) fSum;
							this.vegetationPrefabsHeight [h].Add (this.prefabs [i]);
							this.vegetationRange [h].Add (f);
						}
					}
				}
			}
		}
	}
}
