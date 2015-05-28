using UnityEngine;
using System.Collections;

using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	public class PlanetChunck : MonoBehaviour {

		public enum CubeFace {
			Top,
			Side,
			Bottom
		}

		public PlanetSide planetSide;
		public Vector3 orientation;
		public Vector3 posInChunck;
		public int [] blocks;
		public Mesh meshCollider;

		public void Initialize (Vector3 orientation, Vector3 posInChunck, PlanetSide planetSide) {
			this.orientation = orientation;
			this.posInChunck = posInChunck;
			this.planetSide = planetSide;

			this.blocks = new int[PlanetUtility.ChunckSize * PlanetUtility.ChunckSize * 32];
			for (int i = 0; i < PlanetUtility.ChunckSize; i++) {
				for (int j = 0; j < PlanetUtility.ChunckSize; j++) {
					for (int k = 0; k < 32; k++) {
						this.blocks[i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = 0;
					}
				}
			}
		}

		public Mesh BuildMesh () {
			Mesh mesh = new Mesh ();
			mesh.subMeshCount = 2;

			this.meshCollider = new Mesh ();
			this.meshCollider.subMeshCount = 1;

			List<Vector3> vertices = new List<Vector3> ();
			List<int>[] triangles = new List<int>[2];
			for (int i = 0; i < 2; i++) {
				triangles[i] = new List<int> ();
			}
			List<Vector3> normals = new List<Vector3> ();
			List<Vector2> uv = new List<Vector2> ();

			int iOff = (int) this.posInChunck.x * PlanetUtility.ChunckSize;
			int jOff = (int) this.posInChunck.y * PlanetUtility.ChunckSize;
			int kOff = (int) this.posInChunck.z * PlanetUtility.ChunckSize;

			Vector3[][] baseMesh = PlanetUtility.GetBaseMesh (this.planetSide.subDegree).vertices;

			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;

			for (int i = 0; i < PlanetUtility.ChunckSize; i++) {
				for (int j = 0; j < PlanetUtility.ChunckSize; j++) {
					for (int k = 0; k < 32; k++) {
						if (this.blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] == 0) {
							if (k < this.planetSide.planet.waterLevel) {
								this.blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = 2;
							}
						}

						if (this.blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] != 0) {

							int blockType = this.blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize];

							bool isOnTop = false;

							if (k + 1 < 32) {
								if (this.blocks [i + j * PlanetUtility.ChunckSize + (k + 1) * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] == 0) {
									isOnTop = true;
								}
								else if (blockType == 2) {
									if (this.blocks [i + j * PlanetUtility.ChunckSize + (k + 1) * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] != 2) {
										isOnTop = true;
									}
								}
							}
							else if (k + 1 == 32) {
								isOnTop = true;
							}

							float blockHeight = 1f;
							if ((blockType == 2) && (isOnTop)) {
								blockHeight = 0.8f;
							}

							int subMesh = 0;
							if (blockType == 2) {
								subMesh = 1;
							}

							bool buildIMinus = false;

							if (blockType != 2) {
								int iMinusBlock;
								if (i > 0) {
									iMinusBlock = this.blocks [i - 1 + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize];
								}
								else if (i == 0) {
									iMinusBlock = this.planetSide.GetBlockFor (iOff + i - 1, jOff + j, kOff + k);
								}
								else {
									iMinusBlock = -1;
								}
								if (((iMinusBlock == 0) || (iMinusBlock == 2)) || (iMinusBlock == -1)) {
									buildIMinus = true;
								}
							}

							if (buildIMinus) {
								a = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j + 1] + (kOff + k) * baseMesh[iOff + i][jOff + j + 1].normalized);
								normals.Add ((baseMesh[iOff + i][jOff + j + 1] - baseMesh[iOff + i + 1][jOff + j + 1]).normalized);
								
								b = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j + 1] + (kOff + k + blockHeight) * baseMesh[iOff + i][jOff + j + 1].normalized);
								normals.Add ((baseMesh[iOff + i][jOff + j + 1] - baseMesh[iOff + i + 1][jOff + j + 1]).normalized);
								
								c = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j] + (kOff + k + blockHeight) * baseMesh[iOff + i][jOff + j].normalized);
								normals.Add ((baseMesh[iOff + i][jOff + j] - baseMesh[iOff + i + 1][jOff + j]).normalized);
								
								d = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j] + (kOff + k) * baseMesh[iOff + i][jOff + j].normalized);
								normals.Add ((baseMesh[iOff + i][jOff + j] - baseMesh[iOff + i + 1][jOff + j]).normalized);

								this.AddUV (uv, blockType, CubeFace.Side, isOnTop);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (b);
								triangles[subMesh].Add (c);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (c);
								triangles[subMesh].Add (d);
							}

							bool buildIPlus = false;

							if (blockType != 2) {
								int iPlusBlock;
								if (i + 1 < PlanetUtility.ChunckSize) {
									iPlusBlock = this.blocks [i + 1 + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize];
								}
								else if (i + 1 == PlanetUtility.ChunckSize) {
									iPlusBlock = this.planetSide.GetBlockFor (iOff + i + 1, jOff + j, kOff + k);
								}
								else {
									iPlusBlock = -1;
								}
								if (((iPlusBlock == 0) || (iPlusBlock == 2)) || (iPlusBlock == -1)) {
									buildIPlus = true;
								}
							}

							if (buildIPlus) {
								a = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j] + (kOff + k) * baseMesh[iOff + i + 1][jOff + j].normalized);
								normals.Add ((baseMesh[iOff + i + 1][jOff + j] - baseMesh[iOff + i][jOff + j]).normalized);
								
								b = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j] + (kOff + k + blockHeight) * baseMesh[iOff + i + 1][jOff + j].normalized);
								normals.Add ((baseMesh[iOff + i + 1][jOff + j] - baseMesh[iOff + i][jOff + j]).normalized);
								
								c = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j + 1] + (kOff + k + blockHeight) * baseMesh[iOff + i + 1][jOff + j + 1].normalized);
								normals.Add ((baseMesh[iOff + i + 1][jOff + j + 1] - baseMesh[iOff + i][jOff + j + 1]).normalized);
								
								d = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j + 1] + (kOff + k) * baseMesh[iOff + i + 1][jOff + j + 1].normalized);
								normals.Add ((baseMesh[iOff + i + 1][jOff + j + 1] - baseMesh[iOff + i][jOff + j + 1]).normalized);
								
								this.AddUV (uv, blockType, CubeFace.Side, isOnTop);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (b);
								triangles[subMesh].Add (c);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (c);
								triangles[subMesh].Add (d);
							}

							bool buildJMinus = false;

							if (blockType != 2) {
								int jMinusBlock;
								if (j > 0) {
									jMinusBlock = this.blocks [i + (j - 1) * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize];
								}
								else if (j == 0) {
									jMinusBlock = this.planetSide.GetBlockFor (iOff + i, jOff + j - 1, kOff + k);
								}
								else {
									jMinusBlock = -1;
								}
								if (((jMinusBlock == 0) || (jMinusBlock == 2)) || (jMinusBlock == -1)) {
									buildJMinus = true;
								}
							}

							if (buildJMinus) {
								a = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j] + (kOff + k) * baseMesh[iOff + i][jOff + j].normalized);
								normals.Add ((baseMesh[iOff + i][jOff + j] - baseMesh[iOff + i][jOff + j + 1]).normalized);
								
								b = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j] + (kOff + k + blockHeight) * baseMesh[iOff + i][jOff + j].normalized);
								normals.Add ((baseMesh[iOff + i][jOff + j] - baseMesh[iOff + i][jOff + j + 1]).normalized);
								
								c = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j] + (kOff + k + blockHeight) * baseMesh[iOff + i + 1][jOff + j].normalized);
								normals.Add ((baseMesh[iOff + i + 1][jOff + j] - baseMesh[iOff + i + 1][jOff + j + 1]).normalized);
								
								d = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j] + (kOff + k) * baseMesh[iOff + i + 1][jOff + j].normalized);
								normals.Add ((baseMesh[iOff + i + 1][jOff + j] - baseMesh[iOff + i + 1][jOff + j + 1]).normalized);
								
								this.AddUV (uv, blockType, CubeFace.Side, isOnTop);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (b);
								triangles[subMesh].Add (c);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (c);
								triangles[subMesh].Add (d);
							}
							
							bool buildJPlus = false;
							
							if (blockType != 2) {
								int jPlusBlock;
								if (j + 1 < PlanetUtility.ChunckSize) {
									jPlusBlock = this.blocks [i + (j + 1) * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize];
								}
								else if (j + 1 == PlanetUtility.ChunckSize) {
									jPlusBlock = this.planetSide.GetBlockFor (iOff + i, jOff + j + 1, kOff + k);
								}
								else {
									jPlusBlock = -1;
								}
								if (((jPlusBlock == 0) || (jPlusBlock == 2)) || (jPlusBlock == -1)) {
									buildJPlus = true;
								}
							}

							if (buildJPlus) {
								a = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j + 1] + (kOff + k) * baseMesh[iOff + i + 1][jOff + j + 1].normalized);
								normals.Add ((baseMesh[iOff + i + 1][jOff + j + 1] - baseMesh[iOff + i + 1][jOff + j]).normalized);
								
								b = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j + 1] + (kOff + k + blockHeight) * baseMesh[iOff + i + 1][jOff + j + 1].normalized);
								normals.Add ((baseMesh[iOff + i + 1][jOff + j + 1] - baseMesh[iOff + i + 1][jOff + j]).normalized);
								
								c = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j + 1] + (kOff + k + blockHeight) * baseMesh[iOff + i][jOff + j + 1].normalized);
								normals.Add ((baseMesh[iOff + i][jOff + j + 1] - baseMesh[iOff + i][jOff + j]).normalized);
								
								d = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j + 1] + (kOff + k) * baseMesh[iOff + i][jOff + j + 1].normalized);
								normals.Add ((baseMesh[iOff + i][jOff + j + 1] - baseMesh[iOff + i][jOff + j]).normalized);
								
								this.AddUV (uv, blockType, CubeFace.Side, isOnTop);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (b);
								triangles[subMesh].Add (c);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (c);
								triangles[subMesh].Add (d);
							}
							
							bool buildKMinus = false;

							if (blockType != 2) {
								if (k > 0) {
									if ((this.blocks [i + j * PlanetUtility.ChunckSize + (k - 1) * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] == 0) || (this.blocks [i + j * PlanetUtility.ChunckSize + (k - 1) * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] == 2)) {
										buildKMinus = true;
									}
								}
								else if (k == 0) {
									buildKMinus = false;
								}
							}

							if (buildKMinus) {
								a = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j + 1] + (kOff + k) * baseMesh[iOff + i][jOff + j + 1].normalized);
								normals.Add (-baseMesh[iOff + i][jOff + j + 1].normalized);
								
								b = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j] + (kOff + k) * baseMesh[iOff + i][jOff + j].normalized);
								normals.Add (-baseMesh[iOff + i][jOff + j].normalized);
								
								c = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j] + (kOff + k) * baseMesh[iOff + i + 1][jOff + j].normalized);
								normals.Add (-baseMesh[iOff + i + 1][jOff + j].normalized);
								
								d = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j + 1] + (kOff + k) * baseMesh[iOff + i + 1][jOff + j + 1].normalized);
								normals.Add (-baseMesh[iOff + i + 1][jOff + j + 1].normalized);
								
								this.AddUV (uv, blockType, CubeFace.Bottom, isOnTop);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (b);
								triangles[subMesh].Add (c);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (c);
								triangles[subMesh].Add (d);
							}
							
							bool buildKPlus = false;

							if (blockType != 2) {
								if (k + 1 < 32) {
									if ((this.blocks [i + j * PlanetUtility.ChunckSize + (k + 1) * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] == 0) || (this.blocks [i + j * PlanetUtility.ChunckSize + (k + 1) * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] == 2)) {
										buildKPlus = true;
									}
								}
								else if (k + 1 == 32) {
									buildKPlus = true;
								}
							}
							else if ((blockType == 2) && isOnTop) {
								buildKPlus = true;
							}

							if (buildKPlus) {
								a = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j] + (kOff + k + blockHeight) * baseMesh[iOff + i][jOff + j].normalized);
								normals.Add (baseMesh[iOff + i][jOff + j].normalized);
								
								b = vertices.Count;
								vertices.Add (baseMesh[iOff + i][jOff + j + 1] + (kOff + k + blockHeight) * baseMesh[iOff + i][jOff + j + 1].normalized);
								normals.Add (baseMesh[iOff + i][jOff + j + 1].normalized);
								
								c = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j + 1] + (kOff + k + blockHeight) * baseMesh[iOff + i + 1][jOff + j + 1].normalized);
								normals.Add (baseMesh[iOff + i + 1][jOff + j + 1].normalized);
								
								d = vertices.Count;
								vertices.Add (baseMesh[iOff + i + 1][jOff + j] + (kOff + k + blockHeight) * baseMesh[iOff + i + 1][jOff + j].normalized);
								normals.Add (baseMesh[iOff + i + 1][jOff + j].normalized);
								
								this.AddUV (uv, blockType, CubeFace.Top, isOnTop);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (b);
								triangles[subMesh].Add (c);
								
								triangles[subMesh].Add (a);
								triangles[subMesh].Add (c);
								triangles[subMesh].Add (d);
							}
						}
					}
				}
			}

			mesh.vertices = vertices.ToArray ();
			this.meshCollider.vertices = vertices.ToArray ();

			mesh.SetTriangles (triangles [0].ToArray(), 0);
			mesh.SetTriangles (triangles [1].ToArray(), 1);
			this.meshCollider.SetTriangles (triangles [0].ToArray(), 0);

			mesh.normals = normals.ToArray ();
			mesh.uv = uv.ToArray ();

			return mesh;
		}

		private void AddUV (List<Vector2> uvs, int blockType, CubeFace face, bool isOnTop) {
			Vector2 offSet = Vector2.zero;
			float padding = 5f;
			Vector2 padding0 = new Vector2 (padding / 1024f, padding / 1024f);
			Vector2 padding1 = new Vector2 (padding / 1024f, - padding / 1024f);
			Vector2 padding2 = new Vector2 (- padding / 1024f, - padding / 1024f);
			Vector2 padding3 = new Vector2 (- padding / 1024f, padding / 1024f);

			if (blockType == 1) {
				offSet = new Vector2 (0f, 3f/4f);
			}
			else if (blockType == 2) {
				uvs.Add (new Vector2 (0f, 0f));
				uvs.Add (new Vector2 (0f, 1f));
				uvs.Add (new Vector2 (1f, 1f));
				uvs.Add (new Vector2 (1f, 0f));
				return;
			}
			else if (blockType == 3) {
				offSet = new Vector2 (1f/4f, 3f/4f);
			}
			else if (blockType == 4) {
				offSet = new Vector2 (1f/2f, 3f/4f);
			}
			else if (blockType == 5) {
				offSet = new Vector2 (3f/4f, 3f/4f);
			}

			if (face == CubeFace.Top) {
				uvs.Add (offSet + new Vector2 (0f, 1f/8f) + padding0);
				uvs.Add (offSet + new Vector2 (0f, 2f/8f) + padding1);
				uvs.Add (offSet + new Vector2 (1f/8f, 2f/8f) + padding2);
				uvs.Add (offSet + new Vector2 (1f/8f, 1f/8f) + padding3);
			}
			else if ((face == CubeFace.Side) && isOnTop) {
				uvs.Add (offSet + new Vector2 (1f/8f, 1f/8f) + padding0);
				uvs.Add (offSet + new Vector2 (1f/8f, 2f/8f) + padding1);
				uvs.Add (offSet + new Vector2 (2f/8f, 2f/8f) + padding2);
				uvs.Add (offSet + new Vector2 (2f/8f, 1f/8f) + padding3);
			}
			else if (((face == CubeFace.Side) && !isOnTop) || (face == CubeFace.Bottom)) {
				uvs.Add (offSet + new Vector2 (0f, 0f) + padding0);
				uvs.Add (offSet + new Vector2 (0f, 1f/8f) + padding1);
				uvs.Add (offSet + new Vector2 (1f/8f, 1f/8f) + padding2);
				uvs.Add (offSet + new Vector2 (1f/8f, 0f) + padding3);
			}
		}

		public void SetMesh () {
			this.transform.parent = this.planetSide.transform;
			this.name = "Chunck_" + this.posInChunck.x + "|" + this.posInChunck.y + "|" + this.posInChunck.z;
			this.transform.localPosition = Vector3.zero;
			this.transform.localRotation = Quaternion.identity;
			this.transform.localScale = Vector3.one;

			this.GetComponent<MeshFilter> ().sharedMesh = this.BuildMesh ();

			this.GetComponent<MeshRenderer> ().materials = this.planetSide.materials;

			if (this.GetComponent<MeshCollider> ()) {
				this.GetComponent<MeshCollider> ().sharedMesh = this.meshCollider;
			}
		}
	}
}
