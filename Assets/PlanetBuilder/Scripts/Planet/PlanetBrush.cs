using UnityEngine;
using System.Collections;
using SvenFrankson.Game.SphereCraft;

public class PlanetBrush : MonoBehaviour {

    public Material[] planetMaterials;
    public Material[] eraserMaterials;

    private PlanetSide planetSide = null;
    private int iPos = -1;
    private int jPos = -1;
    private int kPos = -1;
    private byte block = 0;

    private MeshFilter c_MeshFilter;
    private MeshFilter C_MeshFilter
    {
        get
        {
            if (c_MeshFilter == null)
            {
                c_MeshFilter = this.GetComponent<MeshFilter>();
            }
            return c_MeshFilter;
        }
    }

    private Renderer c_Renderer;
    private Renderer C_Renderer
    {
        get
        {
            if (c_Renderer == null)
            {
                c_Renderer = this.GetComponent<Renderer>();
            }
            return c_Renderer;
        }
    }

    public void Set(PlanetSide newPlanetSide, int newIPos, int newJPos, int newKPos, byte newBlock)
    {
        if  ((newPlanetSide == this.planetSide) &&
            (newIPos == this.iPos) &&
            (newJPos == this.jPos) &&
            (newKPos == this.kPos) &&
            (newBlock == this.block))
        {
            return;
        }

        this.planetSide = newPlanetSide;
        this.iPos = newIPos;
        this.jPos = newJPos;
        this.kPos = newKPos;
        this.block = newBlock;

        this.C_MeshFilter.sharedMesh = this.planetSide.planet.WorldPositionToBlockMesh(this.iPos, this.jPos, this.kPos, this.block);

        if (this.block == 0)
        {
            this.C_Renderer.sharedMaterials = this.eraserMaterials;
        }
        else
        {
            this.C_Renderer.sharedMaterials = this.planetMaterials;
        }

        this.transform.parent = newPlanetSide.transform;
        this.transform.localPosition = Vector3.zero;
        this.transform.localRotation = Quaternion.identity;
    }
}
