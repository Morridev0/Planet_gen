using UnityEngine;

public class Perlin_noise : MonoBehaviour
{
    public int pixWidth = 1024;
    public int pixHeight = 1024;
    public float xOrg;
    public float yOrg;
    public float scale = 20.0F;
    private Texture2D noiseTex;
    private Color[] pix;
    private Renderer rend;

    void Start()
    {
        rend = GetComponent<Renderer>();
        noiseTex = new Texture2D(pixWidth, pixHeight);
        pix = new Color[noiseTex.width * noiseTex.height];
        rend.material.SetTexture("_BaseMap", noiseTex);
    }

    void CalcNoise()
    {
        int y = 0;
        while (y < noiseTex.height)
        {
            int x = 0;
            while (x < noiseTex.width)
            {
                float xCoord = xOrg + (float)x / noiseTex.width * scale;
                float yCoord = yOrg + (float)y / noiseTex.height * scale;
                float sample = Mathf.PerlinNoise(xCoord, yCoord);
                pix[y * pixWidth + x] = GetBiomeColor(sample);
                //pix[y * noiseTex.width + x] = new Color(sample, sample, sample);
                x++;
            }
            y++;
        }
        noiseTex.SetPixels(pix);
        noiseTex.Apply();
    }

    Color GetBiomeColor(float value)
    {
        if (value < 0.30f)
            return new Color(0, 0.25f, 0.7f); // océano

        if (value < 0.40f)
            return new Color(0.9f, 0.85f, 0.5f); // playa

        if (value < 0.65f)
            return new Color(0.15f, 0.6f, 0.2f); // bosque


        if (value < 0.85f)
            return Color.gray; // montaña
    
        return Color.white; // nieve
    }

    void Update()
    {
        CalcNoise();
    }
}