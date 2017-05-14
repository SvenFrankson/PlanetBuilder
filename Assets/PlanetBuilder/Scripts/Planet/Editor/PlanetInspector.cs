using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace SvenFrankson.Game.SphereCraft {
	
	[CustomEditor (typeof (Planet))]
	public class PlanetInspector : Editor {

        private Dictionary<Planet.Side, bool> generated;
		
		private Planet storedTarget;
		private Planet Target {
			get {
				if (this.storedTarget == null) {
                    
					this.storedTarget = (Planet) target;
				}
				
				return storedTarget;
			}
		}

        private int planetNameIndex = 0;
        private List<string> planetNames = null;
        private List<string> PlanetNames
        {
            get
            {
                if (this.planetNames == null)
                {
                    this.planetNames = new List<string>(PlanetUtility.GetPlanetList());
                }

                return this.planetNames;
            }
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            this.planetNameIndex = this.PlanetNames.IndexOf(Target.planetName);
            this.planetNameIndex = EditorGUILayout.Popup("Planet Name", this.planetNameIndex, this.PlanetNames.ToArray());
            if (GUI.changed)
            {
                Target.planetName = this.PlanetNames[this.planetNameIndex];
            }
            if (GUILayout.Button("Initialize"))
            {
                Target.Initialize();
            }
            foreach (Planet.Side side in Enum.GetValues(typeof(Planet.Side)))
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(side + ":");
                if (GUILayout.Button("Generate"))
                {
                    Target.Initialize();
                    GenerateSide(side);
                }
                if (GUILayout.Button("Clear"))
                {
                    ClearSide(side);
                }
                EditorGUILayout.EndHorizontal();
            }
            if (GUILayout.Button("GenerateAll"))
            {
                Target.Initialize();
                GenerateAllSides();
            }
            if (GUILayout.Button("ClearAll"))
            {
                Target.Clear();
            }
        }

        private void GenerateAllSides()
        {
            foreach (Planet.Side side in Enum.GetValues(typeof(Planet.Side)))
            {
                GenerateSide(side);
            }
        }

        private void GenerateSide(Planet.Side side) 
        {
            PlanetSide planetSide = Target.planetSides[side];
            planetSide.Initialize(Target);

            for (int i = 0; i < planetSide.nbChunks; i++)
            {
                for (int j = 0; j < planetSide.nbChunks; j++)
                {
                    for (int k = 0; k < planetSide.nbChunks / 2; k++)
                    {
                        chuncksToInstantiate.Add(new ChunckDataToInstantiate(i, j, k, planetSide));
                    }
                }
            }
            EditorApplication.update -= InstantiatePlanetChunck;
            EditorApplication.update += InstantiatePlanetChunck;
        }

        private void ClearSide(Planet.Side side)
        {
            Target.planetSides[side].Clear();
        }

        private class ChunckDataToInstantiate
        {
            public int iPos;
            public int jPos;
            public int kPos;
            public PlanetSide side;

            public ChunckDataToInstantiate(int iPos, int jPos, int kPos, PlanetSide side)
            {
                this.iPos = iPos;
                this.jPos = jPos;
                this.kPos = kPos;
                this.side = side;
            }
        }

        private static List<ChunckDataToInstantiate> chuncksToInstantiate = new List<ChunckDataToInstantiate>();

        private static void InstantiatePlanetChunck()
        {
            if (chuncksToInstantiate.Count == 0)
            {
                EditorApplication.update -= InstantiatePlanetChunck;
            }
            else
            {
                ChunckDataToInstantiate chunck = chuncksToInstantiate[0];
                chuncksToInstantiate.RemoveAt(0);
                chunck.side.chuncks[chunck.kPos][chunck.iPos][chunck.jPos].SetMesh(false);
            }
        }
	}
}