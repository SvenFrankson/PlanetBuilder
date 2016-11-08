using UnityEngine;
using System.Collections;
using SvenFrankson.Game.SphereCraft;
using System.Collections.Generic;

public class PlayerTest : MonoBehaviour {

    public Transform head;
    public Planet target;
    public Rigidbody c_Rigidbody;
    public Rigidbody C_Rigidbody
    {
        get
        {
            if (c_Rigidbody == null)
            {
                c_Rigidbody = this.GetComponent<Rigidbody>();
            }
            return c_Rigidbody;
        }
    }
    public bool blockPainting = true;
    public PlanetBrush brush;
    public byte block;
    private float rayOffset;

    public Vector2 scrollBlockPosition;
    public Texture2D blocksTexture;
    public int[] blocks;
    public Texture2D[] blocksIcons;

    public int xPosBlockGUI;
    public int yPosBlockGUI;
    public int widthBlockGUI;
    public int heightBlockGUI;
    private Rect blockGUI;

    public int xPosOutputGUI;
    public int yPosOutputGUI;
    public int widthOutputGUI;
    public int heightOutputGUI;
    private Rect outputGUI;

    public void Start()
    {
        blockGUI = new Rect(Screen.width * xPosBlockGUI / 100f, Screen.height * yPosBlockGUI / 100f,
                            Screen.width * widthBlockGUI / 100f, Screen.height * heightBlockGUI / 100f);

        outputGUI = new Rect(   Screen.width * xPosOutputGUI / 100f, Screen.height * yPosOutputGUI / 100f,
                                Screen.width * widthOutputGUI / 100f, Screen.height * heightOutputGUI / 100f);

        this.SetBlockIcon();
    }

	void Update () {
        if (!Input.GetMouseButton(1))
        {
            if (Input.GetKey(KeyCode.Z))
            {
                this.transform.position += 20f * Time.deltaTime * this.transform.forward;
            }
            if (Input.GetKey(KeyCode.S))
            {
                this.transform.position -= 20f * Time.deltaTime * this.transform.forward;
            }
            if (Input.GetKey(KeyCode.Q))
            {
                this.transform.rotation = Quaternion.AngleAxis(90f * Time.deltaTime, this.transform.forward) * this.transform.rotation;
            }
            if (Input.GetKey(KeyCode.D))
            {
                this.transform.rotation = Quaternion.AngleAxis(-90f * Time.deltaTime, this.transform.forward) * this.transform.rotation;
            }
        }
        
        if (Input.GetMouseButton(1))
        {
            this.transform.rotation = Quaternion.AngleAxis(- 5f * Input.GetAxis("Mouse X"), this.transform.up) * this.transform.rotation;
            this.head.rotation = Quaternion.AngleAxis(5f * Input.GetAxis("Mouse Y"), this.transform.right) * this.head.rotation;
        }
        if (Input.GetMouseButtonDown(0))
        {
            if (!MouseOverGUI())
            {
                this.PutDataAtMouse();
                //this.PutTreeAtMouse();
            }
        }
        BrushPaintingUpdate();
        this.transform.RotateAround(this.transform.position, Vector3.Cross(this.transform.up, (this.transform.position - this.target.transform.position).normalized), Vector3.Angle((this.transform.position - this.target.transform.position).normalized, this.transform.up));
	}

    public void FixedUpdate()
    {
        C_Rigidbody.AddForce(-(this.transform.position - this.target.transform.position).normalized * 9f);
    }

    public void OnGUI()
    {
        GUILayout.BeginArea(blockGUI);
        scrollBlockPosition = GUILayout.BeginScrollView(scrollBlockPosition);
        for (int i = 0; i < this.blocks.Length; i++)
        {
            if (GUILayout.Button(this.blocksIcons[i], GUILayout.Height(Screen.width * widthBlockGUI / 100f * 0.7f), GUILayout.Width(Screen.width * widthBlockGUI / 100f * 0.7f)))
            {
                this.block = (byte)this.blocks[i];
            }
        }
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUILayout.BeginArea(outputGUI);
        GUILayout.TextArea("Planet Chunck Instances : " + PlanetChunckManager.Instances.Count);
        GUILayout.TextArea("Planet Chunck Update Time : " + PlanetChunckManager.Instance.updateTime);
        GUILayout.EndArea();
    }

