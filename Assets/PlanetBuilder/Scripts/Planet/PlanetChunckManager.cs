using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using SvenFrankson.Game.SphereCraft;

public class PlanetChunckManager : MonoBehaviour {

    static private PlanetChunckManager instance;
    static public PlanetChunckManager Instance
    {
        get
        {
            if (PlanetChunckManager.instance == null)
            {
                PlanetChunckManager.instance = FindObjectOfType<PlanetChunckManager>();
            }
            return PlanetChunckManager.instance;
        }
    }
    public Transform Referential;
    public static List<PlanetChunck> Instances = new List<PlanetChunck>();
    private int cursor = 0;
    private int anglesComputeByFrame = 20;
    public float updateTime = 0f;
    public bool workingLock = false;

    public void Start()
    {
        this.workingLock = false;
    }

    public void Update()
    {
        float t0 = Time.realtimeSinceStartup;
        float t1 = Time.realtimeSinceStartup;
        for (int i = 0; i < anglesComputeByFrame; i++)
        {
            cursor = (cursor + 1) % Instances.Count;
            PlanetChunck planetChunck = Instances[cursor];
            Instances[cursor].AngleToReferential = Vector3.Angle(PlanetChunckManager.Instance.Referential.position - planetChunck.transform.position, planetChunck.transform.TransformVector(planetChunck.LocalUp));
        }
        Instances = Instances.OrderBy(p => p.AngleToReferential).ToList();

        if (workingLock)
        {
            return;
        }

        foreach (PlanetChunck instance in Instances)
        {
            if (!instance.MeshSet)
            {
                if (instance.AngleToReferential < 90f)
                {
                    instance.SetMesh();
                    t1 = Time.realtimeSinceStartup;
                    updateTime = t1 - t0;
                    return;
                }
            }
        }
        t1 = Time.realtimeSinceStartup;
        updateTime = t1 - t0;
    }
}
