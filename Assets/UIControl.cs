using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UIControl : MonoBehaviour
{
    [SerializeField] private InputActionReference toggleMenu;
    [SerializeField] private GameObject uiCanvas;
    private bool uiActive = false;

    private void OnEnable()
    {
        toggleMenu.action.performed += ToggleMenuCanvas;
        toggleMenu.action.Enable(); // important!
    }

    private void OnDisable()
    {
        toggleMenu.action.performed -= ToggleMenuCanvas;
        toggleMenu.action.Disable();
    }

    private void Start()
    {
        uiCanvas.SetActive(false); // Hide UI at start
    }

    private void ToggleMenuCanvas(InputAction.CallbackContext context)
    {
        uiActive = !uiActive; // <-- this is the fix
        uiCanvas.SetActive(uiActive);
    }
}