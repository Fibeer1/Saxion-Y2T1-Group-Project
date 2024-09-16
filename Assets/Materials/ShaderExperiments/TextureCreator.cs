using UnityEngine;
using System.Collections;

public class TextureCreator : MonoBehaviour
{
    public Material mat;

    public Vector2[] circleCenters; // for now: uv coordinates
    public float[] circleRadii; // ugly ; make circle struct instead

    void Start()
    {
        //Renderer rend = GetComponent<Renderer>();

        // Duplicate the original texture and assign to the material
        Texture2D texture = new Texture2D(2048, 2048, TextureFormat.RGBA32, false);
        //rend.material.mainTexture = texture;

        // Create the colors to use
        Color[] colors = new Color[3];
        colors[0] = Color.red;
        colors[1] = Color.green;
        colors[2] = Color.blue;
        int mipCount = Mathf.Min(3, texture.mipmapCount);

        // For each mipmap level, use GetPixels to fetch an array of pixel data, and use SetPixels to fill the mipmap level with one color.
        for (int mip = 0; mip < mipCount; ++mip)
        {
            Color[] cols = texture.GetPixels(mip);
            for (int i = 0; i < cols.Length; ++i)
            {
                int x = i % texture.width;
                int y = i / texture.width;
                Vector2 uv = new Vector2(x * 1f / texture.width, y * 1f / texture.height);

                cols[i] = Color.black;
                for (int j = 0; j < circleCenters.Length; j++)
                {
                    if ((uv - circleCenters[j]).magnitude < circleRadii[j])
                    {
                        float dist = Vector2.Distance(uv, circleCenters[j]);
                        dist = Mathf.Clamp(dist, 0, 1);
                        cols[i] = Color.Lerp(Color.white, Color.black, dist);
                    }
                }

                //cols[i] = new Color(Random.value, Random.value, Random.value);  //Color.Lerp(cols[i], colors[mip], 0.33f);
            }
            texture.SetPixels(cols, mip);
        }
        // Copy the changes to the GPU. and don't recalculate mipmap levels.
        texture.Apply(false);

        mat.SetTexture("_VisibilityTexture", texture);
    }
}