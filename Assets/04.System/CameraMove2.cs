using DG.Tweening;
using System.Collections;
using UnityEngine;

public class CameraMove2 : MonoBehaviour
{
    public GameObject CameraPos;
    public Camera camera;
    public float initialScale = 1f;
    public float size;
    private void Awake()
    {
        camera = GetComponent<Camera>();
    }
    public void CameraFollow(float size)
    {
        
    }

    private void Update()
    {
        if(camera.orthographicSize - initialScale * size > 0.01f)
            camera.orthographicSize = Mathf.Lerp(camera.orthographicSize, initialScale * size, 0.1f);
        else
        {
            camera.orthographicSize = initialScale * size;
        }
    }
}
