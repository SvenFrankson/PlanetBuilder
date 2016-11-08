using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

namespace SvenFrankson.Game.SphereCraft {

    public class Planet : MonoBehaviour
    {
        public enum Side
        {
            Up = 0,
            Right = 1,
            Forward = 2,
            Down = 3,
            Left = 4,
            Back = 5
        };

        [HideInInspector]
        public string planetName;
		public int degree;
        public int size;
		public int waterLevel;
        public int rMin;
		public Material[] planetMaterials;

        public Dictionary<Side, PlanetSide> planetSides = new Dictionary<Side,PlanetSide>();

		public void Start () {
            this.Initialize();

            foreach (PlanetSide planetSide in planetSides.Values)
            {
                planetSide.Initialize(this);
            }
		}

        public void Initialize()
        {
            Debug.Log("Planet Initialize");
            PlanetSide[] planetSidesInChildren = this.GetComponentsInChildren<PlanetSide>();

            this.ReadPlanetInfoFile();
            this.planetSides = new Dictionary<Planet.Side, PlanetSide>();
            this.size = Mathf.FloorToInt(Mathf.Pow(2f, this.degree));
            this.rMin = Mathf.FloorToInt((2 / Mathf.PI - 1 / 8f) * size);

            // Search for PlanetSide in Children. If any cannot be found, it is instantiated.
            foreach (Side side in Enum.GetValues(typeof(Planet.Side))) {

                bool instantiatePlanetSide = true;
                PlanetSide newPlanetSide = null;

                // Search for an already existing planetSide.
                foreach (PlanetSide planetSide in planetSidesInChildren)
                {
                    if (planetSide.side == side)
                    {
                        newPlanetSide = planetSide;
                        instantiatePlanetSide = false;
                    }
                }

                // If no existing PlanetSide has been found, create one.
                if (instantiatePlanetSide)
                {
                    GameObject newSideGameObject = new GameObject();
                    newSideGameObject.name = side.ToString();
                    newSideGameObject.transform.parent = this.transform;
                    newSideGameObject.transform.localPosition = Vector3.zero;
                    newSideGameObject.transform.localRotation = PlanetUtility.LocalRotationFromSide(side);

                    newPlanetSide = newSideGameObject.AddComponent<PlanetSide>();
                    newPlanetSide.side = side;
                }

                this.planetSides.Add(side, newPlanetSide);
            }
		}
        
        public void Clear()
        {
            while (this.transform.childCount > 0)
            {
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            }
            if (this.planetSides != null)
            {
                this.planetSides.Clear();
            }
        }

        public PlanetSide WorldPositionToPlanetSide(Vector3 worldPos)
        {
            Vector3 localPos = this.transform.worldToLocalMatrix * worldPos;

            float[] angles = new float[6];
            angles[(int)Planet.Side.Right] = Vector3.Angle(localPos, Vector3.right);
            angles[(int)Planet.Side.Left] = Vector3.Angle(localPos, -Vector3.right);
            angles[(int)Planet.Side.Forward] = Vector3.Angle(localPos, Vector3.forward);
            angles[(int)Planet.Side.Back] = Vector3.Angle(localPos, -Vector3.forward);
            angles[(int)Planet.Side.Up] = Vector3.Angle(localPos, Vector3.up);
            angles[(int)Planet.Side.Down] = Vector3.Angle(localPos, -Vector3.up);

            Planet.Side smallest = Planet.Side.Up;
            foreach (Planet.Side side in Enum.GetValues(typeof(Planet.Side)))
            {
                if (angles[(int)side] < angles[(int)smallest])
                {
                    smallest = side;
                }
            }

            return this.planetSides[smallest];
        }

        public void WorldPositionToIJK(Vector3 worldPos, out PlanetChunck planetChunck, out int i, out int j, out int k)
        {
            PlanetSide planetSide;
            int iPos, jPos, kPos;
            WorldPositionToIJKPos(worldPos, out planetSide, out iPos, out jPos, out kPos);

            planetChunck = planetSide.chuncks[iPos / PlanetUtility.ChunckSize][jPos / PlanetUtility.ChunckSize][kPos / PlanetUtility.ChunckSize];
            i = iPos % PlanetUtility.ChunckSize;
            j = jPos % PlanetUtility.ChunckSize;
            k = kPos % PlanetUtility.ChunckSize;
        }