    public bool MouseOverGUI()
    {
        if (Input.mousePosition.x > blockGUI.xMin)
        {
            if (Input.mousePosition.x < blockGUI.xMax)
            {
                if (Screen.height - Input.mousePosition.y > blockGUI.yMin)
                {
                    if (Screen.height - Input.mousePosition.y < blockGUI.yMax)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    public void PutDataAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldPos = hit.point + ray.direction.normalized * this.rayOffset;

            target.SetDataAtWorldPos(worldPos, this.block);
        }
    }

    public void BrushPaintingUpdate()
    {
        if (!blockPainting)
        {
            brush.gameObject.SetActive(false);
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                brush.gameObject.SetActive(true);

                this.rayOffset = -0.05f;
                if (block == 0)
                {
                    this.rayOffset = 0.05f;
                }
                Vector3 worldPos = hit.point + ray.direction.normalized * this.rayOffset;
                PlanetSide planetSide;
                int iPos, jPos, kPos;
                target.WorldPositionToIJKPos(worldPos, out planetSide, out iPos, out jPos, out kPos);

                brush.Set(planetSide, iPos, jPos, kPos, block);
            }
            else
            {
                brush.gameObject.SetActive(false);
            }
        }
    }

    #region tree
    public void PutTreeAtMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 worldPos = hit.point + ray.direction.normalized * this.rayOffset;
            PlanetSide planetSide;
            int iPos, jPos, kPos;
            target.WorldPositionToIJKPos(worldPos, out planetSide, out iPos, out jPos, out kPos);

            CreateTree(planetSide, iPos, jPos, kPos);
        }
    }

    public void CreateTree(PlanetSide planetSide, int iPos, int jPos, int kPos)
    {
        List<PlanetChunck> planetChuncks = new List<PlanetChunck>();
        int h = Random.Range(4, 12);

        Debug.Log("H = " + h);
        int bpi = Random.Range(1, h / 2);
        Debug.Log("BPI = " + bpi);
        int bmi = Random.Range(1, h / 2);
        Debug.Log("BMI = " + bmi);
        int bpj = Random.Range(1, h / 2);
        Debug.Log("BPJ = " + bpj);
        int bmj = Random.Range(1, h / 2);
        Debug.Log("BMJ = " + bmj);
        int bup = Random.Range(1, h / 2);
        Debug.Log("BUP = " + bup);

        int[][] allSides = new int[14][];
        allSides[0] = new int[] { 1, 0, 0 };
        allSides[1] = new int[] { -1, 0, 0 };
        allSides[2] = new int[] { 0, 1, 0 };
        allSides[3] = new int[] { 0, -1, 0 };
        allSides[4] = new int[] { 0, 0, 1 };
        allSides[5] = new int[] { 0, 0, -1 };

        allSides[6] = new int[] { 1, 0, -1 };
        allSides[7] = new int[] { -1, 0, -1 };
        allSides[8] = new int[] { 0, 1, -1 };
        allSides[9] = new int[] { 0, -1, -1 };

        allSides[10] = new int[] { 1, 1, 0 };
        allSides[11] = new int[] { -1, 1, 0 };
        allSides[12] = new int[] { -1, 1, 0 };
        allSides[13] = new int[] { -1, -1, 0 };

        for (int k = 0; k <= h; k++)
        {
            PlanetChunck planetChunck = target.SetDataAtIJKPos(planetSide, iPos, jPos, kPos + k, 135, false, false, false);
            if (planetChunck != null)
            {
                if (!planetChuncks.Contains(planetChunck))
                {
                    planetChuncks.Add(planetChunck);
                }
            }
        }
        for (int k = 0; k <= bup; k++)
        {
            PlanetChunck planetChunck = target.SetDataAtIJKPos(planetSide, iPos, jPos, kPos + h + k, 135, false, false, false);
            if (planetChunck != null)
            {
                if (!planetChuncks.Contains(planetChunck))
                {
                    planetChuncks.Add(planetChunck);
                }
            }
            for (int a = 0; a < allSides.Length; a++)
            {
                planetChunck = target.SetDataAtIJKPos(planetSide, iPos + allSides[a][0], jPos + allSides[a][1], kPos + h + k + allSides[a][2], 1, true, false, false);
                if (planetChunck != null)
                {
                    if (!planetChuncks.Contains(planetChunck))
                    {
                        planetChuncks.Add(planetChunck);
                    }
                }
            }
        }
        for (int i = -bmi; (i <= bpi); i++)
        {
            PlanetChunck planetChunck = target.SetDataAtIJKPos(planetSide, iPos + i, jPos, kPos + h, 135, false, false, false);
            if (planetChunck != null)
            {
                if (!planetChuncks.Contains(planetChunck))
                {
                    planetChuncks.Add(planetChunck);
                }
            }
            for (int a = 0; a < allSides.Length; a++)
            {
                planetChunck = target.SetDataAtIJKPos(planetSide, iPos + i + allSides[a][0], jPos + allSides[a][1], kPos + h + allSides[a][2], 1, true, false, false);
                if (planetChunck != null)
                {
                    if (!planetChuncks.Contains(planetChunck))
                    {
                        planetChuncks.Add(planetChunck);
                    }
                }
            }
        }
        for (int j = -bmj; (j <= bpj); j++)
        {
            PlanetChunck planetChunck = target.SetDataAtIJKPos(planetSide, iPos, jPos + j, kPos + h, 135, false, false, false); ;
            if (planetChunck != null)
            {
                if (!planetChuncks.Contains(planetChunck))
                {
                    planetChuncks.Add(planetChunck);
                }
            }
            for (int a = 0; a < allSides.Length; a++)
            {
                planetChunck = target.SetDataAtIJKPos(planetSide, iPos + allSides[a][0], jPos + j + allSides[a][1], kPos + h + allSides[a][2], 1, true, false, false);
                if (planetChunck != null)
                {
                    if (!planetChuncks.Contains(planetChunck))
                    {
                        planetChuncks.Add(planetChunck);
                    }
                }
            }
        }

        foreach (PlanetChunck p in planetChuncks)
        {
            p.SetMesh();
            PlanetUtility.Save(p.PlanetName, p.data, p.iPos, p.jPos, p.kPos, p.planetSide.side);
        }
    }
    #endregion

    #region blockIcon
    private void SetBlockIcon()
    {
        int textureSize = this.blocksTexture.width;
        this.blocksIcons = new Texture2D[this.blocks.Length];
        for (int i = 0; i < this.blocksIcons.Length; i++)
        {
            if (this.blocks[i] != 0)
            {
                int tileSetSize = 8;
                int blockIndex = this.blocks[i] - 1;
                if (this.blocks[i] >= 128)
                {
                    blockIndex = this.blocks[i] - 128 + tileSetSize * tileSetSize / 2;
                }

                int x = textureSize / tileSetSize * (blockIndex % tileSetSize);
                int y = textureSize - textureSize / tileSetSize * (blockIndex / tileSetSize + 1);
                int height = textureSize / tileSetSize;
                int width = height;

                Debug.Log("x:" + x + " y:" + y + " width:" + width + " height:" + height);

                Color[] pixels = this.blocksTexture.GetPixels(x, y, width, height);
                this.blocksIcons[i] = new Texture2D(height, width, TextureFormat.ARGB32, false);
                this.blocksIcons[i].SetPixels(pixels);
                this.blocksIcons[i].Apply();
            }
            else
            {
                this.blocksIcons[i] = new Texture2D(32, 32);
            }
        }
    }
    #endregion
}
