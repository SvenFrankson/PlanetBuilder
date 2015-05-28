using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {
	
	[CustomEditor (typeof (Planet))]
	public class PlanetInspector : Editor {
		
		static private int language = 0;
		
		static private Texture2D logo = null;
		static private Texture2D Logo {
			get {
				if (logo == null) {
					logo = Resources.LoadAssetAtPath<Texture2D> ("Assets/PlanetBuilder/Resources/icons/logoPlanetBuilderSmall.png");
					
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
		
		private Planet storedTarget;
		private Planet Target {
			get {
				if (this.storedTarget == null) {
					this.storedTarget = (Planet) target;
				}
				
				return storedTarget;
			}
		}
		
		public void OnEnable () {
			Target.SearchPlanetSide ();
		}
		
		public override void OnInspectorGUI () {
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("");
			GUILayout.Box (Logo);
			GUILayout.Label ("");
			GUILayout.EndHorizontal ();
			
			if (!(PrefabUtility.GetPrefabType (Target) == PrefabType.Prefab)) {
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
					showSizeInfo = !showSizeInfo;
				}
				Target.subDegree = EditorGUILayout.IntSlider (sizeLabel[language], Target.subDegree, 3, 9);
				GUILayout.EndHorizontal ();
				if (showSizeInfo) {
					EditorGUILayout.HelpBox (sizeInfo[language], MessageType.Info);
				}
				if (Target.subDegree >= 7) {
					EditorGUILayout.HelpBox (sizeWarning[language], MessageType.Warning);
				}
				
				GUILayout.BeginHorizontal ("box");
				GUI.changed = false;
				GUILayout.Button ("?", GUILayout.ExpandWidth (false));
				if (GUI.changed) {
					GUI.changed = false;
					showHeightMapInfo = !showHeightMapInfo;
				}
				Target.heightMap = EditorGUILayout.ObjectField (heightMapLabel[language], Target.heightMap, typeof(Cubemap), true) as Cubemap;
				GUILayout.EndHorizontal ();
				if (showHeightMapInfo) {
					EditorGUILayout.HelpBox (heightMapInfo[language], MessageType.Info);
				}
				
				GUILayout.BeginHorizontal ("box");
				GUI.changed = false;
				GUILayout.Button ("?", GUILayout.ExpandWidth (false));
				if (GUI.changed) {
					GUI.changed = false;
					showHeightCoefInfo = !showHeightCoefInfo;
				}
				Target.heightCoef = EditorGUILayout.IntSlider (heightCoefLabel[language], Target.heightCoef, 10, 100);
				GUILayout.EndHorizontal ();
				if (showHeightCoefInfo) {
					EditorGUILayout.HelpBox (heightCoefInfo[language], MessageType.Info);
				}
				
				GUILayout.BeginHorizontal ("box");
				GUI.changed = false;
				GUILayout.Button ("?", GUILayout.ExpandWidth (false));
				if (GUI.changed) {
					GUI.changed = false;
					showHoleMapInfo = !showHoleMapInfo;
				}
				Target.holeMap = EditorGUILayout.ObjectField (holeMapLabel[language], Target.holeMap, typeof(Cubemap), true) as Cubemap;
				GUILayout.EndHorizontal ();
				if (showHoleMapInfo) {
					EditorGUILayout.HelpBox (holeMapInfo[language], MessageType.Info);
				}
				
				GUILayout.BeginHorizontal ("box");
				GUI.changed = false;
				GUILayout.Button ("?", GUILayout.ExpandWidth (false));
				if (GUI.changed) {
					GUI.changed = false;
					showHoleThresholdInfo = !showHoleThresholdInfo;
				}
				Target.holeThreshold = EditorGUILayout.IntSlider (holeThresholdLabel[language], Target.holeThreshold, 0, 255);
				GUILayout.EndHorizontal ();
				if (showHoleThresholdInfo) {
					EditorGUILayout.HelpBox (holeThresholdInfo[language], MessageType.Info);
				}
				
				GUILayout.BeginHorizontal ("box");
				GUI.changed = false;
				GUILayout.Button ("?", GUILayout.ExpandWidth (false));
				if (GUI.changed) {
					GUI.changed = false;
					showWaterLevelInfo = !showWaterLevelInfo;
				}
				Target.waterLevel = EditorGUILayout.IntSlider (waterLevelLabel[language], Target.waterLevel, 0, 32);
				GUILayout.EndVertical ();
				if (showWaterLevelInfo) {
					EditorGUILayout.HelpBox (waterLevelInfo[language], MessageType.Info);
				}
				
				GUILayout.BeginHorizontal ("box");
				GUI.changed = false;
				GUILayout.Button ("?", GUILayout.ExpandWidth (false));
				if (GUI.changed) {
					GUI.changed = false;
					showDirtThicknessInfo = !showDirtThicknessInfo;
				}
				Target.dirtThickness = EditorGUILayout.IntSlider (dirtThicknessLabel[language], Target.dirtThickness, 0, 32);
				GUILayout.EndVertical ();
				if (showDirtThicknessInfo) {
					EditorGUILayout.HelpBox (dirtThicknessInfo[language], MessageType.Info);
				}
				
				GUILayout.BeginHorizontal ("box");
				GUI.changed = false;
				GUILayout.Button ("?", GUILayout.ExpandWidth (false));
				if (GUI.changed) {
					GUI.changed = false;
					showDirtMinInfo = !showDirtMinInfo;
				}
				Target.dirtMin = EditorGUILayout.IntSlider (dirtMinLabel[language], Target.dirtMin, 0, 32);
				GUILayout.EndVertical ();
				if (showDirtMinInfo) {
					EditorGUILayout.HelpBox (dirtMinInfo[language], MessageType.Info);
				}
				
				GUILayout.BeginHorizontal ("box");
				GUI.changed = false;
				GUILayout.Button ("?", GUILayout.ExpandWidth (false));
				if (GUI.changed) {
					GUI.changed = false;
					showDirtMaxInfo = !showDirtMaxInfo;
				}
				Target.dirtMax = EditorGUILayout.IntSlider (dirtMaxLabel[language], Target.dirtMax, 0, 32);
				GUILayout.EndVertical ();
				if (showDirtMaxInfo) {
					EditorGUILayout.HelpBox (dirtMaxInfo[language], MessageType.Info);
				}
				
				GUILayout.BeginVertical ("box");
				EditorGUILayout.BeginHorizontal ();
				GUI.changed = false;
				GUILayout.Button ("?", GUILayout.ExpandWidth (false));
				if (GUI.changed) {
					GUI.changed = false;
					showPlanetMaterialsInfo = !showPlanetMaterialsInfo;
				}
				EditorGUILayout.LabelField (planetMaterialsLabelTitle [language]);
				EditorGUILayout.EndHorizontal ();
				for (int i = 0; i < 2; i ++) {
					Target.planetMaterials [i] = EditorGUILayout.ObjectField (planetMaterialsLabel [language][i], Target.planetMaterials [i], typeof(Material), true) as Material;
				}
				GUILayout.EndVertical ();
				if (showPlanetMaterialsInfo) {
					EditorGUILayout.HelpBox (planetMaterialsInfo[language], MessageType.Info);
				}
				
				if (PrefabUtility.GetPrefabType (Target) == PrefabType.PrefabInstance) {
					EditorGUILayout.HelpBox (prefabWarning [language], MessageType.Warning);
				}

				if (Target.vegetationData != null) {
					// Display something about vegetation
				}
				
				if (GUILayout.Button ("Generate")) {
					Target.BuildPlanet ();
				}
				
				if (GUILayout.Button ("Build Colliders")) {
					Target.BuildColliders ();
				}
				
				if (GUILayout.Button ("Build Lite Colliders")) {
					Target.BuildLiteColliders ();
				}
				
				if (GUILayout.Button ("Remove Colliders")) {
					Target.DestroyColliders ();
				}
				
				if (GUILayout.Button ("Clear Vegetation")) {
					Target.ClearVegetation ();
				}
			}
		}
		
		#region Text
		
		static private bool showSizeInfo = false;
		static private string[] sizeLabel = new string[2];
		static private string[] sizeWarning = new string[2];
		static private string[] sizeInfo = new string[2];
		
		static private bool showHeightMapInfo = false;
		static private string[] heightMapLabel = new string[2];
		static private string[] heightMapInfo = new string[2];
		
		static private bool showHeightCoefInfo = false;
		static private string[] heightCoefLabel = new string[2];
		static private string[] heightCoefInfo = new string[2];
		
		static private bool showHoleMapInfo = false;
		static private string[] holeMapLabel = new string[2];
		static private string[] holeMapInfo = new string[2];
		
		static private bool showHoleThresholdInfo = false;
		static private string[] holeThresholdLabel = new string[2];
		static private string[] holeThresholdInfo = new string[2];
		
		static private bool showWaterLevelInfo = false;
		static private string[] waterLevelLabel = new string[2];
		static private string[] waterLevelInfo = new string[2];
		
		static private bool showDirtThicknessInfo = false;
		static private string[] dirtThicknessLabel = new string[2];
		static private string[] dirtThicknessInfo = new string[2];
		
		static private bool showDirtMinInfo = false;
		static private string[] dirtMinLabel = new string[2];
		static private string[] dirtMinInfo = new string[2];
		
		static private bool showDirtMaxInfo = false;
		static private string[] dirtMaxLabel = new string[2];
		static private string[] dirtMaxInfo = new string[2];
		
		static private bool showPlanetMaterialsInfo = false;
		static private string[] planetMaterialsLabelTitle = new string[2];
		static private string[] planetMaterialsInfo = new string[2];
		static private string[][] planetMaterialsLabel = new string[2][];
		
		static private string[] prefabWarning = new string[2];
		
		static PlanetInspector () {
			sizeLabel [0] = "Size";
			sizeLabel [1] = "Taille";
			
			sizeWarning [0] = "Creating a planet with size 7 will instantiate more than 1500 GameObject. It could take a while, and even crash Unity Editor on some computers. Saving scene before trying to generate this planet would be wise.";
			sizeWarning [1] = "Créer une planète de taille 7 instancie plus de 1500 GameObject. L'opération peut prendre du temps, voire provoquer la fermeture de Unity Editor sur certains ordinateurs. Il est conseillé de sauvegarder la scène avant de tenter de générer la planète.";
			
			sizeInfo [0] = "A planet is made of 6 squares wrapped on a sphere. Each square is a grid with a side of 2 power \'Size\'.";
			sizeInfo [1] = "Une planète est consituée de 6 carrés étirés sur une sphère. Chaque carré est une grille qui a pour coté 2 puissance \'Taille\'.";
			
			heightMapLabel [0] = "HeightMap";
			heightMapLabel [1] = "Carte des Reliefs";
			
			heightMapInfo [0] = "HeightMap is a seamless grey-leveled CubeMap representing the height of the Planet for a given point of the surface (where black represents the lowest point). A nice way of obtaining such a CubeMap is by using 3D Perlin noise. Unity editor extension \'CubeMap Generator\' helps you generate easily this kind of CubeMap.";
			heightMapInfo [1] = "La Carte des Reliefs est une CubeMap en niveaux de gris sans discontinuités, qui représente la hauteur de la planete pour un point de sa surface (où le noir représente le point le plus bas atteignable). Une bonne solution pour obtenir une telle CubeMap est l'utilisation du bruit de Perlin. L'extension editeur Unity \'CubeMap Generator\' permet de générer facilement une telle CubeMap.";
			
			heightCoefLabel [0] = "Height coefficient";
			heightCoefLabel [1] = "Coefficient de Relief";
			
			heightCoefInfo [0] = "Height coefficient allows you to raise or lower the global height of the Planet. Default value of 32 is the height for a pure white (#ffffff) pixel on the HeightMap. HeightCoef above 32 may cause some moutains to be flat with CubeMap already sending high values.";
			heightCoefInfo [1] = "Le Coefficient de Relief permet d'augmenter ou de diminuer la hauteur globale de la planete. La valeur par défaut de 32 est la hauteur pour un pixel blanc pur (#ffffff) sur la Carte des Reliefs. Un Coefficient de Hauteur supérieur à 32 pourra rendre plats certains sommets si la Carte des Reliefs retourne déjà des valeurs élevées.";
			
			holeMapLabel [0] = "HoleMap";
			holeMapLabel [1] = "Carte des Creux";
			
			holeMapInfo [0] = "Not implemented yet. Should be a map representing holes and caves in the ground.";
			holeMapInfo [1] = "Non implémentée actuellement. Devrait etre une carte représentant les creux et caves dans le sol.";
			
			holeThresholdLabel [0] = "Hole Threshold";
			holeThresholdLabel [1] = "Seuil de Creux";
			
			holeThresholdInfo [0] = "Not implemented yet.";
			holeThresholdInfo [1] = "Non implémentée actuellement.";
			
			waterLevelLabel [0] = "Water Level";
			waterLevelLabel [1] = "Niveau de la Mer";
			
			waterLevelInfo [0] = "Sets the height of water from level 0 (in blocks).";
			waterLevelInfo [1] = "Determine la hauteur de l'eau depuis le niveau 0 (en blocs).";
			
			dirtThicknessLabel [0] = "Dirt Thickness";
			dirtThicknessLabel [1] = "Epaisseur de la Terre";
			
			dirtThicknessInfo [0] = "Sets the thickness of dirt above rocks (in blocks).";
			dirtThicknessInfo [1] = "Determine l'épaisseur de terre au dessus des roches (en blocs).";
			
			dirtMinLabel [0] = "Dirt Minimal Height";
			dirtMinLabel [1] = "Altitude Minimale de la Terre";
			
			dirtMinInfo [0] = "Sets the height under which no dirt will be generated (in blocks).";
			dirtMinInfo [1] = "Determine la hauteur en dessous de laquelle aucun bloc de terre ne sera généré (en blocs).";
			
			dirtMaxLabel [0] = "Dirt Maximal Height";
			dirtMaxLabel [1] = "Altitude Maximale de la Terre";
			
			dirtMaxInfo [0] = "Sets the height above which no dirt will be generated (in blocks).";
			dirtMaxInfo [1] = "Determine la hauteur au dessus de laquelle aucun bloc de terre ne sera généré (en blocs).";
			
			planetMaterialsLabelTitle [0] = "Planet Materials";
			planetMaterialsLabelTitle [1] = "Textures des Blocs";
			
			planetMaterialsInfo [0] = "Planet Materials are the Materials used to display blocks. They are made of a texture divided in four quarters. Top-Left is top of the block. Top-Right is sides of the block when block is on top. Bottom-Left is bottom of the block ,and side when block is not on top. Bottom-right is not used.";
			planetMaterialsInfo [1] = "Les Textures des Blocks sont les Materials utilisés pour l'affichage des blocs. Ils sont consistués d'une texture divisé en 4 quarts. En haut à gauche le dessus du bloc. En haut à droit les cotés du bloc si le bloc du dessus est vide. En bas à gauche le dessous du bloc et les cotés du bloc si le bloc du dessus n'est pas vide. En bas à droite est non-utilisé.";
			
			planetMaterialsLabel [0] = new string[2];
			planetMaterialsLabel [0] [0] = "Ground";
			planetMaterialsLabel [0] [1] = "Water";
			planetMaterialsLabel [1] = new string[2];
			planetMaterialsLabel [1] [0] = "Sol";
			planetMaterialsLabel [1] [1] = "Eau";
			
			prefabWarning [0] = "Not disconnecting prefab before playing or reloading scene may cause serialization to misbehave. Using MenuItem \"GameObject/Break Prefab Instance\" avoid issues.";
			prefabWarning [1] = "Garder la connexion avec le Prefab par défaut peut provoquer des disfonctionnement du mécanisme de serialization Unity. Utiliser le menu \"GameObject/Break Prefab Instance\" résoud le problème.";
		}
		
		#endregion
	}
}