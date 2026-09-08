using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class BoomerangThrowing : MonoBehaviour
{
    /// <summary>
    ///
    ///     Handles the throwing of the boomerang
    /// 
    /// </summary>
    
    
    
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private GameObject splittingRang;
    [SerializeField] private Transform boomerangContainer;
    [SerializeField] private float throwForce;
    private bool isBoomerangOut;
    
    
    
    private void Update()
    {
        if (isBoomerangOut) return;
        
        if (playerInput.GetThrowInput())
        {
            splittingRang.transform.SetParent(boomerangContainer, true);
            Rigidbody splittingRangRb = splittingRang.GetComponent<Rigidbody>();
            splittingRangRb.interpolation = RigidbodyInterpolation.Interpolate;
            splittingRangRb.AddForce(splittingRangRb.transform.forward * throwForce, ForceMode.Impulse);
            PlayerEvents.BoomerangThrown();

            isBoomerangOut = true;
        }
    }
}
