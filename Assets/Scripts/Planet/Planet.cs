using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {
	
	public class Planet : MonoBehaviour {
		
		public int subDegree;
		public Cubemap heightMap;
		public int heightCoef;
		public Cubemap holeMap;
		public int holeThreshold;
		//public Cubemap holeHeightMap;
		public int waterLevel;
		public int dirtThickness;
		public int dirtMin;
		public int dirtMax;
		public VegetationData vegetationData;
		
		public PlanetSide up;
		public PlanetSide down;
		public PlanetSide right;
		public PlanetSide left;
		public PlanetSide forward;
		public PlanetSide back;
		public List<PlanetSide> planetSides;
		
		public Transform vegetationFolder;
		
		public Material[] planetMaterials = new Material[2];

		public void Start () {
			if (this.vegetationData != null) {
				this.vegetationData.ComputeVegetationRange ();
			}
		}
		
		public void BuildPlanet () {
			#if UNITY_EDITOR
			EditorUtility.DisplayProgressBar ("Planet Builder", "Building Planet Sides...", 0f);
			#endif
			up.BuildSide (this, Vector3.up, subDegree);
			#if UNITY_EDITOR
			EditorUtility.DisplayProgressBar ("Planet Builder", "Building Planet Sides...", 1f / 6f);
			#endif
			down.BuildSide (this, Vector3.down, subDegree);
			#if UNITY_EDITOR
			EditorUtility.DisplayProgressBar ("Planet Builder", "Building Planet Sides...", 2f / 6f);
			#endif
			right.BuildSide (this, Vector3.right, subDegree);
			#if UNITY_EDITOR
			EditorUtility.DisplayProgressBar ("Planet Builder", "Building Planet Sides...", 3f / 6f);
			#endif
			left.BuildSide (this, Vector3.left, subDegree);
			#if UNITY_EDITOR
			EditorUtility.DisplayProgressBar ("Planet Builder", "Building Planet Sides...", 4f / 6f);
			#endif
			forward.BuildSide (this, Vector3.forward, subDegree);
			#if UNITY_EDITOR
			EditorUtility.DisplayProgressBar ("Planet Builder", "Building Planet Sides...", 5f / 6f);
			#endif
			back.BuildSide (this, Vector3.back, subDegree);
			#if UNITY_EDITOR
			EditorUtility.ClearProgressBar ();
			#endif
		}
		
		public void SearchPlanetSide () {
			if (this.up == null) {
				this.up = this.transform.Find ("up").GetComponent<PlanetSide> ();
			}
			if (this.down == null) {
				this.down = this.transform.Find ("down").GetComponent<PlanetSide> ();
			}
			if (this.right == null) {
				this.right = this.transform.Find ("right").GetComponent<PlanetSide> ();
			}
			if (this.left == null) {
				this.left = this.transform.Find ("left").GetComponent<PlanetSide> ();
			}
			if (this.forward == null) {
				this.forward = this.transform.Find ("forward").GetComponent<PlanetSide> ();
			}
			if (this.back == null) {
				this.back = this.transform.Find ("back").GetComponent<PlanetSide> ();
			}
			if (this.vegetationFolder == null) {
				this.vegetationFolder = this.transform.Find ("Vegetation");
				if (this.vegetationFolder == null) {
					this.vegetationFolder = (new GameObject ()).transform;
					this.vegetationFolder.name = "Vegetation";
					this.vegetationFolder.parent = this.transform;
					this.vegetationFolder.localPosition = Vector3.zero;
					this.vegetationFolder.localRotation = Quaternion.identity;
					this.vegetationFolder.localScale = Vector3.one;
				}
			}
			
			this.planetSides = new List<PlanetSide> ();
			this.planetSides.Add (this.up);
			this.planetSides.Add (this.down);
			this.planetSides.Add (this.right);
			this.planetSides.Add (this.left);
			this.planetSides.Add (this.forward);
			this.planetSides.Add (this.back);
		}
		
		public PlanetSide GetPlanetSideFor (Vector3 localPos) {
			PlanetSide targetPlanetSide = this.up;
			float angle = Vector3.Angle (this.transform.up, localPos);
			
			if (Vector3.Angle (-this.transform.up, localPos) < angle) {
				targetPlanetSide = this.down;
				angle = Vector3.Angle (-this.transform.up, localPos);
			}
			
			if (Vector3.Angle (this.transform.right, localPos) < angle) {
				targetPlanetSide = this.right;
				angle = Vector3.Angle (this.transform.right, localPos);
			}
			if (Vector3.Angle (-this.transform.right, localPos) < angle) {
				targetPlanetSide = this.left;
				angle = Vector3.Angle (-this.transform.right, localPos);
			}
			
			if (Vector3.Angle (this.transform.forward, localPos) < angle) {
				targetPlanetSide = this.forward;
				angle = Vector3.Angle (this.transform.forward, localPos);
			}
			if (Vector3.Angle (-this.transform.forward, localPos) < angle) {
				targetPlanetSide = this.back;
				angle = Vector3.Angle (-this.transform.forward, localPos);
			}
			
			return targetPlanetSide;
		}
		
		public void AddBlockAt (Vector3 worldPos, int block) {
			Vector3 localPos = this.transform.InverseTransformPoint (worldPos);
			PlanetSide pSide = this.GetPlanetSideFor (localPos);
			pSide.AddBlockAt (localPos, block);
		}
		
		public void AddGameObjectAtCursor (GameObject prefab) {
			GameObject dropped = Instantiate (prefab, Vector3.zero, Quaternion.identity) as GameObject;
			dropped.transform.parent = PlanetCursor.cursorPlanetSide.transform;
			dropped.transform.localPosition = PlanetCursor.GetLocalPosBlockCenter ();
			float a = Vector3.Angle (dropped.transform.up, (dropped.transform.position - dropped.transform.parent.position));
			dropped.transform.RotateAround (dropped.transform.position, Vector3.Cross (dropped.transform.up, (dropped.transform.position - dropped.transform.parent.position)), a);
			dropped.transform.RotateAround (dropped.transform.position, dropped.transform.up, UnityEngine.Random.Range (0, 360f));
			dropped.transform.parent = this.vegetationFolder;
		}
		
		public void AddGameObjectAt (int i, int j, int k, PlanetSide pSide, GameObject prefab) {
			GameObject dropped = Instantiate (prefab, Vector3.zero, Quaternion.identity) as GameObject;
			dropped.transform.parent = pSide.transform;
			dropped.transform.localPosition = pSide.GetLocalPosBlockCenter (i, j, k);
			float a = Vector3.Angle (dropped.transform.up, (dropped.transform.position - dropped.transform.parent.position));
			dropped.transform.RotateAround (dropped.transform.position, Vector3.Cross (dropped.transform.up, (dropped.transform.position - dropped.transform.parent.position)), a);
			dropped.transform.RotateAround (dropped.transform.position, dropped.transform.up, UnityEngine.Random.Range (0, 360f));
			dropped.transform.parent = this.vegetationFolder;
		}
		
		public void ClearVegetation () {
			DestroyImmediate (this.vegetationFolder.gameObject);
			this.vegetationFolder = (new GameObject ()).transform;
			this.vegetationFolder.name = "Vegetation";
			this.vegetationFolder.parent = this.transform;
			this.vegetationFolder.localPosition = Vector3.zero;
			this.vegetationFolder.localRotation = Quaternion.identity;
			this.vegetationFolder.localScale = Vector3.one;
		}
		
		public void SetCursorAt (int blockType, Vector3 worldPos) {
			Vector3 localPos = this.transform.InverseTransformPoint (worldPos);
			PlanetSide pSide = this.GetPlanetSideFor (localPos);
			Vector3 ijk = pSide.GetIJKFor (localPos);
			PlanetCursor.SetAt (blockType, (int) ijk.x, (int) ijk.y, (int) ijk.z, pSide);
		}
		
		public void BuildColliders () {
			PlanetChunck[] chuncks = this.GetComponentsInChildren<PlanetChunck> ();
			
			int nbChuncks = chuncks.Length;
			int counter = 0;
			
			foreach (PlanetChunck chunck in chuncks) {
				#if UNITY_EDITOR
				EditorUtility.DisplayProgressBar ("Planet Builder", "Building Mesh Colliders...", (float) counter / (float) nbChuncks);
				counter ++;
				#endif
				
				MeshCollider mc = chunck.GetComponent<MeshCollider> ();
				if (mc == null) {
					Collider c = chunck.GetComponent<Collider> ();
					if (c != null) {
						DestroyImmediate (c);
					}
					mc = chunck.gameObject.AddComponent<MeshCollider> ();
				}
				
				mc.sharedMesh = chunck.meshCollider;
			}
			
			#if UNITY_EDITOR
			EditorUtility.ClearProgressBar ();
			#endif
		}

		public void BuildLiteColliders () {
			PlanetChunck[] chuncks = this.GetComponentsInChildren<PlanetChunck> ();
			
			int nbChuncks = chuncks.Length;
			int counter = 0;
			
			foreach (PlanetChunck chunck in chuncks) {
				#if UNITY_EDITOR
				EditorUtility.DisplayProgressBar ("Planet Builder", "Building Lite Colliders...", (float) counter / (float) nbChuncks);
				counter ++;
				#endif
				
				Collider c = chunck.gameObject.GetComponent<Collider> ();
				DestroyImmediate (c);

				chunck.gameObject.AddComponent<SphereCollider> ();
			}
			
			#if UNITY_EDITOR
			EditorUtility.ClearProgressBar ();
			#endif
		}
		
		public void DestroyColliders () {
			PlanetChunck[] chuncks = this.GetComponentsInChildren<PlanetChunck> ();
			
			foreach (PlanetChunck chunck in chuncks) {
				MeshCollider mc = chunck.gameObject.GetComponent<MeshCollider> ();
				if (mc != null) {
					DestroyImmediate (mc);
				}
			}
		}
	}
}