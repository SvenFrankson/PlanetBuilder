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

			this.chuncks = new PlanetChunck[7][][];
			for (int kPos = 0; kPos <= 6; kPos++)
			{
				int chuncksCount = PlanetUtility.DegreeToChuncksCount (PlanetUtility.KPosToDegree (kPos));
				this.chuncks [kPos] = new PlanetChunck[chuncksCount][];
				for (int iPos = 0; iPos < chuncksCount; iPos++)
				{
					this.chuncks[kPos][iPos] = new PlanetChunck[chuncksCount];
					for (int jPos = 0; jPos < chuncksCount; jPos++)
					{
						Transform planetChunkChild = this.transform.Find(PlanetUtility.ChunckName(iPos, jPos, kPos));
						if (planetChunkChild != null)
						{
							this.chuncks[kPos][iPos][jPos] = planetChunkChild.GetComponent<PlanetChunck>();
						}
						if (this.chuncks[kPos][iPos][jPos] == null)
						{
							this.chuncks[kPos][iPos][jPos] = PlanetUtility.InstantiatePlanetChunck(iPos, jPos, kPos, this);
						}
						this.chuncks[kPos][iPos][jPos].Initialize();
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
