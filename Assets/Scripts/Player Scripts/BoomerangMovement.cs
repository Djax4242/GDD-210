using System;
using System.Collections;
using UnityEngine;

public class BoomerangMovement : MonoBehaviour
{
    /// <summary>
    ///
    ///     Handles the boomerang movement when its thrown
    ///     Handles Visual rotation when the boomerang is thrown
    /// 
    /// </summary>
    
    
    [Header("--- References ---")]
    [Tooltip("Visual of the boomerang so we can rotate it")]
    [SerializeField] private Transform boomerangVisual;
    [Tooltip("Need this so we can disable the boomerangs collider when its held")]
    [SerializeField] private Collider collider;
    [Tooltip("This is where the boomerang will try to return to")]
    [SerializeField] private Transform holdPoint;
    [Tooltip("Rigidbody of the boomerang")]
    [SerializeField] private Rigidbody boomerangRb;
    [SerializeField] private TrailRenderer trailRenderer;
    
    [Space]
    [Header("--- Boomerang Movement Settings ---")]
    [Tooltip("How fast the boomerang visual rotates")]
    [SerializeField] private float visualRotationSpeed;
    [Tooltip("How far away the boomerang needs to be to be collected")]
    [SerializeField] private float collectionDistance;
    [Tooltip("How aggressively the boomerang will try to return to the player")]
    [SerializeField] private float returnForce;
    [Tooltip("How long the boomerang cant be picked up")]
    [SerializeField] private float pickupInvulnerabilityTime;
    // Is the boomerang currently out
    private bool isBoomerangOut;
    // Can the boomerang be picked up
    private bool canBoomerangBePickedUp;
    
    
    
    private void OnEnable()
    {
        PlayerEvents.OnBoomerangThrown += Thrown;
    }

    private void OnDisable()
    {
        PlayerEvents.OnBoomerangThrown -= Thrown;
    }

    private void Update()
    {
        if (!isBoomerangOut) return;
        boomerangVisual.transform.Rotate(0, visualRotationSpeed * Time.deltaTime, 0, Space.Self);
    }

    private void FixedUpdate()
    {
        if (!isBoomerangOut) return;
        
        if (Vector3.Distance(boomerangRb.position, holdPoint.position) < collectionDistance && canBoomerangBePickedUp)
        {
            Collected();
            boomerangRb.linearVelocity = Vector3.zero;
            canBoomerangBePickedUp = false;
            boomerangRb.transform.SetParent(holdPoint, true);

            boomerangRb.interpolation = RigidbodyInterpolation.None;
            
            transform.localPosition = Vector3.zero;
        }
        
        // Force the boomerang to try to return to the player
        boomerangRb.AddForce((holdPoint.position - boomerangRb.position) * returnForce);
    }

    private void Thrown()
    {
        isBoomerangOut = true;
        collider.enabled = true;
        trailRenderer.enabled = true;
        StartCoroutine(PickupInvulnerability());
    }

    private void Collected()
    {
        isBoomerangOut = false;
        collider.enabled = false;
        trailRenderer.enabled = false;
    }

    private IEnumerator PickupInvulnerability()
    {
        yield return new WaitForSeconds(pickupInvulnerabilityTime);
        canBoomerangBePickedUp = true;
    }
}
