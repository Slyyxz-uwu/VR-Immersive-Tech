using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControl : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleMenu;
    [SerializeField] private GameObject uiCanvas;
    private bool uiActive = false;

    // Start is called before the first frame update
    void Start()
    {
        toggleMenu.action.performed += ToggleMenuCanvas;
        uiCanvas.SetActive(false);
    }

    private void ToggleMenuCanvas(InputAction.CallbackContext context)

    {
        if (uiActive)
        {
            uiActive = true;
        }
        else
        {
            uiActive = false;
        }
        uiCanvas.SetActive(uiActive);
    }
}