using UnityEngine;
using System.Collections;
using SvenFrankson.Game.SphereCraft;

public class PlacerTest : MonoBehaviour {

    public Planet target;

    public void TestWorldPosTo()
    {
        Debug.Log("# TEST WORLD POS TO #");

        Vector3 position = this.transform.position;
        PlanetSide planetSide = target.WorldPositionToPlanetSide(position);

        Debug.Log("PlanetSide : " + planetSide.name);

        PlanetChunck planetChunck;
        int iPos, jPos, kPos;
        target.WorldPositionToIJK(position, out planetChunck, out iPos, out jPos, out kPos);

        Debug.Log("PlanetChunck : " + planetChunck.name);
        Debug.Log("IPos : " + iPos);
        Debug.Log("JPos : " + jPos);
        Debug.Log("KPos : " + kPos);

        byte data = target.WorldPositionToData(position);

        Debug.Log("Data : " + data);

        Debug.Log("# OVER : TEST WORLD POS TO #");
    }

    public void TestPutData()
    {
        Debug.Log("# TEST WORLD POS TO #");

        Vector3 position = this.transform.position;
        target.SetDataAtWorldPos(position, 130);

        Debug.Log("# OVER : TEST WORLD POS TO #");
    }
}
