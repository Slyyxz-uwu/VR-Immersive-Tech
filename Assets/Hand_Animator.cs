using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Hand_Animator : MonoBehaviour
{

    [SerializeField] private NearFarInteractor nearFarInteractor;
    [SerializeField] private SkinnedMeshRenderer handMesh;

    private void Awake()
    {
        nearFarInteractor.selectEntered.AddListener(OnGrab);
        nearFarInteractor.selectExited.AddListener(OnRelease);


    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        Debug.Log("Selected");
        if (handMesh != null)
        {
            handMesh.enabled = false; // Hide the hand when grabbing
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        if (handMesh != null)
        {
            handMesh.enabled = true; // Show the hand again when released
        }
    }
}