using UnityEngine;
using UnityEngine.InputSystem; 
public class ClickToBuild : MonoBehaviour
{
    [SerializeField] private LayerMask buildPadMask;

    void Update()
    {
        // Left mouse click (new input system)
        if (Mouse.current == null) return;
        if (!Mouse.current.leftButton.wasPressedThisFrame) return;

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, buildPadMask))
        {
            BuildPad pad = hit.collider.GetComponent<BuildPad>();
            if (pad != null)
                pad.TryBuild();
        }
    }
}