using UnityEngine;
using UnityEditor;
using System.IO;
using System.IO.Compression;
using System.Collections;
using SvenFrankson.Game.SphereCraft;
using System;

namespace SvenFrankson.Tools {

	public enum BlockData {
		Empty = 0,
		Grass = 129,
		Dirt = 130,
		Sand = 131,
		Rock = 132,
		Trunc = 133,
		Leaves = 134,
		Snow = 135
	}

	public class PlanetDataGenerator : EditorWindow {

        public string planetName = "Test";
		public int kPosMax = 6;
		public int lowestPointPercent = 70;
		public int highestPointPercent = 80;
		private int maxHeight;
		private int lowestPoint;
		private int highestPoint;
		private int lowToHighPoint;
		private int degreeAtKPosMax;
        public bool overwrite = false;
        public bool babylonJSVersion = false;
        public string output = "";
        private float[][] thresholds;
		private float SandDirtA = -0.013f;
		private float SandDirtB = 0.8f;
		private float DirtSnowA = -0.05f;
		private float DirtSnowB = 4.5f;

		public class LargeMap
		{
			public int size;
			public float[][] heightMap;
			public float[][] latMap;

			public LargeMap(int size)
			{
				this.size = size;
				this.heightMap = new float[size][];
				this.latMap = new float[size][];
				for (int i = 0; i < size; i++)
				{
					this.heightMap[i] = new float[size];
					this.latMap[i] = new float[size];
				}
			}
		}

        [MenuItem("Window/PlanetDataGenerator")]
		static void Open () {
            EditorWindow.GetWindow<PlanetDataGenerator>();
		}

		public static bool Exists(string name) {
			return false;
		}

		public static void Save(string name, LargeMap map) {
			string directoryPath = Application.dataPath + "/../PlanetData/largeMaps";
			Directory.CreateDirectory(directoryPath);
			string saveFilePath = directoryPath + "/" + name + ".png";
			Debug.Log ("Save " + saveFilePath);
			FileStream saveFile = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
			BinaryWriter dataStream = new BinaryWriter(saveFile);

			Color[] colors = new Color[map.size * map.size];
			for (int i = 0; i < map.size; i++) {
				for (int j = 0; j < map.size; j++) {
					colors [i * map.size + j] = new Color (map.heightMap [i] [j], map.heightMap [i] [j], map.heightMap [i] [j]);
				}
			}
			Texture2D imgMap = new Texture2D(map.size, map.size);
			imgMap.SetPixels (colors);

			dataStream.Write(imgMap.EncodeToPNG ());

			dataStream.Close();
			saveFile.Close();
		}

		public static LargeMap Load(string name, int size) {
			string path = Application.dataPath + "/../PlanetData/largeMaps/" + name + ".png";
			if (File.Exists (path)) {
				Debug.Log ("Load " + path);
				byte[] data = File.ReadAllBytes (path);
				Texture2D imgMap = new Texture2D (2, 2);
				imgMap.LoadImage (data);
				LargeMap map = new LargeMap (size);
				for (int i = 0; i < map.size; i++) {
					for (int j = 0; j < map.size; j++) {
						map.heightMap [i] [j] = imgMap.GetPixel (i, j).r;
					}
				}
				return map;
			} 
			else {
				return null;
			}
		}

		public void OnGUI () {
            EditorGUILayout.LabelField("PlanetDataGenerator");
            this.planetName = EditorGUILayout.TextField("PlanetName", this.planetName);
			this.kPosMax = EditorGUILayout.IntField("KPosMax", this.kPosMax);
			this.lowestPointPercent = EditorGUILayout.IntField("LowestPoint (%)", this.lowestPointPercent);
			this.highestPointPercent = EditorGUILayout.IntField("HighestPoint (%)", this.highestPointPercent);
            this.overwrite = EditorGUILayout.Toggle("Overwrite", this.overwrite);
            this.babylonJSVersion = EditorGUILayout.Toggle("BabylonJS Version", this.babylonJSVersion);
            EditorGUILayout.TextArea(this.output);

			if (GUILayout.Button ("Compute Properties")) {

			}
			if (GUILayout.Button ("Create CubeMap")) {
                Check();
            }
		}

