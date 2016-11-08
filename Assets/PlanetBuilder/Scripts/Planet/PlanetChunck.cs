using UnityEngine;
using UnityEditor;
using System.Collections;

using System.Collections.Generic;
using System;

namespace SvenFrankson.Game.SphereCraft {

	public class PlanetChunck : MonoBehaviour {

		public enum CubeFace {
			Top,
			Side,
			Bottom
		}

        public int Degree
        {
            get
            {
                return this.planetSide.Degree;
            }
        }
        public int WaterLevel
        {
            get
            {
                return this.planetSide.WaterLevel;
            }
        }
        public string PlanetName
        {
            get
            {
                return this.planetSide.PlanetName;
            }
        }
		public PlanetSide planetSide;
        public int iPos;
        public int jPos;
        public int kPos;
        public Byte[][][] data;
        public Byte Data(int i, int j, int k)
        {
            return this.data[i][j][k];
        }
        public void SetData(Byte b, int i, int j, int k)
        {
            this.data[i][j][k] = b;
        }
        private Vector3 localUp = Vector3.up;
        public Vector3 LocalUp
        {
            get
            {
                return localUp;
            }
        }
        public float AngleToReferential;
        private bool meshSet = false;
        public bool MeshSet
        {
            get
            {
                return meshSet;
            }
        }

        public bool DebugMode = false;

        public void Update()
        {
            if (DebugMode)
            {
                Debug.DrawLine(this.transform.position, this.transform.TransformVector(this.LocalUp) * 200, Color.red);
            }
        }

        public void Initialize()
        {
            PlanetChunckManager.Instances.Add(this);

            this.transform.parent = this.planetSide.transform;
            this.name = "Chunck_" + this.iPos + "|" + this.jPos + "|" + this.kPos;
            this.transform.localPosition = Vector3.zero;
            this.transform.localRotation = Quaternion.identity;
            this.transform.localScale = Vector3.one;

            int y = PlanetUtility.ChunckSize / 2 + this.jPos * PlanetUtility.ChunckSize;
            int z = PlanetUtility.ChunckSize / 2 + this.iPos * PlanetUtility.ChunckSize;
            this.localUp = PlanetUtility.EvaluateVertex(this.planetSide.Size, y, z).normalized;
            this.AngleToReferential = Vector3.Angle(PlanetChunckManager.Instance.Referential.position - this.transform.position, this.transform.TransformVector(this.LocalUp));
		}

