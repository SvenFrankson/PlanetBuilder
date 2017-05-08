using UnityEngine;
using UnityEditor;
using System.IO;
using System.IO.Compression;
using System.Collections;
using SvenFrankson.Game.SphereCraft;
using System;

namespace SvenFrankson.Tools {

	public enum BlockData {
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
        public int degree = 7;
        public int waterPercent = 50;
        public int size = 0;
        public int chunckCount = 0;
        public bool overwrite = false;
        public bool babylonJSVersion = false;
        public string output = "";
        private float[][] thresholds;

        private class Map
        {
            public int[][] heightMap;
            public float[][] latMap;

            public Map()
            {
                this.heightMap = new int[PlanetUtility.ChunckSize][];
                this.latMap = new float[PlanetUtility.ChunckSize][];
                for (int i = 0; i < PlanetUtility.ChunckSize; i++)
                {
                    this.heightMap[i] = new int[PlanetUtility.ChunckSize];
                    this.latMap[i] = new float[PlanetUtility.ChunckSize];
                }
            }
        }

        private void ComputeProperties()
        {
            this.size = Mathf.FloorToInt(Mathf.Pow(2f, this.degree));
            this.chunckCount = this.size / PlanetUtility.ChunckSize;
            this.thresholds = new float[4][];
            for (int i = 0; i < 4; i++)
            {
                this.thresholds[i] = new float[2];
            }
            // Snow
            this.thresholds[0][0] = -1.1f / 90f;
            this.thresholds[0][1] = 1.5f;
            // Rock
            this.thresholds[1][0] = -0.8f / 90f;
            this.thresholds[1][1] = 1.1f;
            // Grass
            this.thresholds[2][0] = -0.1f / 90f;
            this.thresholds[2][1] = 0.6f;
            // Sand
            this.thresholds[3][0] = 0f;
            this.thresholds[3][1] = 0.2f;
        }

        [MenuItem("Window/PlanetDataGenerator")]
		static void Open () {
            EditorWindow.GetWindow<PlanetDataGenerator>();
		}

