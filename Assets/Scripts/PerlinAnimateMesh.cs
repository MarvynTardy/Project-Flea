using UnityEngine;
using System.Collections;

public class PerlinAnimateMesh : MonoBehaviour
{
    public float perlinScale = 4.56f;
    public float waveSpeed = 1f;
    public float waveHeight = 2f;

    public Vector3 l_NoiseTransform;

    // private Mesh mesh;

    void Update()
    {
        // AnimateMesh();
        AnimateTransform();
    }

    //void AnimateMesh()
    //{
    //    if (!mesh)
    //        mesh = GetComponent<MeshFilter>().mesh;

    //    Vector3[] vertices = mesh.vertices;

    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        float pX = (vertices[i].x * perlinScale) + (Time.timeSinceLevelLoad * waveSpeed);
    //        float pZ = (vertices[i].z * perlinScale) + (Time.timeSinceLevelLoad * waveSpeed);

    //        vertices[i].y = (Mathf.PerlinNoise(pX, pZ) - 0.5f) * waveHeight;
    //    }

    //    mesh.vertices = vertices;
    //}

    void AnimateTransform()
    {
        l_NoiseTransform = transform.localPosition;
        
        float pX = (l_NoiseTransform.x * perlinScale) + (Time.timeSinceLevelLoad * waveSpeed);
        float pZ = (l_NoiseTransform.z * perlinScale) + (Time.timeSinceLevelLoad * waveSpeed);

        // l_NoiseTransform.x = (Mathf.PerlinNoise(pX, pZ) - 0.5f) * waveHeight;
        l_NoiseTransform.y = (Mathf.PerlinNoise(pX, pZ) - 0.5f) * waveHeight;
        // l_NoiseTransform.z = (Mathf.PerlinNoise(pX, pZ) - 0.5f) * waveHeight;

        transform.localPosition = l_NoiseTransform;
    }
}