        public void BuildMeshAlt (out Mesh mesh, out Mesh meshCollider)
        {
            int size = this.planetSide.Size;
            int rMin = this.planetSide.RMin;
            float rWater = WaterLevel - 0.2f;

            mesh = new Mesh();
            List<Vector3> vertices = new List<Vector3>();
            List<Vector2> uvs = new List<Vector2>();
            List<int> trianglesTop = new List<int>();
            List<int> trianglesSide = new List<int>();
            List<int> trianglesBottom = new List<int>();
            List<int> trianglesWater = new List<int>();

            for (int i = 0; i < PlanetUtility.ChunckSize; i++)
            {
                for (int j = 0; j < PlanetUtility.ChunckSize; j++)
                {
                    for (int k = 0; k < PlanetUtility.ChunckSize; k++)
                    {
                        int y = j + this.jPos * PlanetUtility.ChunckSize;
                        int z = i + this.iPos * PlanetUtility.ChunckSize;
                        if ((k + this.kPos * PlanetUtility.ChunckSize) == WaterLevel)
                        {
                            if (this.data[i][j][k] == 0)
                            {
                                int a = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z) * (rWater + rMin));
                                int b = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z) * (rWater + rMin));
                                int c = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z + 1) * (rWater + rMin));
                                int d = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z + 1) * (rWater + rMin));

                                trianglesWater.Add(a);
                                trianglesWater.Add(b);
                                trianglesWater.Add(c);

                                trianglesWater.Add(a);
                                trianglesWater.Add(c);
                                trianglesWater.Add(d);

                                AddUVWater(uvs);
                            }
                        }
                        Byte block = this.data[i][j][k];
                        if (block != 0)
                        {
                            if ((i - 1 < 0) || (this.data[i - 1][j][k] < 128 && this.data[i - 1][j][k] != block))
                            {
                                int a = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int b = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int c = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int d = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));

                                trianglesSide.Add(a);
                                trianglesSide.Add(b);
                                trianglesSide.Add(c);

                                trianglesSide.Add(a);
                                trianglesSide.Add(c);
                                trianglesSide.Add(d);

                                PlanetUtility.AddUV(uvs, block);
                            }
                            if ((j - 1 < 0) || (this.data[i][j - 1][k] < 128 && this.data[i][j - 1][k] != block))
                            {
                                int a = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int b = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int c = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z + 1) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int d = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z + 1) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));

                                trianglesSide.Add(a);
                                trianglesSide.Add(b);
                                trianglesSide.Add(c);

                                trianglesSide.Add(a);
                                trianglesSide.Add(c);
                                trianglesSide.Add(d);

                                PlanetUtility.AddUV(uvs, block);
                            }
                            if ((k - 1 < 0) || (this.data[i][j][k - 1] < 128 && this.data[i][j][k - 1] != block))
                            {
                                int a = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int b = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z + 1) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int c = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z + 1) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int d = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));

                                trianglesTop.Add(a);
                                trianglesTop.Add(b);
                                trianglesTop.Add(c);

                                trianglesTop.Add(a);
                                trianglesTop.Add(c);
                                trianglesTop.Add(d);

                                PlanetUtility.AddUV(uvs, block);
                            }
                            if ((i + 1 >= PlanetUtility.ChunckSize) || (this.data[i + 1][j][k] < 128 && this.data[i + 1][j][k] != block))
                            {
                                int a = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z + 1) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int b = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z + 1) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int c = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z + 1) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int d = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z + 1) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));

                                trianglesSide.Add(a);
                                trianglesSide.Add(b);
                                trianglesSide.Add(c);

                                trianglesSide.Add(a);
                                trianglesSide.Add(c);
                                trianglesSide.Add(d);

                                PlanetUtility.AddUV(uvs, block);
                            }
                            if ((j + 1 >= PlanetUtility.ChunckSize) || (this.data[i][j + 1][k] < 128 && this.data[i][j + 1][k] != block))
                            {
                                int a = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z + 1) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int b = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z + 1) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int c = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int d = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z) * (k + this.kPos * PlanetUtility.ChunckSize + rMin));

                                trianglesSide.Add(a);
                                trianglesSide.Add(b);
                                trianglesSide.Add(c);

                                trianglesSide.Add(a);
                                trianglesSide.Add(c);
                                trianglesSide.Add(d);

                                PlanetUtility.AddUV(uvs, block);
                            }
                            if ((k + 1 >= PlanetUtility.ChunckSize) || (this.data[i][j][k + 1] < 128 && this.data[i][j][k + 1] != block))
                            {
                                int a = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int b = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int c = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y + 1, z + 1) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));
                                int d = vertices.Count;
                                vertices.Add(PlanetUtility.EvaluateVertex(size, y, z + 1) * (k + 1 + this.kPos * PlanetUtility.ChunckSize + rMin));

                                trianglesTop.Add(a);
                                trianglesTop.Add(b);
                                trianglesTop.Add(c);

                                trianglesTop.Add(a);
                                trianglesTop.Add(c);
                                trianglesTop.Add(d);

                                PlanetUtility.AddUV(uvs, block);
                            }
                        }
                    }
                }
            }

            mesh.vertices = vertices.ToArray();
            mesh.uv = uvs.ToArray();
            mesh.subMeshCount = 4;
            mesh.SetTriangles(trianglesTop, 0);
            mesh.SetTriangles(trianglesSide, 1);
            mesh.SetTriangles(trianglesBottom, 2);
            mesh.SetTriangles(trianglesWater, 3);
            mesh.RecalculateNormals();

            meshCollider = new Mesh();
            meshCollider.vertices = vertices.ToArray();
            List<int> meshColliderTriangles = new List<int>(trianglesTop);
            meshColliderTriangles.AddRange(trianglesSide);
            meshColliderTriangles.AddRange(trianglesBottom);
            meshCollider.triangles = meshColliderTriangles.ToArray();
        }

        private void AddUVWater(List<Vector2> uvs)
        {
            Vector2 uvA = new Vector2(0f, 0f);
            Vector2 uvB = new Vector2(0f, 1f);
            Vector2 uvC = new Vector2(1f, 1f);
            Vector2 uvD = new Vector2(1f, 0f);

            uvs.Add(uvA);
            uvs.Add(uvB);
            uvs.Add(uvC);
            uvs.Add(uvD);
        }

		public void SetMesh (bool async = true) {
            PlanetChunckManager.Instance.workingLock = true;
            if (async)
            {
                StartCoroutine(SetMeshAsync());
            }
            else
            {
                if (this.data == null)
                {
                    this.data = PlanetUtility.Read(this.PlanetName, iPos, jPos, kPos, this.planetSide.side);
                }

                Mesh mesh;
                Mesh meshCollider;
                this.BuildMeshAlt(out mesh, out meshCollider);

                this.GetComponent<MeshFilter>().sharedMesh = mesh;

                this.GetComponent<MeshRenderer>().materials = this.planetSide.Materials;

                if (this.GetComponent<MeshCollider>())
                {
                    this.GetComponent<MeshCollider>().sharedMesh = meshCollider;
                }

                this.meshSet = true;
            }
		}

        IEnumerator SetMeshAsync()
        {
            if (this.data == null)
            {
                this.data = PlanetUtility.Read(this.PlanetName, iPos, jPos, kPos, this.planetSide.side);
                yield return null;
            }

            Mesh mesh;
            Mesh meshCollider;
            this.BuildMeshAlt(out mesh, out meshCollider);

            this.GetComponent<MeshFilter>().sharedMesh = mesh;

            this.GetComponent<MeshRenderer>().materials = this.planetSide.Materials;
            yield return null;

            if (this.GetComponent<MeshCollider>())
            {
                this.GetComponent<MeshCollider>().sharedMesh = meshCollider;
            }

            this.meshSet = true;
            PlanetChunckManager.Instance.workingLock = false;
        }
	}
}