        public int SavePlanetInfoFile()
        {
            string directoryPath = Application.dataPath + "/../PlanetData/" + this.planetName + "/";
            Directory.CreateDirectory(directoryPath);
            string saveFilePath = directoryPath + "/" + this.planetName + ".info";
            FileStream saveFile = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
            StreamWriter dataStream = new StreamWriter(saveFile);

            dataStream.WriteLine("#planetName=" + this.planetName);

            dataStream.Close();
            saveFile.Close();

            return 1;
        }

        private void Check()
        {
			this.degreeAtKPosMax = PlanetUtility.KPosToDegree (this.kPosMax);
            this.output = "";
            string directoryPath = Application.dataPath + "/../PlanetData/" + this.planetName + "/";
            if (Directory.Exists(directoryPath))
            {
                if (this.overwrite)
                {
                    EditorUtility.DisplayProgressBar("Planet Data Generator", "Deleting existing files (it might take a few minutes)...", 0f);
                    Directory.Delete(directoryPath, true);
                    this.Generate();
                }
                else
                {
                    this.output += "Planet '" + this.planetName + "' already exists. Aborting.\n" +
                                    "You may pick another name, or check 'Overwrite' to erase the existing '" + this.planetName + "' planet.";
                    return;
                }
            }
            else
            {
                Generate();
            }
            this.output += "Planet '" + this.planetName + "' generated";
        }

        private void Generate()
        {
            float t0 = Time.realtimeSinceStartup;
            float tSave = 0f;
            float tGet = 0f;
            tBuildHeightMap = 0f;
            tSetBytes = 0f;
			RandomSeed mapSeed = new RandomSeed(this.planetName);
			RandomSeed holes0MapSeed = new RandomSeed(this.planetName + "holes0Map");
			RandomSeed holes0HeightMapSeed = new RandomSeed(this.planetName + "holes0HeightMap");
			RandomSeed holes1MapSeed = new RandomSeed(this.planetName + "holes1Map");
			RandomSeed holes1HeightMapSeed = new RandomSeed(this.planetName + "holes1HeightMap");
			RandomSeed holes2MapSeed = new RandomSeed(this.planetName + "holes2Map");
			RandomSeed holes2HeightMapSeed = new RandomSeed(this.planetName + "holes2HeightMap");
            int chunckSaved = 0;
            int chunckTotal = 4242;
			this.degreeAtKPosMax = PlanetUtility.KPosToDegree (this.kPosMax);
			this.maxHeight = PlanetUtility.ChunckSize * (this.kPosMax + 1);
			this.lowestPoint = Mathf.FloorToInt (this.lowestPointPercent / 100f * this.maxHeight);
			this.highestPoint = Mathf.FloorToInt (this.highestPointPercent / 100f * this.maxHeight);
			this.lowToHighPoint = highestPoint - lowestPoint;

            foreach (Planet.Side side in Enum.GetValues(typeof(Planet.Side)))
			{
				LargeMap map = GetLargeMapFor(mapSeed, side, true);
				LargeMap holes0Map = GetLargeMapFor(holes0MapSeed, side);
				LargeMap holes0HeightMap = GetLargeMapFor(holes0HeightMapSeed, side);
				LargeMap holes1Map = GetLargeMapFor(holes1MapSeed, side);
				LargeMap holes1HeightMap = GetLargeMapFor(holes1HeightMapSeed, side);
				LargeMap holes2Map = GetLargeMapFor(holes2MapSeed, side);
				LargeMap holes2HeightMap = GetLargeMapFor(holes2HeightMapSeed, side);

				for (int kPos = 0; kPos <= this.kPosMax; kPos++) {
					int chuncksCount = PlanetUtility.DegreeToChuncksCount (PlanetUtility.KPosToDegree (kPos));
					for (int iPos = 0; iPos < chuncksCount; iPos++) {
						for (int jPos = 0; jPos < chuncksCount; jPos++) {
							float t2 = Time.realtimeSinceStartup;
							Byte[][][] chunckData = GetByteFor (iPos, jPos, kPos, side, map, holes0Map, holes0HeightMap, holes1Map, holes1HeightMap, holes2Map, holes2HeightMap);
							float t3 = Time.realtimeSinceStartup;
							tGet += (t3 - t2);

							float t4 = Time.realtimeSinceStartup;
							if (this.babylonJSVersion) {
								PlanetUtility.SaveForBabylonJSVersion (this.planetName, chunckData, iPos, jPos, kPos, side);
							} else {
								PlanetUtility.Save (this.planetName, chunckData, iPos, jPos, kPos, side);
							}
							float t5 = Time.realtimeSinceStartup;
							tSave += (t5 - t4);

							chunckSaved++;
							EditorUtility.DisplayProgressBar ("Planet Data Generator", "Saving chunck datas for side " + side + " (" + ((int)side + 1) + "/6)...", (float)chunckSaved / (float)chunckTotal);
						}
					}
				}
            }
            float t1 = Time.realtimeSinceStartup;
            this.SavePlanetInfoFile();
            EditorUtility.ClearProgressBar();
            this.output += "Total time to create files : " + (t1 - t0) + "s.\n";
            this.output += "  Including time to compute block byte : " + tGet + "s.\n";
            this.output += "    Including time to build heightMap : " + tBuildHeightMap + "s.\n";
            this.output += "    Including time to set bytes : " + tSetBytes + "s.\n";
			this.output += "  Including time to save files : " + tSave + "s.\n";
			this.output += "MinValue : " + this.minValue + ".\n";
			this.output += "MaxValue : " + this.maxValue + ".\n";
			this.output += "MedianValue : " + (this.valuesSum / this.valuesCount) + ".\n";
			this.output += "KPosMax : " + this.kPosMax + ".\n";
			this.output += "MaxHeight : " + this.maxHeight + ".\n";
			this.output += "LowestPoint : " + this.lowestPoint + ".\n";
			this.output += "LowToHighPoint : " + this.lowToHighPoint + ".\n";
			this.output += "HighestPoint : " + this.highestPoint + ".\n";
        }

