using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SvenFrankson.Tools {

	public class CubeMapGenerator : EditorWindow {

		public int size = 128;
		public int smoothness = 3;

		[MenuItem ("Window/CubeMapGenerator")]
		static void Open () {
			EditorWindow.GetWindow<CubeMapGenerator> ();
		}

		public void OnGUI () {
			this.name = EditorGUILayout.TextField ("Name", this.name);
			this.size = EditorGUILayout.IntField ("Size", this.size);
			this.smoothness = EditorGUILayout.IntField ("Smoothness", this.smoothness);

			if (GUILayout.Button ("Create CubeMap")) {
				this.Build ();
			}
		}

		public void Build () {

			int [][][] cubeBase = CubeMapUtility.GenerateRandomCube (this.size);

			int [][][] cubeFour = CubeMapUtility.ExtractRandomEighth (cubeBase);
			for (int i = 0; i < this.smoothness; i++) {
				cubeFour = CubeMapUtility.GaussianBlur (cubeFour);
			}
			
			int [][][] cubeThree = CubeMapUtility.ExtractRandomEighth (cubeFour);
			for (int i = 0; i < this.smoothness; i++) {
				cubeThree = CubeMapUtility.GaussianBlur (cubeThree);
			}
			
			int [][][] cubeTwo = CubeMapUtility.ExtractRandomEighth (cubeThree);
			for (int i = 0; i < this.smoothness; i++) {
				cubeTwo = CubeMapUtility.GaussianBlur (cubeTwo);
			}

			int [][][] cubeOne = CubeMapUtility.ExtractRandomEighth (cubeTwo);
			for (int i = 0; i < this.smoothness; i++) {
				cubeOne = CubeMapUtility.GaussianBlur (cubeOne);
			}
			
			int [][][] cubeZero = CubeMapUtility.ExtractRandomEighth (cubeOne);
			for (int i = 0; i < this.smoothness; i++) {
				cubeZero = CubeMapUtility.GaussianBlur (cubeZero);
			}

			cubeZero = CubeMapUtility.Fuse (cubeZero, cubeOne, 2);
			cubeZero = CubeMapUtility.Fuse (cubeZero, cubeTwo, 4);
			cubeZero = CubeMapUtility.Fuse (cubeZero, cubeThree, 8);
			cubeZero = CubeMapUtility.Fuse (cubeZero, cubeFour, 16);
			cubeZero = CubeMapUtility.Fuse (cubeZero, cubeBase, 32);
			CubeMapUtility.BuildCubeMap (this.name, cubeZero);
		}
	}
}