using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {
	
	public class PlanetBaseMesh {
		
		private int subDegree = -1;
		private int edgeCount;
		private int edgeLength;
		
		public Vector3[][] vertices;
		public Vector3[][] centers;
		public Vector3[] normals;
		public List<int> triangles;

		private Mesh mesh;

		public PlanetBaseMesh (int subDegree) {
			this.subDegree = subDegree;
			this.Initialize ();
		}
		
		public void Initialize () {
			this.triangles = new List<int> ();
			
			this.edgeCount = (int) Mathf.Pow (2, this.subDegree);
			this.edgeLength = this.edgeCount + 1;
			
			this.vertices = new Vector3[this.edgeLength][];
			for (int i = 0; i < this.edgeLength; i++) {
				this.vertices[i] = new Vector3[this.edgeLength];
			}
			
			for (int j = 0; j < this.edgeLength; j++) {
				for (int i = 0; i < this.edgeLength; i++) {
					this.vertices[i][j] = Vector3.zero;
					
					if (i < this.edgeCount) {
						if (j < this.edgeCount) {
							this.triangles.Add (i + j * this.edgeLength);
							this.triangles.Add (i + (j + 1) * this.edgeLength);
							this.triangles.Add ((i + 1) + (j + 1) * this.edgeLength);
							
							this.triangles.Add (i + j * this.edgeLength);
							this.triangles.Add ((i + 1) + (j + 1) * this.edgeLength);
							this.triangles.Add ((i + 1) + j * this.edgeLength);
						}
					}
				}
			}
			
			this.StraightVertexInit ();

			this.centers = new Vector3[this.edgeCount][];
			for (int i = 0; i < this.edgeCount; i++) {
				this.centers [i] = new Vector3[this.edgeCount];
				for (int j = 0; j < this.edgeCount; j++) {
					this.centers [i][j] = (this.vertices[i][j] + this.vertices[i][j + 1] + this.vertices[i + 1][j + 1] + this.vertices[i + 1][j]).normalized * this.edgeCount * 0.8f;
				}
			}
		}

		public void StraightVertexInit () {

			for (int j = 0; j < this.edgeLength; j++) {
				for (int i = 0; i < this.edgeLength; i++) {

					float iRad = -45f + i * (90f / (float)this.edgeCount);
					float jRad = -45f + j * (90f / (float)this.edgeCount);
					
					iRad = iRad * Mathf.Deg2Rad;
					jRad = jRad * Mathf.Deg2Rad;
					
					this.vertices[i][j] = new Vector3 (Mathf.Sin (iRad) / Mathf.Cos (iRad), Mathf.Sin (jRad) / Mathf.Cos (jRad), -1f).normalized * this.edgeCount * 0.8f;
				}
			}
		}

		public Vector3 GetIJK (Vector3 pos) {
			Vector3 posZero = pos.normalized * this.edgeCount * 0.8f;
			Vector2 ijPos = this.NonRecursiveGetIJ (posZero);
			int h = Mathf.FloorToInt (pos.magnitude - this.edgeCount * 0.8f);

			return new Vector3 (ijPos.x, ijPos.y, h);
		}

		public Vector2 NonRecursiveGetIJ (Vector3 pos) {
			int closestI = 0;
			int closestJ = 0;
			float sqrDist = (pos - this.centers [0] [0]).sqrMagnitude;

			for (int i = 0; i < this.edgeCount; i++) {
				for (int j = 0; j < this.edgeCount; j++) {
					float tmpDist = (pos - this.centers [i] [j]).sqrMagnitude;
					if (tmpDist < sqrDist) {
						sqrDist = tmpDist;
						closestI = i;
						closestJ = j;
					}
				}
			}

			return new Vector2 (closestI, closestJ);
		}

		public Vector2 RecursiveGetIJ (Vector3 pos, int i1, int j1, int i2, int j2) {

			Vector3 A = this.vertices [i1] [j1];
			Vector3 B = this.vertices [i1] [j2];
			Vector3 C = this.vertices [i2] [j2];
			Vector3 D = this.vertices [i2] [j1];

			Vector3 closest = A;
			int closestI = i1;
			int closestJ = j1;
			float sqrDist = (pos - A).sqrMagnitude;

			if ((pos - B).sqrMagnitude < sqrDist) {
				closest = B;
				sqrDist = (pos - B).sqrMagnitude;
				closestI = i1;
				closestJ = j2;
			}
			
			if ((pos - C).sqrMagnitude < sqrDist) {
				closest = C;
				sqrDist = (pos - C).sqrMagnitude;
				closestI = i2;
				closestJ = j2;
			}

			if ((pos - D).sqrMagnitude < sqrDist) {
				closest = D;
				sqrDist = (pos - D).sqrMagnitude;
				closestI = i2;
				closestJ = j1;
			}

			if (i1 + 1 == i2) {
				return new Vector2 (closestI, closestJ);
			}

			if (closest == A) {
				return RecursiveGetIJ (pos, i1, j1, (i1 + i2) /2, (j1 + j2) /2);
			}
			else if (closest == B) {
				return RecursiveGetIJ (pos, i1, (j1 + j2) /2 + 1, (i1 + i2) /2, j2);
			}
			else if (closest == C) {
				return RecursiveGetIJ (pos, (i1 + i2) /2 + 1, (j1 + j2) /2 + 1, i2, j2);
			}
			else {
				return RecursiveGetIJ (pos, (i1 + i2) /2 + 1, j1, i2, (j1 + j2) /2);
			}
		}
		
		public Mesh GetMesh () {
			Mesh m = new Mesh ();
			
			Vector3[] arrayVertices = new Vector3[this.edgeLength * this.edgeLength];
			Vector3[] arrayNormals = new Vector3[this.edgeLength * this.edgeLength];
			Vector2[] arrayUv = new Vector2[this.edgeLength * this.edgeLength];
			
			for (int i = 0; i < this.edgeLength; i++) {
				for (int j = 0; j < this.edgeLength; j++) {
					arrayVertices[i + j * this.edgeLength] = this.vertices[i][j];
					arrayNormals[i + j * this.edgeLength] = this.vertices[i][j];
					arrayUv[i + j * this.edgeLength] = new Vector2 (((float) i) / ((float) this.edgeCount), ((float) j) / ((float) this.edgeCount));
				}
			}
			
			m.vertices = arrayVertices;
			m.triangles = this.triangles.ToArray ();
			m.normals = arrayNormals;
			m.uv = arrayUv;
			
			return m;
		}
	}
}