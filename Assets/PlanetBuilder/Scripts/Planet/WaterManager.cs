using UnityEngine;
using System.Collections;

public class WaterManager : MonoBehaviour {

	public float xSpeed = 0f;
	public float ySpeed = 0f;
	public Material waterMaterial;

	public void Update () {
		waterMaterial.mainTextureOffset += new Vector2 (this.xSpeed * Time.deltaTime, this.ySpeed * Time.deltaTime);

		if (waterMaterial.mainTextureOffset.x > 1f) {
			waterMaterial.mainTextureOffset = new Vector2 (0f, waterMaterial.mainTextureOffset.y);
		}

		if (waterMaterial.mainTextureOffset.y > 1f) {
			waterMaterial.mainTextureOffset = new Vector2 (waterMaterial.mainTextureOffset.x, 0f);
		}
	}
}