        public void WorldPositionToIJKPos(Vector3 worldPos, out PlanetSide planetSide, out int iPos, out int jPos, out int kPos)
        {
            planetSide = WorldPositionToPlanetSide(worldPos);
            Vector3 localPos = planetSide.transform.worldToLocalMatrix * worldPos;
            float r = localPos.magnitude;

            if (Mathf.Abs(localPos.x) > 1f)
            {
                localPos = localPos / localPos.x;
            }
            if (Mathf.Abs(localPos.y) > 1f)
            {
                localPos = localPos / localPos.y;
            }
            if (Mathf.Abs(localPos.z) > 1f)
            {
                localPos = localPos / localPos.z;
            }

            float tanX = localPos.x;
            float tanY = localPos.y;
            float tanZ = localPos.z;

            float xDeg = Mathf.Atan(tanX);
            float yDeg = Mathf.Atan(tanY);
            float zDeg = Mathf.Atan(tanZ);

            xDeg = Mathf.Rad2Deg * xDeg;
            yDeg = Mathf.Rad2Deg * yDeg;
            zDeg = Mathf.Rad2Deg * zDeg;

            iPos = Mathf.FloorToInt((zDeg + 45f) / 90f * planetSide.Size);
            jPos = Mathf.FloorToInt((yDeg + 45f) / 90f * planetSide.Size);
            kPos = Mathf.FloorToInt(r - rMin);
        }

        public Byte WorldPositionToData(Vector3 worldPos)
        {
            PlanetChunck planetChunck;
            int i, j, k;
            this.WorldPositionToIJK(worldPos, out planetChunck, out i, out j, out k);

            return planetChunck.Data(i, j, k);
        }

