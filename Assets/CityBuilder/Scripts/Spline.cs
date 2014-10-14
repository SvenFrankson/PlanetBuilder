//	The MIT License (MIT)
//	
//	Copyright (c) 2014 SvenFrankson
//		
//		Permission is hereby granted, free of charge, to any person obtaining a copy
//		of this software and associated documentation files (the "Software"), to deal
//		in the Software without restriction, including without limitation the rights
//		to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//		copies of the Software, and to permit persons to whom the Software is
//		furnished to do so, subject to the following conditions:
//		
//		The above copyright notice and this permission notice shall be included in all
//		copies or substantial portions of the Software.
//		
//		THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//		IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//		FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//		AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//		LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//		OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//		SOFTWARE.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	public class Spline : MonoBehaviour {

		public Spline end;
		public Transform sphericalGravity;

		public Vector3 a = Vector3.zero;
		public Vector3 b = Vector3.zero;
		public Vector3 c = Vector3.zero;
		public Vector3 d = Vector3.zero;

		public GameObject patern;

		public float zeroSpeed = 40f;

		private MeshFilter cMeshFilter;
		public MeshFilter CMeshFilter {
			get {
				if (this.cMeshFilter == null) {
					this.cMeshFilter = this.GetComponent<MeshFilter> ();
					if (this.cMeshFilter == null) {
						this.cMeshFilter = this.gameObject.AddComponent<MeshFilter> ();
					}
				}
				return this.cMeshFilter;
			}
		}
		
		private MeshCollider cMeshCollider;
		public MeshCollider CMeshCollider {
			get {
				if (this.cMeshCollider == null) {
					this.cMeshCollider = this.GetComponent<MeshCollider> ();
					if (this.cMeshCollider == null) {
						DestroyImmediate (this.gameObject.GetComponent<Collider> ());
						this.cMeshCollider = this.gameObject.AddComponent<MeshCollider> ();
					}
				}
				return this.cMeshCollider;
			}
		}

		private Mesh PrimitiveSphere {
			get {
				GameObject sphere = GameObject.CreatePrimitive (PrimitiveType.Sphere);
				Mesh m = sphere.GetComponent<MeshFilter> ().sharedMesh;
				DestroyImmediate (sphere);

				return m;
			}
		}

		public void BuildSpline () {
			Vector3 Q0 = this.transform.position;
			Vector3 Q1 = this.end.transform.position;
			Vector3 T0 = this.transform.forward * zeroSpeed;
			Vector3 T1 = - this.end.transform.forward * zeroSpeed;

			this.a = Q0;
			this.b = T0;
			this.c = 3f * (Q1 - Q0) - (2f * T0 + T1);
			this.d = (T1 - T0) / 3f - 2f / 3f * this.c;

			this.end.end = this;
		}

		public void SearchPlanet () {

			if (this.sphericalGravity == null) {
				bool found = false;
				Transform p = this.transform.parent;
				while (!found) {
					if (p == null) {
						found = true;
					}
					else if (p.GetComponent<Planet> () != null) {
						found = true;
						this.sphericalGravity = p;
					}
					else {
						p = p.parent;
					}
				}
			}
		}

		public Vector3 Evaluate (float t) {
			Vector3 value = this.a + this.b * t + this.c * t * t + this.d * t * t * t;

			return value;
		}

		public Vector3 EvaluateTangent (float t) {
			Vector3 value = this.b + 2f * this.c * t + 3f * this.d * t * t;
			
			return value;
		}

		public float SplineLength () {
			float l = 0;

			l += Vector3.Distance (this.Evaluate (0.25f), this.Evaluate (0f));
			l += Vector3.Distance (this.Evaluate (0.5f), this.Evaluate (0.25f));
			l += Vector3.Distance (this.Evaluate (0.75f), this.Evaluate (0.5f));
			l += Vector3.Distance (this.Evaluate (1f), this.Evaluate (0.75f));

			return l;
		}

		public void PopOnSpline () {
			this.SearchPlanet ();

			int ringCount = Mathf.FloorToInt (this.SplineLength () / 1f) + 1;
			List<GameObject> parts = new List<GameObject> ();

			this.transform.position = this.transform.position;
			this.transform.rotation = this.transform.rotation;

			for (int i = 0; i <= ringCount; i++) {
				float t = (float) i / (float) ringCount;
				GameObject g = new GameObject ();
				g.transform.position = this.Evaluate (t);
				Vector3 f = this.EvaluateTangent (t);
				Vector3 up = Vector3.up;
				if (this.sphericalGravity != null) {
					up = (g.transform.position - this.sphericalGravity.position).normalized;
				}
				g.transform.rotation = Quaternion.LookRotation (f, up);
				g.transform.localScale = Vector3.one;
				g.transform.parent = this.transform;
				parts.Add (g);
			}

			this.FuseBuildAndBake (parts.ToArray ());
		}

		static public Mesh ExtractPatern (out List<KeyValuePair<int, int[]>> pairs, Mesh mesh) {
			pairs = new List<KeyValuePair<int, int[]>> ();
			Dictionary<int, int> oldToNewIndex = new Dictionary<int, int> ();
			List<Vector3> newVertices = new List<Vector3> ();

			for (int i = 0; i < mesh.vertexCount; i++) {
				if (Mathf.Abs(mesh.vertices [i].z) < 0.05f) {
					oldToNewIndex.Add (i, newVertices.Count);
					newVertices.Add (mesh.vertices [i]);
				}
			}

			List<int> oldKept = new List<int> (oldToNewIndex.Keys);

			for (int t = 0; t < mesh.subMeshCount; t++) {
				int[] triangles = mesh.GetTriangles (t);

				for (int i = 0; i < triangles.Length / 3; i++) {
					int a = triangles [3 * i];
					int b = triangles [3 * i + 1];
					int c = triangles [3 * i + 2];
					
					if (oldKept.Contains (a)) {
						if (oldKept.Contains (b)) {
							pairs.Add (new KeyValuePair<int, int[]> (t, new int[] {oldToNewIndex[a], oldToNewIndex[b]}));
						}
					}
					
					if (oldKept.Contains (b)) {
						if (oldKept.Contains (c)) {
							pairs.Add (new KeyValuePair<int, int[]> (t, new int[] {oldToNewIndex[b], oldToNewIndex[c]}));
						}
					}
					
					if (oldKept.Contains (c)) {
						if (oldKept.Contains (a)) {
							pairs.Add (new KeyValuePair<int, int[]> (t, new int[] {oldToNewIndex[c], oldToNewIndex[a]}));
						}
					}
				}
			}
			
			Mesh m = new Mesh ();
			m.vertices = newVertices.ToArray ();

			return m;
		}

		public void FuseBuildAndBake (GameObject[] parts) {

			List<KeyValuePair<int, int[]>> pairs = new List<KeyValuePair<int, int[]>> ();
			Mesh paternMesh = ExtractPatern (out pairs, this.patern.GetComponent<MeshFilter> ().sharedMesh);
			int paternVertexCount = paternMesh.vertices.Length;

			CombineInstance[] instances = new CombineInstance[parts.Length];

			for (int i = 0; i < instances.Length; i++) {
				instances[i].mesh = paternMesh;
				instances[i].subMeshIndex = 0;
				instances[i].transform = Matrix4x4.TRS (parts[i].transform.localPosition, parts[i].transform.localRotation, parts[i].transform.localScale);
			}

			Mesh fusedMesh = new Mesh ();

			fusedMesh.CombineMeshes (instances, true, true);

			List<int>[] triangles = new List<int> [this.patern.GetComponent<MeshFilter> ().sharedMesh.subMeshCount];
			for (int i = 0; i < triangles.Length; i++) {
				triangles[i] = new List<int> ();
			}

			foreach (KeyValuePair<int, int[]> kvp in pairs) {
				for (int i = 0; i < parts.Length - 1; i++) {
					triangles[kvp.Key].Add (i * paternVertexCount + kvp.Value[0]);
					triangles[kvp.Key].Add ((i + 1) * paternVertexCount + kvp.Value[0]);
					triangles[kvp.Key].Add (i * paternVertexCount + kvp.Value[1]);
				
					triangles[kvp.Key].Add ((i + 1) * paternVertexCount + kvp.Value[0]);
					triangles[kvp.Key].Add ((i + 1) * paternVertexCount + kvp.Value[1]);
					triangles[kvp.Key].Add (i * paternVertexCount + kvp.Value[1]);
				}
			}

			fusedMesh.subMeshCount = this.patern.GetComponent<MeshFilter> ().sharedMesh.subMeshCount;
			for (int i = 0; i < triangles.Length; i++) {
				fusedMesh.SetTriangles (triangles[i].ToArray (), i);
			}
			fusedMesh.RecalculateNormals ();

			for (int i = 0; i < parts.Length; i++) {
				DestroyImmediate (parts[i]);
			}
			
			this.CMeshFilter.sharedMesh = fusedMesh;
			this.CMeshCollider.sharedMesh = fusedMesh;

			this.end.CMeshFilter.sharedMesh = null;
			this.end.CMeshCollider.sharedMesh = null;
		}

		public void DestroyPath () {
			this.CMeshCollider.sharedMesh = null;
			this.CMeshFilter.sharedMesh = this.PrimitiveSphere;

			if (this.end != null) {
				this.end.CMeshCollider.sharedMesh = null;
				this.end.CMeshFilter.sharedMesh = this.PrimitiveSphere;
				this.end.end = null;
			}

			this.end = null;
		}
	}
}
