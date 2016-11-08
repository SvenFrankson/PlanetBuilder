using UnityEngine;
using System.Collections;

public class RandomSeed
{

    private int seed = 42;
    private int RANDOMLENGTH = 1991;

    private float[] rands;
    private float[] Rands
    {
        get
        {
            if (this.rands == null)
            {
                Random.seed = this.seed;
                this.rands = new float[RANDOMLENGTH];
                for (int i = 0; i < RANDOMLENGTH; i++)
                {
                    this.rands[i] = Random.Range(-1f, 1f);
                }
            }

            return rands;
        }
    }

    public RandomSeed(int seed)
    {
        this.seed = seed;
    }

    public RandomSeed(string seedString)
    {
        this.seed = 0;
        foreach (char c in seedString)
        {
            this.seed += c;
        }
    }

    public int RandRange(int i, int min, int max)
    {
        return Mathf.FloorToInt((Rand(i) + 1f) / 2f * (max - min + 1f) + min);
    }

    public int RandRange(int i, int j, int k, int d, int min, int max)
    {
        return Mathf.FloorToInt((Rand(i, j, k, d) + 1f) / 2f * (max - min + 1f) + min);
    }

    public float Rand(int i)
    {
        int index = i * 41;

        return Rands[index % RANDOMLENGTH];
    }

    public float Rand(int i, int j, int k, int d)
    {
        int index = Mathf.Abs(i * 11 + j * 13 + k * 17 + d);
        return Rands[index % RANDOMLENGTH];
    }
}
