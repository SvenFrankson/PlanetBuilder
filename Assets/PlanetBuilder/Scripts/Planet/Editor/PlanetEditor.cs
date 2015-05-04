using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	[InitializeOnLoad]
	public class PlanetEditor : Editor {
		
		static Vector3 mousePos = Vector3.zero;
		static Vector3 mouseRayImpact = Vector3.zero;
		static private float mouseRayCorrection = 0.2f;
		static private Planet selectedPlanet = null;
		static private bool planetEditMode = false;
		static private bool planetHit = false;
		static private int selectedBlock = 0;
		static private GameObject selectedGameObject = null;

		static private Texture2D planetEditorLogo = null;
		static private Texture2D PlanetEditorLogo {
			get {
				if (PlanetEditor.planetEditorLogo == null) {
					PlanetEditor.planetEditorLogo = Resources.LoadAssetAtPath<Texture2D> ("Assets/Resources/icons/logoPlanetEditor.png");
					
				}
				return PlanetEditor.planetEditorLogo;
			}
		}

		static private Texture2D planetEraseLogo = null;
		static private Texture2D PlanetEraseLogo {
			get {
				if (PlanetEditor.planetEraseLogo == null) {
					PlanetEditor.planetEraseLogo = Resources.LoadAssetAtPath<Texture2D> ("Assets/Resources/icons/logoPlanetErase.png");
					
				}
				return PlanetEditor.planetEraseLogo;
			}
		}
		
		static private Texture2D planetDirtLogo = null;
		static private Texture2D PlanetDirtLogo {
			get {
				if (PlanetEditor.planetDirtLogo == null) {
					PlanetEditor.planetDirtLogo = Resources.LoadAssetAtPath<Texture2D> ("Assets/Resources/icons/logoPlanetDirt.png");
					
				}
				return PlanetEditor.planetDirtLogo;
			}
		}
		
		static private Texture2D planetRockLogo = null;
		static private Texture2D PlanetRockLogo {
			get {
				if (PlanetEditor.planetRockLogo == null) {
					PlanetEditor.planetRockLogo = Resources.LoadAssetAtPath<Texture2D> ("Assets/Resources/icons/logoPlanetRock.png");
					
				}
				return PlanetEditor.planetRockLogo;
			}
		}
		
		static private Texture2D planetSandLogo = null;
		static private Texture2D PlanetSandLogo {
			get {
				if (PlanetEditor.planetSandLogo == null) {
					PlanetEditor.planetSandLogo = Resources.LoadAssetAtPath<Texture2D> ("Assets/Resources/icons/logoPlanetSand.png");
					
				}
				return PlanetEditor.planetSandLogo;
			}
		}
		
		static private Texture2D planetDustLogo = null;
		static private Texture2D PlanetDustLogo {
			get {
				if (PlanetEditor.planetDustLogo == null) {
					PlanetEditor.planetDustLogo = Resources.LoadAssetAtPath<Texture2D> ("Assets/Resources/icons/logoPlanetDust.png");
					
				}
				return PlanetEditor.planetDustLogo;
			}
		}

		static private Material eraseMaterial = null;
		static private Material EraseMaterial {
			get {
				if (PlanetEditor.eraseMaterial == null) {
					PlanetEditor.eraseMaterial = Resources.LoadAssetAtPath<Material> ("Assets/Resources/Materials/PlanetErase.mat");
					
				}
				return PlanetEditor.eraseMaterial;
			}
		}
		
		static private Material vegetationMaterial = null;
		static private Material VegetationMaterial {
			get {
				if (PlanetEditor.vegetationMaterial == null) {
					PlanetEditor.vegetationMaterial = Resources.LoadAssetAtPath<Material> ("Assets/Resources/Materials/PlanetVegetation.mat");
					
				}
				return PlanetEditor.vegetationMaterial;
			}
		}

		static private List<GameObject> prefabsVegetation = null;
		static private List<GameObject> PrefabsVegetation {
			get {
				if (prefabsVegetation == null) {
					prefabsVegetation = new List<GameObject> ();
					Object[] vegetationObjects = Resources.LoadAll<GameObject> ("Prefab/Vegetation");
					foreach (Object o in vegetationObjects) {
						prefabsVegetation.Add (o as GameObject);
					}
				}

				return prefabsVegetation;
			}
		}

		static private List<Texture2D> prefabsVegetationLogo = null;
		static private List<Texture2D> PrefabsVegetationLogo {
			get {
				if (prefabsVegetationLogo == null) {
					prefabsVegetationLogo = new List<Texture2D> ();
					foreach (GameObject go in PrefabsVegetation) {
						prefabsVegetationLogo.Add (AssetPreview.GetAssetPreview (go));
					}
				}

				return prefabsVegetationLogo;
			}
		}

		static private Vector2 scrollPos = Vector2.zero;

		static PlanetEditor () {
			EditorApplication.update += PlanetEditor.PlanetEditorUpdate;
			SceneView.onSceneGUIDelegate += PlanetEditor.PlanetEditorGUI;
		}

		static void PlanetEditorGUI (SceneView sceneView) {
			mousePos.x = Event.current.mousePosition.x / Screen.width;
			mousePos.y = (SceneView.lastActiveSceneView.camera.pixelHeight - Event.current.mousePosition.y) / SceneView.lastActiveSceneView.camera.pixelHeight;

			Handles.BeginGUI ();
			if (PlanetEditor.PlanetSelected () != null) {
				GUILayout.Button (PlanetEditorLogo, GUILayout.Height (50f), GUILayout.Width (150f));
				if (!PlanetEditor.planetEditMode) {
					if (GUILayout.Button ("EDIT", GUILayout.Height (30f), GUILayout.Width (150f))) {
						PlanetEditor.selectedPlanet = PlanetEditor.PlanetSelected ();
						PlanetEditor.planetEditMode = true;
						PlanetEditor.selectedPlanet.BuildColliders ();
						PlanetEditor.selectedBlock = 0;
						PlanetCursor.SetMaterial (PlanetEditor.EraseMaterial);
					}
				}
				else if (PlanetEditor.planetEditMode) {
					if (GUILayout.Button ("OVER", GUILayout.Height (30f), GUILayout.Width (150f))) {
						PlanetEditor.planetEditMode = false;
						PlanetCursor.HideCursor ();
					}

					GUILayout.Button ("Cursor. i = " + PlanetCursor.posInPlanetSide.x + " | j = " + PlanetCursor.posInPlanetSide.y + " | h = " + PlanetCursor.posInPlanetSide.z, GUILayout.Height (30f), GUILayout.Width (250f));
					
					scrollPos = GUILayout.BeginScrollView (scrollPos, GUILayout.Width (110f));
					if (GUILayout.Button (PlanetEditor.PlanetEraseLogo, GUILayout.Height (80f), GUILayout.Width (80f))) {
						PlanetEditor.mouseRayCorrection = 0.2f;
						PlanetEditor.selectedBlock = 0;
						PlanetCursor.SetMaterial (PlanetEditor.EraseMaterial);
					}
					if (GUILayout.Button (PlanetEditor.PlanetDirtLogo, GUILayout.Height (80f), GUILayout.Width (80f))) {
						PlanetEditor.mouseRayCorrection = -0.2f;
						PlanetEditor.selectedBlock = 1;
						PlanetCursor.SetMaterial (PlanetEditor.selectedPlanet.planetMaterials[0]);
					}
					if (GUILayout.Button (PlanetEditor.PlanetRockLogo, GUILayout.Height (80f), GUILayout.Width (80f))) {
						PlanetEditor.mouseRayCorrection = -0.2f;
						PlanetEditor.selectedBlock = 3;
						PlanetCursor.SetMaterial (PlanetEditor.selectedPlanet.planetMaterials[2]);
					}
					if (GUILayout.Button (PlanetEditor.PlanetSandLogo, GUILayout.Height (80f), GUILayout.Width (80f))) {
						PlanetEditor.mouseRayCorrection = -0.2f;
						PlanetEditor.selectedBlock = 4;
						PlanetCursor.SetMaterial (PlanetEditor.selectedPlanet.planetMaterials[3]);
					}
					if (GUILayout.Button (PlanetEditor.PlanetDustLogo, GUILayout.Height (80f), GUILayout.Width (80f))) {
						PlanetEditor.mouseRayCorrection = -0.2f;
						PlanetEditor.selectedBlock = 5;
						PlanetCursor.SetMaterial (PlanetEditor.selectedPlanet.planetMaterials[4]);
					}

					for (int i = 0; i < PrefabsVegetation.Count; i++) {
						if (GUILayout.Button (PrefabsVegetationLogo[i], GUILayout.Height (80f), GUILayout.Width (80f))) {
							PlanetEditor.mouseRayCorrection = -0.2f;
							PlanetEditor.selectedBlock = 100;
							PlanetCursor.SetMaterial (VegetationMaterial);
							selectedGameObject = PrefabsVegetation[i];
						}
					}
					GUILayout.EndScrollView ();
					
					if (GUILayout.Button ("Vegetation Wizard", GUILayout.Height (30f), GUILayout.Width (150f))) {
						PlanetVegetationWindow.Open (selectedPlanet, PrefabsVegetation, PrefabsVegetationLogo);
					}

					if (PrefabsVegetation.Count > 0) {
						if (PrefabsVegetation [0] == null) {
							Debug.Log ("IsNull");
						}
					}

					if (GUILayout.Button ("Refresh Vegetation Pictures", GUILayout.Height (30f), GUILayout.Width (150f))) {
						prefabsVegetationLogo = null;
					}
					EditorGUILayout.Space ();
					EditorGUILayout.Space ();
					EditorGUILayout.Space ();
					EditorGUILayout.Space ();
				}
			}
			Handles.EndGUI ();

			if (PlanetEditor.planetEditMode) {

				if (PlanetEditor.planetHit) {
					selectedPlanet.SetCursorAt (PlanetEditor.selectedBlock, mouseRayImpact);
				}
				else {
					PlanetCursor.HideCursor ();
				}

				if (Event.current.type == EventType.MouseDown) {
					if (Event.current.button == 0) {
						if (selectedBlock < 100) {
							PlanetEditor.selectedPlanet.AddBlockAt (PlanetEditor.mouseRayImpact, PlanetEditor.selectedBlock);
						}
						else {
							PlanetEditor.selectedPlanet.AddGameObjectAtCursor (selectedGameObject);
						}
					}
				}
			}
		}

		static Planet PlanetSelected () {
			if (Selection.activeGameObject != null) {
				if (Selection.activeGameObject.GetComponent<Planet> () != null) {
					return Selection.activeGameObject.GetComponent<Planet> ();
				}
			}

			return null;
		}

		static void PlanetEditorUpdate () {
			if (PlanetEditor.planetEditMode) {
				if (Selection.activeGameObject != PlanetEditor.selectedPlanet.gameObject) {
					Selection.activeGameObject = PlanetEditor.selectedPlanet.gameObject;
				}
			}

			if (PlanetEditor.planetEditMode) {
				if (SceneView.lastActiveSceneView != null) {
					Ray mouseRay = SceneView.lastActiveSceneView.camera.ViewportPointToRay(mousePos);
					RaycastHit hitInfo = new RaycastHit ();
					Physics.Raycast (mouseRay, out hitInfo);

					PlanetEditor.planetHit = (hitInfo.distance > 0f);
					mouseRayImpact = mouseRay.GetPoint (hitInfo.distance + PlanetEditor.mouseRayCorrection);
					
					SceneView.lastActiveSceneView.Repaint ();
				}
			}
		}
	}
}