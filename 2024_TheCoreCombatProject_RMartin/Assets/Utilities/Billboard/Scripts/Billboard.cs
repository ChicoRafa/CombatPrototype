using UnityEngine;

public class Billboard : MonoBehaviour
{
    Camera mainCamera;
    
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(-mainCamera.transform.forward);
    }
}