        public Mesh WorldPositionToBlockMesh(int iPos, int jPos, int kPos, Byte block)
        {
            int size = this.size;
            int rMin = this.rMin;

            Mesh m = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> trianglesTop = new List<int>();
            List<int> trianglesSide = new List<int>();
            List<int> trianglesBottom = new List<int>();

            int a = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos) * (kPos + rMin));
            int b = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos) * (kPos + 1 + rMin));
            int c = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos) * (kPos + 1 + rMin));
            int d = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos) * (kPos + rMin));

            trianglesSide.Add(a);
            trianglesSide.Add(b);
            trianglesSide.Add(c);

            trianglesSide.Add(a);
            trianglesSide.Add(c);
            trianglesSide.Add(d);

            PlanetUtility.AddUV(uvs, block);
                
            a = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos) * (kPos + rMin));
            b = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos) * (kPos + 1 + rMin));
            c = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos + 1) * (kPos + 1 + rMin));
            d = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos + 1) * (kPos + rMin));

            trianglesSide.Add(a);
            trianglesSide.Add(b);
            trianglesSide.Add(c);

            trianglesSide.Add(a);
            trianglesSide.Add(c);
            trianglesSide.Add(d);

            PlanetUtility.AddUV(uvs, block);
                
            a = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos) * (kPos + rMin));
            b = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos + 1) * (kPos + rMin));
            c = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos + 1) * (kPos + rMin));
            d = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos) * (kPos + rMin));

            trianglesTop.Add(a);
            trianglesTop.Add(b);
            trianglesTop.Add(c);

            trianglesTop.Add(a);
            trianglesTop.Add(c);
            trianglesTop.Add(d);

            PlanetUtility.AddUV(uvs, block);
                
            a = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos + 1) * (kPos + rMin));
            b = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos + 1) * (kPos + 1 + rMin));
            c = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos + 1) * (kPos + 1 + rMin));
            d = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos + 1) * (kPos + rMin));

            trianglesSide.Add(a);
            trianglesSide.Add(b);
            trianglesSide.Add(c);

            trianglesSide.Add(a);
            trianglesSide.Add(c);
            trianglesSide.Add(d);

            PlanetUtility.AddUV(uvs, block);
                
            a = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos + 1) * (kPos + rMin));
            b = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos + 1) * (kPos + 1 + rMin));
            c = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos) * (kPos + 1 + rMin));
            d = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos) * (kPos + rMin));

            trianglesSide.Add(a);
            trianglesSide.Add(b);
            trianglesSide.Add(c);

            trianglesSide.Add(a);
            trianglesSide.Add(c);
            trianglesSide.Add(d);

            PlanetUtility.AddUV(uvs, block);
                
            a = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos) * (kPos + 1 + rMin));
            b = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos) * (kPos + 1 + rMin));
            c = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos + 1, iPos + 1) * (kPos + 1 + rMin));
            d = vertices.Count;
            vertices.Add(PlanetUtility.EvaluateVertex(size, jPos, iPos + 1) * (kPos + 1 + rMin));

            trianglesTop.Add(a);
            trianglesTop.Add(b);
            trianglesTop.Add(c);

            trianglesTop.Add(a);
            trianglesTop.Add(c);
            trianglesTop.Add(d);

            PlanetUtility.AddUV(uvs, block);

            m.vertices = vertices.ToArray();
            m.subMeshCount = 3;
            m.SetTriangles(trianglesTop, 0);
            m.SetTriangles(trianglesSide, 1);
            m.SetTriangles(trianglesBottom, 2);
            m.uv = uvs.ToArray();
            m.RecalculateNormals();

            if (block == 0)
            {
                Vector3[] eraserVertices = new Vector3[m.vertices.Length];
                for (int i = 0; i < m.vertices.Length; i++)
                {
                    eraserVertices[i] = m.vertices[i] + m.normals[i] * 0.05f;
                }
                m.vertices = eraserVertices;
            }

            return m;
        }

        public PlanetChunck SetDataAtWorldPos(Vector3 worldPos, Byte data, bool ifZero = false, bool rebuild = true, bool save = true)
        {
            PlanetChunck planetChunck;
            int i, j, k;
            this.WorldPositionToIJK(worldPos, out planetChunck, out i, out j, out k);
            if ((ifZero && planetChunck.data[i][j][k] == 0) || !ifZero)
            {
                planetChunck.SetData(data, i, j, k);
            }
            else
            {
                return null;
            }
            if (rebuild)
            {
                planetChunck.SetMesh();
            }
            if (save)
            {
                PlanetUtility.Save(planetChunck.PlanetName, planetChunck.data, planetChunck.iPos, planetChunck.jPos, planetChunck.kPos, planetChunck.planetSide.side);
            }
            return planetChunck;
        }

        public PlanetChunck SetDataAtIJKPos(PlanetSide planetSide, int iPos, int jPos, int kPos, Byte data, bool ifZero = false, bool rebuild = true, bool save = true)
        {
            PlanetChunck planetChunck = planetSide.chuncks[iPos / PlanetUtility.ChunckSize][jPos / PlanetUtility.ChunckSize][kPos / PlanetUtility.ChunckSize];
            if ((ifZero && planetChunck.data[iPos % PlanetUtility.ChunckSize][jPos % PlanetUtility.ChunckSize][kPos % PlanetUtility.ChunckSize] == 0) || !ifZero)
            {
                planetChunck.SetData(data, iPos % PlanetUtility.ChunckSize, jPos % PlanetUtility.ChunckSize, kPos % PlanetUtility.ChunckSize);
            }
            if (rebuild)
            {
                planetChunck.SetMesh();
            }
            if (save)
            {
                PlanetUtility.Save(planetChunck.PlanetName, planetChunck.data, planetChunck.iPos, planetChunck.jPos, planetChunck.kPos, planetChunck.planetSide.side);
            }
            return planetChunck;
        }

        public void ReadPlanetInfoFile()
        {
            string directoryPath = Application.dataPath + "/../PlanetData/" + this.planetName + "/";
            string dataFilePath = directoryPath + "/" + this.planetName + ".info";
            FileStream dataFile = new FileStream(dataFilePath, FileMode.Open, FileAccess.Read);
            StreamReader dataStream = new StreamReader(dataFile);

            dataStream.ReadLine();
            string l = dataStream.ReadLine();
            Debug.Log(l);
            this.degree = int.Parse(l.Split('=')[1]);
            l = dataStream.ReadLine();
            this.waterLevel = int.Parse(l.Split('=')[1]);

            dataStream.Close();
            dataFile.Close();
        }
	}
}