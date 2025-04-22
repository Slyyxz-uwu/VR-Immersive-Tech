using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControl : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleMenu;
    [SerializeField] private GameObject uiCanvas;
    private bool uiActive = false;

    void Start()
    {
        toggleMenu.action.performed += ToggleMenuCanvas;
        uiCanvas.SetActive(false);
    }

    private void ToggleMenuCanvas(InputAction.CallbackContext context)
    {
        uiActive = !uiActive; // toggle the state
        uiCanvas.SetActive(uiActive);
    }
}