        private float tBuildHeightMap = 0f;
        private float tSetBytes = 0f;

		private LargeMap GetLargeMapFor(RandomSeed seed, Planet.Side side, bool withLatMap = false)
		{

			float t0 = Time.realtimeSinceStartup;

			int x, y, z;
			int size = PlanetUtility.DegreeToSize (PlanetUtility.KPosToDegree (this.kPosMax));
			string mapName = side + "-" + degreeAtKPosMax + "-" + seed.seed;
			bool evaluate = false;
			LargeMap map = Load (mapName, size);
			if (map == null) {
				map = new LargeMap (size);
				evaluate = true;
			}

			if (side == Planet.Side.Top)
			{
				y = size;
				for (int i = 0; i < size; i++)
				{
					for (int j = 0; j < size; j++)
					{
						x = size - j;
						z = i;
						if (evaluate) {
							map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
						}
						if (withLatMap) {
							map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
						}
					}
				}
			}
			else if (side == Planet.Side.Bottom)
			{
				y = 0;
				for (int i = 0; i < size; i++)
				{
					for (int j = 0; j < size; j++)
					{
						x = j;
						z = i;
						if (evaluate) {
							map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
						}
						if (withLatMap) {
							map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
						}
					}
				}
			}
			else if (side == Planet.Side.Right)
			{
				x = size;
				for (int i = 0; i < size; i++)
				{
					for (int j = 0; j < size; j++)
					{
						y = j;
						z = i;
						if (evaluate) {
							map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
						}
						if (withLatMap) {
							map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
						}
					}
				}
			}
			else if (side == Planet.Side.Left)
			{
				x = 0;
				for (int i = 0; i < size; i++)
				{
					for (int j = 0; j < size; j++)
					{
						y = j;
						z = size - i;
						if (evaluate) {
							map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
						}
						if (withLatMap) {
							map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
						}
					}
				}
			}
			else if (side == Planet.Side.Front)
			{
				z = size;
				for (int i = 0; i < size; i++)
				{
					for (int j = 0; j < size; j++)
					{
						y = j;
						x = size - i;
						if (evaluate) {
							map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
						}
						if (withLatMap) {
							map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
						}
					}
				}
			}
			else if (side == Planet.Side.Back)
			{
				z = 0;
				for (int i = 0; i < size; i++)
				{
					for (int j = 0; j < size; j++)
					{
						y = j;
						x = i;
						if (evaluate) {
							map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
						}
						if (withLatMap) {
							map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
						}
					}
				}
			}
			Save (mapName, map);
			float t1 = Time.realtimeSinceStartup;
			tBuildHeightMap += t1 - t0;

			return map;
		}

