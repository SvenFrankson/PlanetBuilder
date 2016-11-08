using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

namespace SvenFrankson.Game.SphereCraft {
	
	public class PlanetUtility {

		static public int ChunckSize {
			get {
				return 32;
			}
		}

		static public PlanetChunck InstantiatePlanetChunck (int iPos, int jPos, int kPos, PlanetSide planetSide) {
            GameObject planetChunckInstance = new GameObject();

			PlanetChunck planetChunck = planetChunckInstance.AddComponent<PlanetChunck> ();
            planetChunck.iPos = iPos;
            planetChunck.jPos = jPos;
            planetChunck.kPos = kPos;
            planetChunck.planetSide = planetSide;

			planetChunckInstance.AddComponent<MeshFilter> ();
			MeshRenderer planetChunckMeshRenderer = planetChunckInstance.AddComponent<MeshRenderer> ();
			planetChunckMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On;
			planetChunckMeshRenderer.receiveShadows = true;

            planetChunckInstance.AddComponent<MeshCollider>();

			return planetChunck;
		}

        static public string ChunckName(int iPos, int jPos, int kPos)
        {
            return "Chunck_" + iPos + "|" + jPos + "|" + kPos;
        }

        static public int SideNameToIndex(string sideName)
        {
            int sideIndex = -1;

            switch (sideName) {
                case "Up" :
                    sideIndex = 0;
                    break;
                case "Down":
                    sideIndex = 3;
                    break;
                case "Right":
                    sideIndex = 1;
                    break;
                case "Left":
                    sideIndex = 4;
                    break;
                case "Forward":
                    sideIndex = 2;
                    break;
                case "Back":
                    sideIndex = 5;
                    break;                
            }
            return sideIndex;
        }

        static public string SideIndexToName(int sideIndex)
        {
            string sideName = "ERROR_SIDENAME";

            switch (sideIndex)
            {
                case 0:
                    sideName = "Up";
                    break;
                case 3:
                    sideName = "Down";
                    break;
                case 1:
                    sideName = "Right";
                    break;
                case 4:
                    sideName = "Left";
                    break;
                case 2:
                    sideName = "Forward";
                    break;
                case 5:
                    sideName = "Back";
                    break;
            }

            return sideName;
        }

        static public Quaternion LocalRotationFromSide(Planet.Side side)
        {
            Quaternion sideLocalRotation = Quaternion.identity;

            switch (side)
            {
                case Planet.Side.Up:
                    sideLocalRotation = Quaternion.Euler(0f, 0f, 90f);
                    break;
                case Planet.Side.Down:
                    sideLocalRotation = Quaternion.Euler(0f, 0f, -90f);
                    break;
                case Planet.Side.Right:
                    sideLocalRotation = Quaternion.Euler(0f, 0f, 0f);
                    break;
                case Planet.Side.Left:
                    sideLocalRotation = Quaternion.Euler(0f, 180f, 0f);
                    break;
                case Planet.Side.Forward:
                    sideLocalRotation = Quaternion.Euler(0f, -90f, 0f);
                    break;
                case Planet.Side.Back:
                    sideLocalRotation = Quaternion.Euler(0f, 90f, 0f);
                    break;
            }
            return sideLocalRotation;
        }

        static public Vector3 EvaluateVertex(int size, int i, int j)
        {
            float xRad = 45f;
            float yRad = -45f + 90f * (float)i / ((float)size);
            float zRad = -45f + 90f * (float)j / ((float)size);

            xRad = Mathf.Deg2Rad * xRad;
            yRad = Mathf.Deg2Rad * yRad;
            zRad = Mathf.Deg2Rad * zRad;

            return new Vector3(Mathf.Sin(xRad) / Mathf.Cos(xRad), Mathf.Sin(yRad) / Mathf.Cos(yRad), Mathf.Sin(zRad) / Mathf.Cos(zRad)).normalized;
        }

