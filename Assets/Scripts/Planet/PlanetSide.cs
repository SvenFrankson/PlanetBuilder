using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	public class PlanetSide : MonoBehaviour {

		public Planet planet;
		public int subDegree;

		public Vector3 orientation;
		public int heightChuncks;
		public int nbChunks;
		public PlanetChunck[] chuncks;

		public Material[] materials;

		public void Initialize (Planet planet, Vector3 orientation, int subDegree) {
			this.planet = planet;
			this.materials = planet.planetMaterials;
			this.subDegree = subDegree;
			this.orientation = orientation;
			this.nbChunks = ((int) Mathf.Pow (2, subDegree)) / PlanetUtility.ChunckSize;
			this.heightChuncks = 1;

			this.chuncks = new PlanetChunck[this.nbChunks * this.nbChunks * this.heightChuncks];
			for (int i = 0; i < this.nbChunks; i++) {
				for (int j = 0; j < this.nbChunks; j++) {
					for (int k = 0; k < this.heightChuncks; k++) {
						this.chuncks[i + j * this.nbChunks + k * this.nbChunks * this.heightChuncks] = PlanetUtility.InstantiatePlanetChunck (this.orientation, new Vector3 (i, j, k), this);
					}
				}
			}
		}

		public void SetWithHeightMap () {

			int cubeMapLength = this.planet.heightMap.width;
			CubemapFace face = CubemapFace.PositiveX;
			if (this.orientation == Vector3.right) {
				face = CubemapFace.PositiveX;
			}
			else if (this.orientation == Vector3.up) {
				face = CubemapFace.PositiveY;
			}
			else if (this.orientation == Vector3.forward) {
				face = CubemapFace.PositiveZ;
			}
			else if (this.orientation == Vector3.left) {
				face = CubemapFace.NegativeX;
			}
			else if (this.orientation == Vector3.down) {
				face = CubemapFace.NegativeY;
			}
			else if (this.orientation == Vector3.back) {
				face = CubemapFace.NegativeZ;
			}

			bool hasHoles = true;

			if (this.planet.holeMap == null) {
				hasHoles = false;
			}

			int holeMax = 0;
			float aHole = 0;
			float bHole = 0;

			if (hasHoles) {
				holeMax = Mathf.FloorToInt (31 - 31 * (this.planet.holeThreshold / 255f));
				aHole = holeMax / (255f - this.planet.holeThreshold);
				bHole = holeMax * this.planet.holeThreshold / (this.planet.holeThreshold - 255f);
			}

			for (int ic = 0; ic < this.nbChunks; ic++) {
				for (int jc = 0; jc < this.nbChunks; jc++) {
					for (int kc = 0; kc < this.heightChuncks; kc++) {
						
						for (int i = 0; i < PlanetUtility.ChunckSize; i++) {
							for (int j = 0; j < PlanetUtility.ChunckSize; j++) {

								int x = Mathf.FloorToInt (((float)(ic * PlanetUtility.ChunckSize + i) / (float)(this.nbChunks * PlanetUtility.ChunckSize)) * cubeMapLength);
								int y = Mathf.FloorToInt (((float)(jc * PlanetUtility.ChunckSize + j) / (float)(this.nbChunks * PlanetUtility.ChunckSize)) * cubeMapLength);

								Color colorHM = this.planet.heightMap.GetPixel (face, x, y);
								int h = Mathf.FloorToInt (colorHM.r * this.planet.heightCoef);
								if (h > 32) {
									h = 32;
								}
								
								for (int k = 0; k < h; k++) {
									if (k < this.planet.dirtMin) {
										this.chuncks[ic + jc * this.nbChunks + kc * this.nbChunks * this.heightChuncks].blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = 3;
									}
									else if (k > this.planet.dirtMax) {
										this.chuncks[ic + jc * this.nbChunks + kc * this.nbChunks * this.heightChuncks].blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = 3;
									}
									else if (k > h - this.planet.dirtThickness) {
										if (k >= this.planet.waterLevel) {
											this.chuncks[ic + jc * this.nbChunks + kc * this.nbChunks * this.heightChuncks].blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = 1;
										}
										else {
											this.chuncks[ic + jc * this.nbChunks + kc * this.nbChunks * this.heightChuncks].blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = 4;
										}
									}
									else {
										this.chuncks[ic + jc * this.nbChunks + kc * this.nbChunks * this.heightChuncks].blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = 3;
									}
								}

								if (hasHoles) {
									Color colorHoleMap = this.planet.holeMap.GetPixel (face, x, y);

									int holeSize = Mathf.FloorToInt (aHole * colorHoleMap.r * 255f + bHole);

									if (holeSize > 0) {
										for (int k = 1; k < holeSize; k++) {
											this.chuncks[ic + jc * this.nbChunks + kc * this.nbChunks * this.heightChuncks].blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = 0;
										}
									}
								}

								for (int k = 0; k < this.planet.waterLevel; k++) {
									if (this.chuncks[ic + jc * this.nbChunks + kc * this.nbChunks * this.heightChuncks].blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] == 0) {
										this.chuncks[ic + jc * this.nbChunks + kc * this.nbChunks * this.heightChuncks].blocks [i + j * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = 2;
									}
								}
							}
						}
					}
				}
			}
		}

		public void BuildAllChuncks () {
			for (int ic = 0; ic < this.nbChunks; ic++) {
				for (int jc = 0; jc < this.nbChunks; jc++) {
					for (int kc = 0; kc < this.heightChuncks; kc++) {
						this.chuncks[ic + jc * this.nbChunks + kc * this.nbChunks * this.heightChuncks].SetMesh ();
					}
				}
			}
		}

		public void BuildSide (Planet planet, Vector3 orientation, int subDegree) {
			while (this.transform.childCount > 0) {
				DestroyImmediate (this.transform.GetChild (0).gameObject);
			}
			this.Initialize (planet, orientation, subDegree);
			this.SetWithHeightMap ();
			this.BuildAllChuncks ();
		}

		public int GetBlockFor (int i, int j, int k) {
			if (i < 0) {
				return -1;
			}
			if (j < 0) {
				return -1;
			}
			if (k < 0) {
				return -1;
			}
			if (i >= this.nbChunks * PlanetUtility.ChunckSize) {
				return -1;
			}
			if (j >= this.nbChunks * PlanetUtility.ChunckSize) {
				return -1;
			}
			if (k >= 32) {
				return -1;
			}

			int iChunck = ((int) i) / PlanetUtility.ChunckSize;
			int jChunck = ((int) j) / PlanetUtility.ChunckSize;
			int iInChunck = ((int) i) % PlanetUtility.ChunckSize;
			int jInChunck = ((int) j) % PlanetUtility.ChunckSize;
			
			return this.chuncks [iChunck + jChunck * this.nbChunks].blocks [iInChunck + jInChunck * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize];
		}

		public Vector3 GetIJKFor (Vector3 planetPos) {
			Vector3 localPos = Quaternion.Inverse (this.transform.localRotation) * planetPos;

			return PlanetUtility.GetBaseMesh (this.subDegree).GetIJK (localPos);
		}

		public Vector3 GetLocalPosBlockCenter (int i, int j, int k) {
			Vector3 localPos = Vector3.zero;

			Vector3 [][] baseMesh = PlanetUtility.GetBaseMesh (this.subDegree).vertices;

			localPos = baseMesh [i] [j] + baseMesh [i] [j + 1] + baseMesh [i + 1] [j] + baseMesh [i + 1] [j + 1];
			localPos = localPos / 4f;
			localPos += localPos.normalized * k;

			return localPos;
		}

		public int GetKFor (int i, int j) {
			int iChunck = ((int) i) / PlanetUtility.ChunckSize;
			int jChunck = ((int) j) / PlanetUtility.ChunckSize;
			int iInChunck = ((int) i) % PlanetUtility.ChunckSize;
			int jInChunck = ((int) j) % PlanetUtility.ChunckSize;

			for (int k = 31; k >= 0; k--) {
				if (this.chuncks[iChunck + jChunck * this.nbChunks].blocks [iInChunck + jInChunck * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] != 0) {
					if (this.chuncks[iChunck + jChunck * this.nbChunks].blocks [iInChunck + jInChunck * PlanetUtility.ChunckSize + k * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] != 2) {
						return k;
					}
				}
			}

			return 0;
		}

		public void AddBlockAt (Vector3 planetPos, int block) {
			Vector3 ijkPos = this.GetIJKFor (planetPos);
			
			int iChunck = ((int) ijkPos.x) / PlanetUtility.ChunckSize;
			int jChunck = ((int) ijkPos.y) / PlanetUtility.ChunckSize;
			int iInChunck = ((int) ijkPos.x) % PlanetUtility.ChunckSize;
			int jInChunck = ((int) ijkPos.y) % PlanetUtility.ChunckSize;
			int kInChunck = ((int) ijkPos.z);

			if ((iInChunck >= 0) && (iInChunck < PlanetUtility.ChunckSize)) {
				if ((jInChunck >= 0) && (jInChunck < PlanetUtility.ChunckSize)) {
					if ((kInChunck >= 0) && (kInChunck < 32)) {
						if ((iChunck >= 0) && (iChunck < this.nbChunks)) {
							if ((jChunck >= 0) && (jChunck < this.nbChunks)) {
								this.chuncks[iChunck + jChunck * this.nbChunks].blocks [iInChunck + jInChunck * PlanetUtility.ChunckSize + kInChunck * PlanetUtility.ChunckSize * PlanetUtility.ChunckSize] = block;
								this.chuncks[iChunck + jChunck * this.nbChunks].SetMesh ();
							}
						}
					}
				}
			}
		}
	}
}
