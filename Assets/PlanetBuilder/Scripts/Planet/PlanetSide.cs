using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SvenFrankson.Game.SphereCraft {

	public class PlanetSide : MonoBehaviour {

		public Planet planet;
        public int Degree
        {
            get
            {
                return this.planet.degree;
            }
        }
        public int Size
        {
            get
            {
                return this.planet.size;
            }
        }
        public int RMin
        {
            get
            {
                return this.planet.rMin;
            }
        }
        public int WaterLevel
        {
            get
            {
                return this.planet.waterLevel;
            }
        }
        public string PlanetName
        {
            get
            {
                return this.planet.planetName;
            }
        }
        public Planet.Side side;

		public int nbChunks;
		public PlanetChunck[][][] chuncks;

        public Material[] Materials
        {
            get
            {
                return this.planet.planetMaterials;
            }
        }

        public void Initialize(Planet planet)
        {
            this.planet = planet;
			this.nbChunks = ((int) Mathf.Pow (2, this.Degree)) / PlanetUtility.ChunckSize;

			this.chuncks = new PlanetChunck[this.nbChunks][][];
            for (int iPos = 0; iPos < this.nbChunks; iPos++)
            {
                this.chuncks[iPos] = new PlanetChunck[this.nbChunks][];
                for (int jPos = 0; jPos < this.nbChunks; jPos++)
                {
                    this.chuncks[iPos][jPos] = new PlanetChunck[this.nbChunks / 2];
                    for (int kPos = 0; kPos < this.nbChunks / 2; kPos++)
                    {
                        Transform planetChunkChild = this.transform.Find(PlanetUtility.ChunckName(iPos, jPos, kPos));
                        if (planetChunkChild != null)
                        {
                            this.chuncks[iPos][jPos][kPos] = planetChunkChild.GetComponent<PlanetChunck>();
                        }
                        if (this.chuncks[iPos][jPos][kPos] == null)
                        {
                            this.chuncks[iPos][jPos][kPos] = PlanetUtility.InstantiatePlanetChunck(iPos, jPos, kPos, this);
                        }
                        this.chuncks[iPos][jPos][kPos].Initialize();
                    }
				}
			}
		}

        public void Clear()
        {
            while (this.transform.childCount > 0)
            {
                DestroyImmediate(this.transform.GetChild(0).gameObject);
            }
        }
	}
}
