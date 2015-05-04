using UnityEngine;
using UnityEditor;
using System.Collections;

namespace SvenFrankson.Tools {

	public class CubeMapUtility {

		static public int [][][] GenerateRandomCube (int size) {
			int [][][] randomCube = new int[size][][];

			for (int i = 0; i < size; i++) {
				randomCube [i] = new int[size][];
				for (int j = 0; j < size; j++) {
					randomCube [i][j] = new int[size];
					for (int k = 0; k < size; k++) {
						randomCube [i][j][k] = UnityEngine.Random.Range (0, 255);
					}
				}
			}

			return randomCube;
		}

		static public int [][][] ExtractRandomEighth (int [][][] originalCube) {
			int size = originalCube.Length;

			int [][][] extractedCube = new int[size][][];
			
			for (int i = 0; i < size; i++) {
				extractedCube [i] = new int[size][];
				for (int j = 0; j < size; j++) {
					extractedCube [i][j] = new int[size];
				}
			}

			int i0 = UnityEngine.Random.Range (0, size / 2);
			int j0 = UnityEngine.Random.Range (0, size / 2);
			int k0 = UnityEngine.Random.Range (0, size / 2);

			for (int i = 0; i < size / 2; i++) {
				for (int j = 0; j < size / 2; j++) {
					for (int k = 0; k < size / 2; k++) {
						int h = originalCube [i0 + i][j0 + j][k0 + k];

						extractedCube [2 * i][2 * j][2 * k] = h;
						extractedCube [2 * i][2 * j][2 * k + 1] = h;
						extractedCube [2 * i][2 * j + 1][2 * k] = h;
						extractedCube [2 * i][2 * j + 1][2 * k + 1] = h;
						extractedCube [2 * i + 1][2 * j][2 * k] = h;
						extractedCube [2 * i + 1][2 * j][2 * k + 1] = h;
						extractedCube [2 * i + 1][2 * j + 1][2 * k] = h;
						extractedCube [2 * i + 1][2 * j + 1][2 * k + 1] = h;
					}
				}
			}

			return extractedCube;
		}

		static public int [][][] GaussianBlur (int [][][] cube) {
			int size = cube.Length;

			int [][][] blurredCube = new int[size][][];
			
			blurredCube = new int[size][][];
			for (int i = 0; i < size; i++) {
				blurredCube [i] = new int[size][];
				for (int j = 0; j < size; j++) {
					blurredCube [i][j] = new int[size];
				}
			}

			for (int i = 0; i < size; i++) {
				for (int j = 0; j < size; j++) {
					for (int k = 0; k < size; k++) {

						int h = cube [i][j][k];
						int c = 1;

						if (i > 0) {
							h += cube [i - 1][j][k];
							c ++;
						}
						if (j > 0) {
							h += cube [i][j - 1][k];
							c ++;
						}
						if (k > 0) {
							h += cube [i][j][k - 1];
							c ++;
						}
						if (i + 1 < size) {
							h += cube [i + 1][j][k];
							c ++;
						}
						if (j + 1 < size) {
							h += cube [i][j + 1][k];
							c ++;
						}
						if (k + 1 < size) {
							h += cube [i][j][k + 1];
							c ++;
						}

						blurredCube [i][j][k] = h / c;
					}
				}
			}

			return blurredCube;
		}

		static public int [][][] Fuse (int [][][] mainCube, int [][][] otherCube, int coef) {
			int size = mainCube.Length;

			int [][][] fusedCube = new int[size][][];
			
			for (int i = 0; i < size; i++) {
				fusedCube [i] = new int[size][];
				for (int j = 0; j < size; j++) {
					fusedCube [i][j] = new int[size];
					for (int k = 0; k < size; k++) {
						fusedCube [i][j][k] = mainCube [i][j][k] + (otherCube [i][j][k] - 128) / coef;
						if (fusedCube [i][j][k] < 0) {
							fusedCube [i][j][k] = 0;
						}
						else if (fusedCube [i][j][k] > 255) {
							fusedCube [i][j][k] = 255;
						}
					}
				}
			}

			return fusedCube;
		}

		static public void BuildCubeMap (string name, int [][][] cube) {
			int size = cube.Length;

			Color[] upPixels = new Color [size * size];
			Color[] downPixels = new Color [size * size];
			Color[] rightPixels = new Color [size * size];
			Color[] leftPixels = new Color [size * size];
			Color[] forwardPixels = new Color [size * size];
			Color[] backPixels = new Color [size * size];

			float h = 0;

			for (int i = 0; i < size; i++) {
				for (int j = 0; j < size; j++) {
					h = cube [size - 1 - j][size - 1][size - 1 - i];
					upPixels [size * size - 1 - (j + i * size)] = new Color (h/255f, h/255f, h/255f, 1f);
					
					h = cube [size - 1 - j][0][i];
					downPixels [size * size - 1 - (j + i * size)] = new Color (h/255f, h/255f, h/255f, 1f);
					
					h = cube [size - 1][i][j];
					rightPixels [size * size - 1 - (j + i * size)] = new Color (h/255f, h/255f, h/255f, 1f);
					
					h = cube [0][i][size - 1 - j];
					leftPixels [size * size - 1 - (j + i * size)] = new Color (h/255f, h/255f, h/255f, 1f);
					
					h = cube [size - 1 - j][i][size - 1];
					forwardPixels [size * size - 1 - (j + i * size)] = new Color (h/255f, h/255f, h/255f, 1f);
					
					h = cube [j][i][0];
					backPixels [size * size - 1 - (j + i * size)] = new Color (h/255f, h/255f, h/255f, 1f);
				}
			}

			Cubemap cubeMap = new Cubemap (size, TextureFormat.RGBA32, false);
			cubeMap.SetPixels (upPixels, CubemapFace.PositiveY);
			cubeMap.SetPixels (downPixels, CubemapFace.NegativeY);
			cubeMap.SetPixels (rightPixels, CubemapFace.PositiveX);
			cubeMap.SetPixels (leftPixels, CubemapFace.NegativeX);
			cubeMap.SetPixels (forwardPixels, CubemapFace.PositiveZ);
			cubeMap.SetPixels (backPixels, CubemapFace.NegativeZ);

			AssetDatabase.CreateAsset (cubeMap, "Assets/Resources/CubeMapGenerator/Cubemaps/" + name + ".asset");
		}
	}
}