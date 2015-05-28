using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	public class PlanetVegetationWindow : EditorWindow {
		
		static private int language = 0;
		
		static private Texture2D logo = null;
		static private Texture2D Logo {
			get {
				if (logo == null) {
					logo = Resources.LoadAssetAtPath<Texture2D> ("Assets/PlanetBuilder/Resources/icons/logoPlanetVegetationSmall.png");
					
				}
				return logo;
			}
		}
		
		static private Texture2D englishFlag = null;
		static private Texture2D EnglishFlag {
			get {
				if (englishFlag == null) {
					englishFlag = Resources.LoadAssetAtPath<Texture2D> ("Assets/PlanetBuilder/Resources/icons/logoEnglish.png");
					
				}
				return englishFlag;
			}
		}
		
		static private Texture2D frenchFlag = null;
		static private Texture2D FrenchFlag {
			get {
				if (frenchFlag == null) {
					frenchFlag = Resources.LoadAssetAtPath<Texture2D> ("Assets/PlanetBuilder/Resources/icons/logoFrench.png");
					
				}
				return frenchFlag;
			}
		}

		public Planet target;
		public int frequency = 2;

		public List<GameObject> vegetationPrefabs;
		public List<Texture2D> vegetationPreviews;
		public int[] frequencyOne;
		public int[] minHeight;
		public int[] maxHeight;

		public List<GameObject> [] vegetationPrefabsHeight;
		public List<float> [] vegetationRange;

		static public void Open (Planet planet, List<GameObject> prefabs, List<Texture2D> previews) {

			PlanetVegetationWindow win = EditorWindow.GetWindow<PlanetVegetationWindow> ();

			win.minSize = new Vector2 (300f, 600f);
			win.target = planet;
			win.vegetationPrefabs = prefabs;
			win.vegetationPreviews = previews;
			win.Initialize ();
		}

		public void Initialize () {
			this.frequencyOne = new int [this.vegetationPrefabs.Count];
			this.minHeight = new int [this.vegetationPrefabs.Count];
			this.maxHeight = new int [this.vegetationPrefabs.Count];

			for (int i = 0; i < this.vegetationPrefabs.Count; i++) {
				this.frequencyOne [i] = 50;
				this.minHeight [i] = 0;
				this.maxHeight [i] = 31;
			}
		}

		public void ComputeVegetationRange () {
			this.vegetationPrefabsHeight = new List<GameObject> [32];
			this.vegetationRange = new List<float> [32];

			for (int h = 0; h < 32; h++) {
				this.vegetationPrefabsHeight [h] = new List<GameObject> ();
				this.vegetationRange [h] = new List<float> ();

				int fSum = 0;

				for (int i = 0; i < this.vegetationPrefabs.Count; i++) {
					if (this.minHeight [i] <= h) {
						if (this.maxHeight [i] >= h) {
							fSum += this.frequencyOne [i];
						}
					}
				}

				float f = 0;

				for (int i = 0; i < this.vegetationPrefabs.Count; i++) {
					if (this.minHeight [i] <= h) {
						if (this.maxHeight [i] >= h) {
							f += (float) this.frequencyOne [i] / (float) fSum;
							this.vegetationPrefabsHeight [h].Add (this.vegetationPrefabs [i]);
							this.vegetationRange [h].Add (f);
						}
					}
				}
			}
		}

		public void PopulateWithVegetation () {
			foreach (PlanetSide pSide in this.target.planetSides) {
				for (int i = 0; i < pSide.nbChunks * PlanetUtility.ChunckSize; i++) {
					for (int j = 0; j < pSide.nbChunks * PlanetUtility.ChunckSize; j++) {
						bool doPopulate = (UnityEngine.Random.Range (0, 100) < this.frequency);

						if (doPopulate) {
							int h = pSide.GetKFor (i, j) + 1;
							if (h > 31) {
								h = 31;
							}

							float pickVegetation = UnityEngine.Random.Range (0f, 1f);
							bool done = false;
							for (int n = 0; (n < this.vegetationRange [h].Count) && (!done); n++) {
								if (pickVegetation < this.vegetationRange [h][n]) {
									GameObject prefab = this.vegetationPrefabsHeight [h][n];
									target.AddGameObjectAt (i, j, h, pSide, prefab);
									done = true;
								}
							}
						}
					}
				}
			}
		}

		private Vector2 scrollPos = Vector2.zero;

		public void OnGUI () {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("");
			GUILayout.Box (Logo);
			GUILayout.Label ("");
			GUILayout.EndHorizontal ();

			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ();
			GUILayout.Label ("");
			if (GUILayout.Button (EnglishFlag)) {
				language = 0;
			}
			GUILayout.Label ("");
			if (GUILayout.Button (FrenchFlag)) {
				language = 1;
			}
			GUILayout.Label ("");
			GUILayout.EndHorizontal ();
			EditorGUILayout.Space ();

			GUILayout.BeginHorizontal ("box");
			GUI.changed = false;
			GUILayout.Button ("?", GUILayout.ExpandWidth (false));
			if (GUI.changed) {
				GUI.changed = false;
				showFrequencyInfo = !showFrequencyInfo;
			}
			this.frequency = EditorGUILayout.IntSlider (frequencyLabel[language], this.frequency, 0, 100);
			GUILayout.EndHorizontal ();
			if (showFrequencyInfo) {
				EditorGUILayout.HelpBox (frequencyInfo[language], MessageType.Info);
			}
			
			if (GUILayout.Button ("Generate")) {
				this.ComputeVegetationRange ();
				this.PopulateWithVegetation ();
			}
			
			if (GUILayout.Button ("Set As Vegetation Data")) {
				this.target.vegetationData = new VegetationData (this.frequency, this.vegetationPrefabs.ToArray (), this.frequencyOne, this.minHeight, this.maxHeight);
			}
			
			if (GUILayout.Button ("Clear")) {
				this.target.ClearVegetation ();
			}

			this.scrollPos = EditorGUILayout.BeginScrollView (this.scrollPos);

			for (int i = 0; i < this.vegetationPrefabs.Count; i++) {
				EditorGUILayout.BeginHorizontal ("box");
				GUILayout.Button (this.vegetationPreviews [i], GUILayout.Height (80f), GUILayout.Width (80f));
				EditorGUILayout.BeginVertical ();
				
				if (i == 0) {
					GUILayout.BeginHorizontal ();
					GUI.changed = false;
					GUILayout.Button ("?", GUILayout.ExpandWidth (false));
					if (GUI.changed) {
						GUI.changed = false;
						showFrequencyOneInfo = !showFrequencyOneInfo;
					}
				}
				this.frequencyOne [i] = EditorGUILayout.IntSlider (frequencyOneLabel[language], this.frequencyOne [i], 0, 100);
				if (i == 0) {
					GUILayout.EndHorizontal ();
					if (showFrequencyOneInfo) {
						EditorGUILayout.HelpBox (frequencyOneInfo[language], MessageType.Info);
					}
				}

				if (i == 0) {
					GUILayout.BeginHorizontal ();
					GUI.changed = false;
					GUILayout.Button ("?", GUILayout.ExpandWidth (false));
					if (GUI.changed) {
						GUI.changed = false;
						showMinHeightInfo = !showMinHeightInfo;
					}
				}
				this.minHeight [i] = EditorGUILayout.IntSlider (minHeightLabel[language], this.minHeight [i], 0, 31);
				if (i == 0) {
					GUILayout.EndHorizontal ();
					if (showMinHeightInfo) {
						EditorGUILayout.HelpBox (minHeightInfo[language], MessageType.Info);
					}
				}
				
				if (i == 0) {
					GUILayout.BeginHorizontal ();
					GUI.changed = false;
					GUILayout.Button ("?", GUILayout.ExpandWidth (false));
					if (GUI.changed) {
						GUI.changed = false;
						showMaxHeightInfo = !showMaxHeightInfo;
					}
				}

				this.maxHeight [i] = EditorGUILayout.IntSlider (maxHeightLabel[language], this.maxHeight [i], 0, 31);
				if (i == 0) {
					GUILayout.EndHorizontal ();
					if (showMaxHeightInfo) {
						EditorGUILayout.HelpBox (maxHeightInfo[language], MessageType.Info);
					}
				}

				EditorGUILayout.EndVertical ();
				EditorGUILayout.EndHorizontal ();
			}

			EditorGUILayout.EndScrollView ();
		}

		#region Text
		
		static private bool showFrequencyInfo = false;
		static private string[] frequencyLabel = new string[2];
		static private string[] frequencyInfo = new string[2];
		
		static private bool showFrequencyOneInfo = false;
		static private string[] frequencyOneLabel = new string[2];
		static private string[] frequencyOneInfo = new string[2];
		
		static private bool showMinHeightInfo = false;
		static private string[] minHeightLabel = new string[2];
		static private string[] minHeightInfo = new string[2];
		
		static private bool showMaxHeightInfo = false;
		static private string[] maxHeightLabel = new string[2];
		static private string[] maxHeightInfo = new string[2];
		
		static PlanetVegetationWindow () {
			frequencyLabel [0] = "Frequency";
			frequencyLabel [1] = "Frequence";
			
			frequencyInfo [0] = "Frequency is a number determining the number of element added. 100 meaning that an element will be added on each block.";
			frequencyInfo [1] = "La \'Frequence\' est une grandeur qui détermine le nombre d'éléments qui seront ajoutés. 100 signifiant qu'un élément sera ajouté sur chaque bloc.";

			frequencyOneLabel [0] = "Frequency";
			frequencyOneLabel [1] = "Frequence";

			frequencyOneInfo [0] = "Frequency is a number determining the odd that the associated vegetation element is added and not another one, with no effect on the global number of elements added.";
			frequencyOneInfo [1] = "La \'Frequence\' est une grandeur qui détermine la probabilité que l'élément de végétation associé soit ajouté plutot qu'un autre, sans avoir d'effet sur le nombre global d'éléments ajoutés.";
		
			minHeightLabel [0] = "Min Height";
			minHeightLabel [1] = "Altitude Minimale";

			minHeightInfo [0] = "Minimal height at which associated vegetation element can be added.";
			minHeightInfo [1] = "Altitude minimale à laquelle l'élement de végétation associé peut etre ajouté.";
			
			maxHeightLabel [0] = "Max Height";
			maxHeightLabel [1] = "Altitude Maximale";
			
			maxHeightInfo [0] = "Maximal height at which associated vegetation element can be added.";
			maxHeightInfo [1] = "Altitude maximale à laquelle l'élement de végétation associé peut etre ajouté.";
		}
		
		#endregion
	}
}
