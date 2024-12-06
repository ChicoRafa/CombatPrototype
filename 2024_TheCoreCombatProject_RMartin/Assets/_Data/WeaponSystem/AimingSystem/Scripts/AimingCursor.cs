using UnityEngine;
using UnityEngine.InputSystem;

public class AimingCursor : MonoBehaviour
{
    [SerializeField] private LayerMask LayerMask = Physics.DefaultRaycastLayers;
    Camera mainCamera;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void Update()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray mousePositionRay = mainCamera.ScreenPointToRay(mousePosition);
        
        if(Physics.Raycast(mousePositionRay, out RaycastHit hit, Mathf.Infinity, LayerMask))
        {
            transform.position = hit.point;
        }
    }
}
