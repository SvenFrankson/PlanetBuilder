using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	public class PlanetCursor {

		static GameObject cursorInstance = null;
		static public GameObject CursorInstance {
			get {
				if (cursorInstance == null) {
					cursorInstance = GameObject.Find ("CursorPlanet");
					if (cursorInstance == null) {
						cursorInstance = new GameObject ("CursorPlanet");
						cursorInstance.AddComponent<MeshFilter> ();
						cursorInstance.AddComponent<MeshRenderer> ();
					}
				}

				return cursorInstance;
			}
		}

		static Transform cursorTranform = null;
		static public Transform CursorTransform {
			get {
				if (cursorTranform == null) {
					cursorTranform = CursorInstance.transform;
				}

				return cursorTranform;
			}
		}
		
		static MeshFilter cursorMeshFilter = null;
		static public MeshFilter CursorMeshFilter {
			get {
				if (cursorMeshFilter == null) {
					cursorMeshFilter = CursorInstance.GetComponent<MeshFilter> ();
				}
				
				return cursorMeshFilter;
			}
		}
		
		static MeshRenderer cursorMeshRenderer = null;
		static public MeshRenderer CursorMeshRenderer {
			get {
				if (cursorMeshRenderer == null) {
					cursorMeshRenderer = CursorInstance.GetComponent<MeshRenderer> ();
				}

				return cursorMeshRenderer;
			}
		}

		static public PlanetSide cursorPlanetSide = null;
		static public Vector3 posInPlanetSide = - Vector3.one;

		static public Vector3 GetLocalPosBlockCenter () {
			int i = (int) posInPlanetSide.x;
			int j = (int) posInPlanetSide.y;
			int k = (int) posInPlanetSide.z;

			Vector3[][] baseMesh = PlanetUtility.GetBaseMesh (cursorPlanetSide.subDegree).vertices;

			Vector3 center = Vector3.zero;

			center += baseMesh[i][j + 1] + (k) * baseMesh[i][j + 1].normalized;
			center += baseMesh[i][j] + (k) * baseMesh[i][j].normalized;
			center += baseMesh[i + 1][j] + (k) * baseMesh[i + 1][j].normalized;
			center += baseMesh[i + 1][j + 1] + (k) * baseMesh[i + 1][j + 1].normalized;

			center = center / 4f;

			return center;
		}

		static public void HideCursor () {
			PlanetCursor.CursorMeshRenderer.enabled = false;
		}

		static public void SetMaterial (Material mat) {
			CursorMeshRenderer.sharedMaterials = new Material[1] {mat};
		}

		static public void SetAt (int blockType, int i, int j, int k, PlanetSide planetSide) {
			if ((PlanetCursor.posInPlanetSide != new Vector3 (i, j, k)) || (planetSide.transform != PlanetCursor.CursorTransform.parent)) {
				cursorPlanetSide = planetSide;
				PlanetCursor.BuildCursor (blockType, i, j, k, planetSide);
			}
		}
		
		static void BuildCursor (int blockType, int i, int j, int k, PlanetSide planetSide) {
			Mesh cursorMesh = new Mesh ();
			
			List<Vector3> cursorVertices = new List<Vector3> ();
			List<Vector3> cursorNormals = new List<Vector3> ();
			List<Vector2> cursorUV = new List<Vector2> ();
			List<int> cursorTriangles = new List<int> ();
			
			Vector3[][] baseMesh = PlanetUtility.GetBaseMesh (cursorPlanetSide.subDegree).vertices;
			
			int a = 0;
			int b = 0;
			int c = 0;
			int d = 0;
			
			float blockHeight = 1f;
			
			a = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j + 1] + (k) * baseMesh[i][j + 1].normalized);
			cursorNormals.Add ((baseMesh[i][j + 1] - baseMesh[i + 1][j + 1]).normalized);
			cursorUV.Add (new Vector2 (0.5f, 0.5f));
			
			b = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j + 1] + (k + blockHeight) * baseMesh[i][j + 1].normalized);
			cursorNormals.Add ((baseMesh[i][j + 1] - baseMesh[i + 1][j + 1]).normalized);
			cursorUV.Add (new Vector2 (0.5f, 1));
			
			c = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j] + (k + blockHeight) * baseMesh[i][j].normalized);
			cursorNormals.Add ((baseMesh[i][j] - baseMesh[i + 1][j]).normalized);
			cursorUV.Add (new Vector2 (1, 1));
			
			d = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j] + (k) * baseMesh[i][j].normalized);
			cursorNormals.Add ((baseMesh[i][j] - baseMesh[i + 1][j]).normalized);
			cursorUV.Add (new Vector2 (1, 0.5f));
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (b);
			cursorTriangles.Add (c);
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (c);
			cursorTriangles.Add (d);
			
			a = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j] + (k) * baseMesh[i + 1][j].normalized);
			cursorNormals.Add ((baseMesh[i + 1][j] - baseMesh[i][j]).normalized);
			cursorUV.Add (new Vector2 (0.5f, 0.5f));
			
			b = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j] + (k + blockHeight) * baseMesh[i + 1][j].normalized);
			cursorNormals.Add ((baseMesh[i + 1][j] - baseMesh[i][j]).normalized);
			cursorUV.Add (new Vector2 (0.5f, 1));
			
			c = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j + 1] + (k + blockHeight) * baseMesh[i + 1][j + 1].normalized);
			cursorNormals.Add ((baseMesh[i + 1][j + 1] - baseMesh[i][j + 1]).normalized);
			cursorUV.Add (new Vector2 (1, 1));
			
			d = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j + 1] + (k) * baseMesh[i + 1][j + 1].normalized);
			cursorNormals.Add ((baseMesh[i + 1][j + 1] - baseMesh[i][j + 1]).normalized);
			cursorUV.Add (new Vector2 (1, 0.5f));
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (b);
			cursorTriangles.Add (c);
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (c);
			cursorTriangles.Add (d);
			
			a = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j] + (k) * baseMesh[i][j].normalized);
			cursorNormals.Add ((baseMesh[i][j] - baseMesh[i][j + 1]).normalized);
			cursorUV.Add (new Vector2 (0.5f, 0.5f));
			
			b = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j] + (k + blockHeight) * baseMesh[i][j].normalized);
			cursorNormals.Add ((baseMesh[i][j] - baseMesh[i][j + 1]).normalized);
			cursorUV.Add (new Vector2 (0.5f, 1));
			
			c = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j] + (k + blockHeight) * baseMesh[i + 1][j].normalized);
			cursorNormals.Add ((baseMesh[i + 1][j] - baseMesh[i + 1][j + 1]).normalized);
			cursorUV.Add (new Vector2 (1, 1));
			
			d = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j] + (k) * baseMesh[i + 1][j].normalized);
			cursorNormals.Add ((baseMesh[i + 1][j] - baseMesh[i + 1][j + 1]).normalized);
			cursorUV.Add (new Vector2 (1, 0.5f));
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (b);
			cursorTriangles.Add (c);
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (c);
			cursorTriangles.Add (d);
			
			a = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j + 1] + (k) * baseMesh[i + 1][j + 1].normalized);
			cursorNormals.Add ((baseMesh[i + 1][j + 1] - baseMesh[i + 1][j]).normalized);
			cursorUV.Add (new Vector2 (0.5f, 0.5f));
			
			b = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j + 1] + (k + blockHeight) * baseMesh[i + 1][j + 1].normalized);
			cursorNormals.Add ((baseMesh[i + 1][j + 1] - baseMesh[i + 1][j]).normalized);
			cursorUV.Add (new Vector2 (0.5f, 1));
			
			c = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j + 1] + (k + blockHeight) * baseMesh[i][j + 1].normalized);
			cursorNormals.Add ((baseMesh[i][j + 1] - baseMesh[i][j]).normalized);
			cursorUV.Add (new Vector2 (1, 1));
			
			d = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j + 1] + (k) * baseMesh[i][j + 1].normalized);
			cursorNormals.Add ((baseMesh[i][j + 1] - baseMesh[i][j]).normalized);
			cursorUV.Add (new Vector2 (1, 0.5f));
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (b);
			cursorTriangles.Add (c);
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (c);
			cursorTriangles.Add (d);
			
			a = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j + 1] + (k) * baseMesh[i][j + 1].normalized);
			cursorNormals.Add (-baseMesh[i][j + 1].normalized);
			cursorUV.Add (new Vector2 (0, 0));
			
			b = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j] + (k) * baseMesh[i][j].normalized);
			cursorNormals.Add (-baseMesh[i][j].normalized);
			cursorUV.Add (new Vector2 (0, 0.5f));
			
			c = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j] + (k) * baseMesh[i + 1][j].normalized);
			cursorNormals.Add (-baseMesh[i + 1][j].normalized);
			cursorUV.Add (new Vector2 (0.5f, 0.5f));
			
			d = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j + 1] + (k) * baseMesh[i + 1][j + 1].normalized);
			cursorNormals.Add (-baseMesh[i + 1][j + 1].normalized);
			cursorUV.Add (new Vector2 (0.5f, 0));
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (b);
			cursorTriangles.Add (c);
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (c);
			cursorTriangles.Add (d);
			
			a = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j] + (k + blockHeight) * baseMesh[i][j].normalized);
			cursorNormals.Add (baseMesh[i][j].normalized);
			cursorUV.Add (new Vector2 (0, 0.5f));
			
			b = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i][j + 1] + (k + blockHeight) * baseMesh[i][j + 1].normalized);
			cursorNormals.Add (baseMesh[i][j + 1].normalized);
			cursorUV.Add (new Vector2 (0, 1));
			
			c = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j + 1] + (k + blockHeight) * baseMesh[i + 1][j + 1].normalized);
			cursorNormals.Add (baseMesh[i + 1][j + 1].normalized);
			cursorUV.Add (new Vector2 (0.5f, 1));
			
			d = cursorVertices.Count;
			cursorVertices.Add (baseMesh[i + 1][j] + (k + blockHeight) * baseMesh[i + 1][j].normalized);
			cursorNormals.Add (baseMesh[i + 1][j].normalized);
			cursorUV.Add (new Vector2 (0.5f, 0.5f));
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (b);
			cursorTriangles.Add (c);
			
			cursorTriangles.Add (a);
			cursorTriangles.Add (c);
			cursorTriangles.Add (d);

			if (blockType == 0) {
				Vector3 barycenter = Vector3.zero;
				foreach (Vector3 v in cursorVertices) {
					barycenter += v;
				}

				barycenter /= ((float) cursorVertices.Count);

				for (int v = 0; v < cursorVertices.Count; v++) {
					cursorVertices [v] += (cursorVertices [v] - barycenter).normalized * 0.1f;
				}
			}
			
			cursorMesh.vertices = cursorVertices.ToArray ();
			cursorMesh.triangles = cursorTriangles.ToArray ();
			cursorMesh.normals = cursorNormals.ToArray ();
			cursorMesh.uv = cursorUV.ToArray ();
			
			PlanetCursor.CursorMeshFilter.sharedMesh = cursorMesh;
			PlanetCursor.posInPlanetSide = new Vector3 (i, j, k);
			PlanetCursor.CursorTransform.parent = planetSide.transform;
			PlanetCursor.CursorTransform.localPosition = Vector3.zero;
			PlanetCursor.CursorTransform.localRotation = Quaternion.identity;
			PlanetCursor.CursorMeshRenderer.enabled = true;
		}
	}
}