        static public Vector3 EvaluateVertex(int size, int x, int y, int z)
        {
            float xRad = -45f + 90f * (float)x / ((float)size);
            float yRad = -45f + 90f * (float)y / ((float)size);
            float zRad = -45f + 90f * (float)z / ((float)size);

            xRad = Mathf.Deg2Rad * xRad;
            yRad = Mathf.Deg2Rad * yRad;
            zRad = Mathf.Deg2Rad * zRad;

            return new Vector3(Mathf.Sin(xRad) / Mathf.Cos(xRad), Mathf.Sin(yRad) / Mathf.Cos(yRad), Mathf.Sin(zRad) / Mathf.Cos(zRad)).normalized;
        }

        static public void AddUV(List<Vector2> uvs, Byte block)
        {
            int tileSetSize = 8;
            int blockIndex = block - 1;
            if (block >= 128)
            {
                blockIndex = block - 128 + tileSetSize * tileSetSize / 2;
            }

            Vector2 uvA = new Vector2(1f / tileSetSize * (blockIndex % tileSetSize), 1f - 1f / tileSetSize * (blockIndex / tileSetSize + 1));
            Vector2 uvB = new Vector2(1f / tileSetSize * (blockIndex % tileSetSize), 1f - 1f / tileSetSize * (blockIndex / tileSetSize + 1) + 1f / tileSetSize);
            Vector2 uvC = new Vector2(1f / tileSetSize * (blockIndex % tileSetSize) + 1f / tileSetSize, 1f - 1f / tileSetSize * (blockIndex / tileSetSize + 1) + 1f / tileSetSize);
            Vector2 uvD = new Vector2(1f / tileSetSize * (blockIndex % tileSetSize) + 1f / tileSetSize, 1f - 1f / tileSetSize * (blockIndex / tileSetSize + 1));

            uvs.Add(uvA);
            uvs.Add(uvB);
            uvs.Add(uvC);
            uvs.Add(uvD);
        }

        static public int Save(string planet, Byte[][][] chunckDatas, int iPos, int jPos, int kPos, Planet.Side side)
        {
            string directoryPath = Application.dataPath + "/../PlanetData/" + planet + "/" + side;
            Directory.CreateDirectory(directoryPath);
            string saveFilePath = directoryPath + "/data_" + (int)side + "_" + iPos + "_" + jPos + "_" + kPos + ".pdat";
            FileStream saveFile = new FileStream(saveFilePath, FileMode.Create, FileAccess.Write);
            BinaryWriter dataStream = new BinaryWriter(saveFile);
            for (int i = 0; i < PlanetUtility.ChunckSize; i++)
            {
                for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                {
                    for (int k = 0; k < PlanetUtility.ChunckSize; k++)
                    {
                        dataStream.Write(chunckDatas[i][j][k]);
                    }
                }
            }
            dataStream.Close();
            saveFile.Close();

            return 1;
        }

        static public Byte[][][] Read(string planet, int iPos, int jPos, int kPos, Planet.Side side)
        {
            string directoryPath = Application.dataPath + "/../PlanetData/" + planet + "/" + side;
            string saveFilePath = directoryPath + "/data_" + (int)side + "_" + iPos + "_" + jPos + "_" + kPos + ".pdat";
            FileStream saveFile = new FileStream(saveFilePath, FileMode.Open);
            BinaryReader dataStream = new BinaryReader(saveFile);
            Byte[][][] chunckDatas = new Byte[PlanetUtility.ChunckSize][][];
            for (int i = 0; i < PlanetUtility.ChunckSize; i++)
            {
                chunckDatas[i] = new Byte[PlanetUtility.ChunckSize][];
                for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                {
                    chunckDatas[i][j] = new Byte[PlanetUtility.ChunckSize];
                    for (int k = 0; k < PlanetUtility.ChunckSize; k++)
                    {
                        chunckDatas[i][j][k] = dataStream.ReadByte();
                    }
                }
            }
            dataStream.Close();
            saveFile.Close();

            return chunckDatas;
        }

        static public string[] GetPlanetList()
        {
            string[] planetList = Directory.GetDirectories(Application.dataPath + "/../PlanetData", "*");
            for (int i = 0; i < planetList.Length; i++)
            {
                planetList[i] = planetList[i].Split('\\', '/')[planetList[i].Split('\\', '/').Length - 1];
            }
            return planetList;
        }
	}
}