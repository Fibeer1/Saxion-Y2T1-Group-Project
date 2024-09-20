using UnityEngine;
using System.Collections;

public class TextureCreator : MonoBehaviour
{
    public Material mat;

    public struct Circle
    {
        public Vector3 center;
        public float radius;

        public Circle(Vector3 center, float radius)
        {
            this.center = center;
            this.radius = radius;
        }
    }

    public Circle[] circles;
    public float smoother = 0.01f;
    public GameObject targetObject;
    public Vector2 textureSize = new Vector2(1024, 1024);

    private void Start()
    {
        circles = new Circle[]
        {
            new Circle(new Vector3(0.5f, 0.5f), 0.1f), //Example circle
        };
        StartCoroutine(UpdateTexture());

    }

    private void Update()
    {
        if (targetObject != null)
        {
            // Update the first circle's center to the target object's world position
            Vector3 worldPosition = targetObject.transform.position;
            circles[0].center = new Vector2(worldPosition.x / textureSize.x, worldPosition.z / textureSize.y); // Convert to UV coordinates
        }
    }

    private IEnumerator UpdateTexture()
    {
        while (true)
        {
            CreateTexture();
            yield return new WaitForSeconds(1);
        }
    }

    private void CreateTexture()
    {
        //Renderer rend = GetComponent<Renderer>();

        // Duplicate the original texture and assign to the material
        Texture2D texture = new Texture2D(1024, 1024, TextureFormat.RGBA32, false);

        Color[] cols = new Color[texture.width * texture.height];
        for (int i = 0; i < cols.Length; ++i)
        {
            int x = i % texture.width;
            int y = i / texture.width;
            Vector2 uv = new Vector2(x * 1f / texture.width, y * 1f / texture.height);

            foreach (Circle circle in circles)
            {
                float dist = Vector2.Distance(uv, circle.center);

                //If the pixel is close enough to the edge, smooth it out
                if (dist < circle.radius)
                {
                    float fade = Mathf.InverseLerp(0, circle.radius, dist);

                    fade = Mathf.SmoothStep(0, 1, fade);
                    cols[i] = Color.Lerp(Color.white, Color.black, fade);
                }
            }
        }
        texture.SetPixels(cols);
        texture.Apply(false);
        mat.SetTexture("_VisibilityTexture", texture);
    }
}