		public void OnGUI () {
            EditorGUILayout.LabelField("PlanetDataGenerator");
            this.planetName = EditorGUILayout.TextField("PlanetName", this.planetName);
            this.degree = EditorGUILayout.IntField("Degree", this.degree);
            this.waterPercent = EditorGUILayout.IntField("WaterHeight", this.waterPercent);
            EditorGUILayout.IntField("Size", this.size);
            EditorGUILayout.IntField("ChunckCount", this.chunckCount);
            this.overwrite = EditorGUILayout.Toggle("Overwrite", this.overwrite);
            this.babylonJSVersion = EditorGUILayout.Toggle("BabylonJS Version", this.babylonJSVersion);
            EditorGUILayout.TextArea(this.output);

			if (GUILayout.Button ("Compute Properties")) {

			}
			if (GUILayout.Button ("Create CubeMap")) {
                Check();
            }
            if (GUI.changed)
            {
                this.ComputeProperties();
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
            dataStream.WriteLine("#degree=" + this.degree);
            int waterLevel = this.size / 4;
            dataStream.WriteLine("#waterLevel=" + waterLevel);

            dataStream.Close();
            saveFile.Close();

            return 1;
        }

        private void Check()
        {
            this.output = "";
            this.ComputeProperties();
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
            RandomSeed seed = new RandomSeed(this.planetName);
            int chunckSaved = 0;
            int chunckTotal = chunckCount * chunckCount * chunckCount / 2 * 6;

            foreach (Planet.Side side in Enum.GetValues(typeof(Planet.Side)))
            {
                for (int i = 0; i < this.chunckCount; i++)
                {
                    for (int j = 0; j < this.chunckCount; j++)
                    {
                        Map map = GetHeightMapFor(seed, i, j, side);
                        for (int k = 0; k < this.chunckCount / 2; k++)
                        {
                            float t2 = Time.realtimeSinceStartup;
                            Byte[][][] chunckData = GetByteFor(i, j, k, side, map);
                            float t3 = Time.realtimeSinceStartup;
                            tGet += (t3 - t2);

                            float t4 = Time.realtimeSinceStartup;
                            if (this.babylonJSVersion)
                            {
                                PlanetUtility.SaveForBabylonJSVersion(this.planetName, chunckData, i, j, k, side);
                            }
                            else
                            {
                                PlanetUtility.Save(this.planetName, chunckData, i, j, k, side);
                            }
                            float t5 = Time.realtimeSinceStartup;
                            tSave += (t5 - t4);

                            chunckSaved++;
                            EditorUtility.DisplayProgressBar("Planet Data Generator", "Saving chunck datas for side " + side + " (" + ((int) side + 1) + "/6)...", (float) chunckSaved / (float) chunckTotal);
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
        }

        private float tBuildHeightMap = 0f;
        private float tSetBytes = 0f;

        private Map GetHeightMapFor(RandomSeed seed, int iPos, int jPos, Planet.Side side)
        {
            int maxHeight = this.chunckCount / 2 * PlanetUtility.ChunckSize / 2;
            int x, y, z;
            Map map = new Map();

            float t0 = Time.realtimeSinceStartup;
            if (side == Planet.Side.Top)
            {
                y = size;
                for (int i = 0; i < PlanetUtility.ChunckSize; i++)
                {
                    for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                    {
                        x = size - (j + jPos * PlanetUtility.ChunckSize);
                        z = i + iPos * PlanetUtility.ChunckSize;
                        map.heightMap[i][j] = Mathf.FloorToInt(EvaluateTriCubic(x, y, z, seed) * maxHeight + maxHeight / 2);
                        map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
                    }
                }
            }
            else if (side == Planet.Side.Bottom)
            {
                y = 0;
                for (int i = 0; i < PlanetUtility.ChunckSize; i++)
                {
                    for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                    {
                        x = j + jPos * PlanetUtility.ChunckSize;
                        z = i + iPos * PlanetUtility.ChunckSize;
                        map.heightMap[i][j] = Mathf.FloorToInt(EvaluateTriCubic(x, y, z, seed) * maxHeight + maxHeight / 2);
                        map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
                    }
                }
            }
            else if (side == Planet.Side.Right)
            {
                x = size;
                for (int i = 0; i < PlanetUtility.ChunckSize; i++)
                {
                    for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                    {
                        y = j + jPos * PlanetUtility.ChunckSize;
                        z = i + iPos * PlanetUtility.ChunckSize;
                        map.heightMap[i][j] = Mathf.FloorToInt(EvaluateTriCubic(x, y, z, seed) * maxHeight + maxHeight / 2);
                        map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
                    }
                }
            }
            else if (side == Planet.Side.Left)
            {
                x = 0;
                for (int i = 0; i < PlanetUtility.ChunckSize; i++)
                {
                    for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                    {
                        y = j + jPos * PlanetUtility.ChunckSize;
                        z = size - (i + iPos * PlanetUtility.ChunckSize);
                        map.heightMap[i][j] = Mathf.FloorToInt(EvaluateTriCubic(x, y, z, seed) * maxHeight + maxHeight / 2);
                        map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
                    }
                }
            }
            else if (side == Planet.Side.Front)
            {
                z = size;
                for (int i = 0; i < PlanetUtility.ChunckSize; i++)
                {
                    for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                    {
                        y = j + jPos * PlanetUtility.ChunckSize;
                        x = size - (i + iPos * PlanetUtility.ChunckSize);
                        map.heightMap[i][j] = Mathf.FloorToInt(EvaluateTriCubic(x, y, z, seed) * maxHeight + maxHeight / 2);
                        map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
                    }
                }
            }
            else if (side == Planet.Side.Back)
            {
                z = 0;
                for (int i = 0; i < PlanetUtility.ChunckSize; i++)
                {
                    for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                    {
                        y = j + jPos * PlanetUtility.ChunckSize;
                        x = i + iPos * PlanetUtility.ChunckSize;
                        map.heightMap[i][j] = Mathf.FloorToInt(EvaluateTriCubic(x, y, z, seed) * maxHeight + maxHeight / 2);
                        map.latMap[i][j] = Mathf.Abs(Vector3.Angle(PlanetUtility.EvaluateVertex(size, x, y, z), Vector3.up) - 90f);
                    }
                }
            }
            float t1 = Time.realtimeSinceStartup;
            tBuildHeightMap += t1 - t0;

            return map;
        }

        private Byte[][][] GetByteFor(int iPos, int jPos, int kPos, Planet.Side side, Map map)
        {
            Byte[][][] chunckData = new Byte[PlanetUtility.ChunckSize][][];
            int maxHeight = this.chunckCount / 2 * PlanetUtility.ChunckSize / 2;

            float t2 = Time.realtimeSinceStartup;
            for (int i = 0; i < PlanetUtility.ChunckSize; i++)
            {
                chunckData[i] = new Byte[PlanetUtility.ChunckSize][];
                for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                {
                    chunckData[i][j] = new Byte[PlanetUtility.ChunckSize];
                    for (int k = 0; k < PlanetUtility.ChunckSize; k++)
                    {
                        int h = kPos * PlanetUtility.ChunckSize + k;
                        if (h == 0)
                        {
							chunckData[i][j][k] = (byte) BlockData.Rock;
                        }
                        else if (h <= map.heightMap[i][j])
                        {
                            int[] localThresholds = GetLocalThresholds(maxHeight, map.latMap[i][j]);
                            if (h >= localThresholds[0])
                            {
								chunckData[i][j][k] = (byte) BlockData.Snow;
                            }
                            else if (h >= localThresholds[1])
                            {
								chunckData[i][j][k] = (byte) BlockData.Rock;
                            }
                            else if (h >= localThresholds[2])
                            {
                                if (h == map.heightMap[i][j])
                                {
									chunckData[i][j][k] = (byte) BlockData.Grass;
                                }
                                else
                                {
									chunckData[i][j][k] = (byte) BlockData.Dirt;
                                }
                            }
                            else if (h >= localThresholds[3])
                            {
								chunckData[i][j][k] = (byte) BlockData.Sand;
                            }
                            else
                            {
								chunckData[i][j][k] = (byte) BlockData.Rock;
                            }
                        }
                        else
                        {
                            chunckData[i][j][k] = 0;
                        }
                    }
                }
            }
            float t3 = Time.realtimeSinceStartup;
            tSetBytes += t3 - t2;

            return chunckData;
        }

        private int[] GetLocalThresholds(int maxHeight, float lat)
        {
            int[] localThresholds = new int[4];
            for (int i = 0; i < 4; i++)
            {
                localThresholds[i] = Mathf.FloorToInt((thresholds[i][0] * lat + thresholds[i][1]) * maxHeight + maxHeight / 2) + UnityEngine.Random.Range(-2, 0);
            }
            return localThresholds;
        }

        private float TERP(float t, float a, float b, float c, float d)
        {
            return 0.5f * (c - a + (2.0f * a - 5.0f * b + 4.0f * c - d + (3.0f * (b - c) + d - a) * t) * t) * t + b;
        }

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
            return (value + 1.5f) / 3f;
        }
	}
}