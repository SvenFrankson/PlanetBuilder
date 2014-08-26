using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SvenFrankson.Game.SphereCraft {

	public class PlanetTutorials : EditorWindow {

		static private int language = 0;
		
		static private Texture2D logo = null;
		static private Texture2D Logo {
			get {
				if (logo == null) {
					logo = Resources.LoadAssetAtPath<Texture2D> ("Assets/Resources/icons/logoPlanetTutoSmall.png");
					
				}
				return logo;
			}
		}
		
		static private Texture2D englishFlag = null;
		static private Texture2D EnglishFlag {
			get {
				if (englishFlag == null) {
					englishFlag = Resources.LoadAssetAtPath<Texture2D> ("Assets/Resources/icons/logoEnglish.png");
					
				}
				return englishFlag;
			}
		}
		
		static private Texture2D frenchFlag = null;
		static private Texture2D FrenchFlag {
			get {
				if (frenchFlag == null) {
					frenchFlag = Resources.LoadAssetAtPath<Texture2D> ("Assets/Resources/icons/logoFrench.png");
					
				}
				return frenchFlag;
			}
		}

		[MenuItem ("Planet Builder/Tutorials and Documentations")]
		static public void Open () {
			PlanetTutorials win = EditorWindow.GetWindow<PlanetTutorials> ();
			win.minSize = new Vector2 (350, 600);
		}

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

			EditorGUILayout.BeginVertical ("box");
			EditorGUILayout.LabelField (tuto1Label[language]);
			EditorGUILayout.HelpBox (tuto1HelpBox[language], MessageType.Info);
			if (GUILayout.Button ("Youtube Video")) {
				Application.OpenURL ("https://www.youtube.com/watch?v=ZKu7GosU4N0");
			}
			EditorGUILayout.EndVertical ();
			
			EditorGUILayout.BeginVertical ("box");
			EditorGUILayout.LabelField (tuto2Label[language]);
			EditorGUILayout.HelpBox (tuto2HelpBox[language], MessageType.Info);
			if (GUILayout.Button ("Youtube Video")) {
				Application.OpenURL ("https://www.youtube.com/watch?v=ZKu7GosU4N0");
			}
			EditorGUILayout.EndVertical ();
		}

		#region Text

		static private string[] tuto1Label = new string[2];
		static private string[] tuto1HelpBox = new string[2];
		
		static private string[] tuto2Label = new string[2];
		static private string[] tuto2HelpBox = new string[2];
		
		static PlanetTutorials () {
			tuto1Label [0] = "Tutorial 1 : Starting in Editor Mode.";
			tuto1Label [1] = "Tutoriel 1 : Introduction en mode Editeur.";

			tuto1HelpBox [0] = "Adding PlanetBuilder to project.\nInstantiating a planet.\nUsing Planet Builder options to set it up.\nEditing Planet block by block.";
			tuto1HelpBox [1] = "Ajouter PlanetBuilder au projet.\nInstancier une planète.\nUtiliser les options de paramétrage de Planet Builder.\nEditer une planète bloc par block.";
		
			tuto2Label [0] = "Tutorial 2 : Vegetation Wizard.";
			tuto2Label [1] = "Tutoriel 2 : Vegetation Wizard.";
			
			tuto2HelpBox [0] = "Add plants block by block.\nVegetation Wizard, height options.\nVegetation Wizard, frequency options.";
			tuto2HelpBox [1] = "Ajouter la vegetation bloc par block.\nOptions d'altitude du Vegetation Wizard.\nOptions de fréquence du VegetationWizard.";
		}
		
		#endregion
	}
}
