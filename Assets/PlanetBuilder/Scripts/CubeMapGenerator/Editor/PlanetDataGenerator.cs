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
		private int degree;
        public bool overwrite = false;
        public bool babylonJSVersion = false;
        public string output = "";
        private float[][] thresholds;

		private class LargeMap
		{
			public float[][] heightMap;

			public LargeMap(int size)
			{
				this.heightMap = new float[size][];
				for (int i = 0; i < size; i++)
				{
					this.heightMap[i] = new float[size];
				}
			}
		}

        [MenuItem("Window/PlanetDataGenerator")]
		static void Open () {
            EditorWindow.GetWindow<PlanetDataGenerator>();
		}

		public void OnGUI () {
            EditorGUILayout.LabelField("PlanetDataGenerator");
            this.planetName = EditorGUILayout.TextField("PlanetName", this.planetName);
			this.kPosMax = EditorGUILayout.IntField("KPosMax", this.kPosMax);
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
			this.degree = PlanetUtility.KPosToDegree (this.kPosMax);
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
			RandomSeed holesMapSeed = new RandomSeed(this.planetName + "holesMap");
			RandomSeed holesHeightMapSeed = new RandomSeed(this.planetName + "holesHeightMap");
            int chunckSaved = 0;
            int chunckTotal = 4242;

            foreach (Planet.Side side in Enum.GetValues(typeof(Planet.Side)))
			{
				LargeMap map = GetLargeMapFor(mapSeed, side);
				LargeMap holesMap = GetLargeMapFor(holesMapSeed, side);
				LargeMap holesHeightMap = GetLargeMapFor(holesHeightMapSeed, side);

				for (int kPos = 0; kPos < this.kPosMax; kPos++) {
					int chuncksCount = PlanetUtility.DegreeToChuncksCount (PlanetUtility.KPosToDegree (kPos));
					for (int iPos = 0; iPos < chuncksCount; iPos++) {
						for (int jPos = 0; jPos < chuncksCount; jPos++) {
							float t2 = Time.realtimeSinceStartup;
							Byte[][][] chunckData = GetByteFor (iPos, jPos, kPos, side, map, holesMap, holesHeightMap);
							float t3 = Time.realtimeSinceStartup;
							tGet += (t3 - t2);

							float t4 = Time.realtimeSinceStartup;
							if (this.babylonJSVersion) {
								PlanetUtility.SaveForBabylonJSVersion (this.planetName, chunckData, iPos, jPos, kPos, side);
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
        }

        private float tBuildHeightMap = 0f;
        private float tSetBytes = 0f;

		private LargeMap GetLargeMapFor(RandomSeed seed, Planet.Side side)
		{
			int x, y, z;
			int size = PlanetUtility.DegreeToSize (PlanetUtility.KPosToDegree (this.kPosMax));
			LargeMap map = new LargeMap (size);
			float t0 = Time.realtimeSinceStartup;

			if (side == Planet.Side.Top)
			{
				y = size;
				for (int i = 0; i < size; i++)
				{
					for (int j = 0; j < size; j++)
					{
						x = size - j;
						z = i;
						map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
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
						map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
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
						map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
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
						map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
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
						map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
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
						map.heightMap[i][j] = EvaluateTriCubic(x, y, z, seed);
					}
				}
			}
			float t1 = Time.realtimeSinceStartup;
			tBuildHeightMap += t1 - t0;

			return map;
		}
			
		private Byte[][][] GetByteFor(int iPos, int jPos, int kPos, Planet.Side side, LargeMap map, LargeMap holesMap, LargeMap holesHeightMap)
		{
			int maxHeight = PlanetUtility.ChunckSize * this.kPosMax;
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
						int heightThreshold = Mathf.FloorToInt(map.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * (maxHeight / 2) + (maxHeight / 2));
						int holeHeight = Mathf.FloorToInt (holesMap.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * 64 - 32);
						holeHeight = Math.Max (holeHeight, 0);
						int holeAltitude = Mathf.FloorToInt(holesHeightMap.heightMap[iGlobal * sizeRatio][jGlobal * sizeRatio] * (maxHeight / 2) + (maxHeight / 2));

						if (kGlobal < heightThreshold) {
							if (kGlobal < holeAltitude || kGlobal > holeAltitude + holeHeight - 1) {
								chunckData [i] [j] [k] = (byte) BlockData.Dirt;
							} else {
								chunckData [i] [j] [k] = (byte) BlockData.Empty;
							}
						} else {
							chunckData [i] [j] [k] = (byte) BlockData.Empty;
						}
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
        public float EvaluateTriCubic(int x, int y, int z, RandomSeed r)
        {
            float value = 0f;

            for (int d = 2; d < degree; d++)
            {
                int range = Mathf.FloorToInt(Mathf.Pow(2f, degree - d));
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
			return value;
        }
	}
}