		private BlockData GetSoilBlock(float latitude, int i, int j, int k) {
			float random = Mathf.Abs (Mathf.Cos (i * 13 + j * 93 + k * 41));
			if (random < this.SandDirtA * latitude + this.SandDirtB) {
				return BlockData.Sand;
			}
			if (random < this.DirtSnowA * latitude + this.DirtSnowB) {
				return BlockData.Dirt;
			}
			return BlockData.Snow;
		}
			
		private Byte[][][] GetByteFor(
			int iPos,
			int jPos,
			int kPos,
			Planet.Side side,
			LargeMap map, LargeMap
			holes0Map,
			LargeMap holes0DepthMap,
			LargeMap holes1Map,
			LargeMap holes1DepthMap,
			LargeMap holes2Map,
			LargeMap holes2DepthMap
		)
		{
            Byte[][][] chunckData = new Byte[PlanetUtility.ChunckSize][][];
			int size = PlanetUtility.DegreeToSize (PlanetUtility.KPosToDegree (kPos));
			int mapSize = map.heightMap.Length;
			int sizeRatio = mapSize / size;

            float t2 = Time.realtimeSinceStartup;
            for (int i = 0; i < PlanetUtility.ChunckSize; i++)
            {
                chunckData[i] = new Byte[PlanetUtility.ChunckSize][];
                for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                {
                    chunckData[i][j] = new Byte[PlanetUtility.ChunckSize];
                    for (int k = 0; k < PlanetUtility.ChunckSize; k++)
                    {
						int iGlobal = iPos * PlanetUtility.ChunckSize + i;
						int jGlobal = jPos * PlanetUtility.ChunckSize + j;
						int kGlobal = kPos * PlanetUtility.ChunckSize + k;
						int heightThreshold = Mathf.FloorToInt(map.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * lowToHighPoint + lowestPoint);
						int h0Alt = Mathf.FloorToInt(holes0Map.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * lowToHighPoint + lowestPoint - lowToHighPoint / 2);
						int h0Depth = Mathf.FloorToInt(holes0DepthMap.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * lowToHighPoint / 2);
						//h0Depth = 0;
						int h1Alt = Mathf.FloorToInt(holes1Map.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * lowToHighPoint + lowestPoint - lowToHighPoint);
						int h1Depth = Mathf.FloorToInt(holes1DepthMap.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * lowToHighPoint / 2);
						//h1Depth = 0;
						int h2Alt = Mathf.FloorToInt(holes2Map.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * lowToHighPoint + lowestPoint - 3 * lowToHighPoint / 2);
						int h2Depth = Mathf.FloorToInt(holes2DepthMap.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * lowToHighPoint / 2);

						BlockData data = BlockData.Empty;

						// if k is under main heightMap threshold
						if (kGlobal <= heightThreshold) {
							// if k is not in generated hole0
							if ((kGlobal <= h0Alt) || (kGlobal >= h0Alt + h0Depth)) {
								// if k is not in generated hole1
								if ((kGlobal <= h1Alt) || (kGlobal >= h1Alt + h1Depth)) {
									// if k is not in generated hole2
									if ((kGlobal <= h2Alt) || (kGlobal >= h2Alt + h2Depth)) {
									// then data is not empty, set rock as default
										data = BlockData.Rock;
										// compute depth of soil block (blocks that are not rocks)
										int soilDepth = Mathf.FloorToInt (Mathf.Abs (Mathf.Cos (i * 53 + j * 41 + k * 29) * 5) + 1);
										// if k is close to the surface of the map or of any hole
										if (
											((kGlobal <= heightThreshold) && (heightThreshold - kGlobal < soilDepth)) ||
											((kGlobal <= h0Alt) && (h0Alt - kGlobal < soilDepth)) ||
											((kGlobal <= h1Alt) && (h1Alt - kGlobal < soilDepth)) ||
											((kGlobal <= h2Alt) && (h2Alt - kGlobal < soilDepth))) {
											// then data is soil block
											data = GetSoilBlock (map.latMap [iGlobal * sizeRatio] [jGlobal * sizeRatio], iGlobal, jGlobal, kGlobal);
											// if data is Dirt
											if (data == BlockData.Dirt) {
												// and if data is strictly at surface of any hole
												if (
													(kGlobal == heightThreshold) ||
													(kGlobal == h0Alt)) {
													data = BlockData.Grass;
												}
											}
										}
									}
								}
							}
						}

						/*
						if (kGlobal <= heightThreshold) {
							// then data is not empty, set rock as default
							data = BlockData.Rock;
							// compute depth of soil block (blocks that are not rocks)
							int soilDepth = Mathf.FloorToInt (Mathf.Abs (Mathf.Cos (i * 53 + j * 41 + k * 29) * 5) + 1);
							if (heightThreshold - kGlobal < soilDepth) {
								// then data is soil block
								data = GetSoilBlock (map.latMap [iGlobal * sizeRatio] [jGlobal * sizeRatio], iGlobal, jGlobal, kGlobal);
								// if data is Dirt
								if (data == BlockData.Dirt) {
									// and if data is strictly at surface of any hole
									if (kGlobal == heightThreshold) {
										data = BlockData.Grass;
									}
								}
							}
						}
						*/
						chunckData [i] [j] [k] = (byte) data;
                    }
                }
            }
            float t3 = Time.realtimeSinceStartup;
            tSetBytes += t3 - t2;

            return chunckData;
        }

        private float TERP(float t, float a, float b, float c, float d)
        {
            return 0.5f * (c - a + (2.0f * a - 5.0f * b + 4.0f * c - d + (3.0f * (b - c) + d - a) * t) * t) * t + b;
        }

		private float minValue = 0;
		private float maxValue = 0;
		private float valuesSum = 0;
		private float valuesCount = 0;
        public float EvaluateTriCubic(int x, int y, int z, RandomSeed r)
        {
            float value = 0f;

			for (int d = 2; d < degreeAtKPosMax; d++)
            {
				int range = Mathf.FloorToInt(Mathf.Pow(2f, degreeAtKPosMax - d));
                int x0 = (x / range) * range;
                int y0 = (y / range) * range;
                int z0 = (z / range) * range;

                float xd = (float)(x % range) / (float)range;
                float yd = (float)(y % range) / (float)range;
                float zd = (float)(z % range) / (float)range;

                float[][][] f = new float[4][][];
                for (int i = 0; i < 4; i++)
                {
                    f[i] = new float[4][];
                    for (int j = 0; j < 4; j++)
                    {
                        f[i][j] = new float[4];
                        for (int k = 0; k < 4; k++)
                        {
                            f[i][j][k] = r.Rand(x0 + (i - 1) * range, y0 + (j - 1) * range, z0 + (k - 1) * range, d);
                        }
                    }
                }

                float[][] fz = new float[4][];
                for (int i = 0; i < 4; i++)
                {
                    fz[i] = new float[4];
                    for (int j = 0; j < 4; j++)
                    {
                        fz[i][j] = TERP(zd, f[i][j][0], f[i][j][1], f[i][j][2], f[i][j][3]);
                    }
                }

                float[] fy = new float[4];
                for (int i = 0; i < 4; i++)
                {
                    fy[i] = TERP(yd, fz[i][0], fz[i][1], fz[i][2], fz[i][3]);
                }

                float fx = TERP(xd, fy[0], fy[1], fy[2], fy[3]);

                value += fx / Mathf.FloorToInt(Mathf.Pow(2f, d - 2));
            }

            // Note : value belongs to  [-1.5f ; +1.5f]
			value = (value + 1.5f) / 3f;
			this.minValue = Mathf.Min (this.minValue, value);
			this.maxValue = Mathf.Max (this.maxValue, value);
			this.valuesCount++;
			this.valuesSum += value;
			return value;
        }
	}
}