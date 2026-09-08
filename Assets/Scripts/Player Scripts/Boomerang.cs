using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Boomerang : MonoBehaviour
{

    [SerializeField] private GameObject boomerangPrefab;
    [SerializeField] private Transform boomerangSpawnPoint;
    [SerializeField] private float force;
    
    
    
    
    private void Update()
    {
        // if (Keyboard.current.fKey.wasPressedThisFrame)
        // {
        //     Rigidbody boomerangRb = Instantiate(boomerangPrefab, boomerangSpawnPoint.position, boomerangSpawnPoint.rotation).GetComponent<Rigidbody>();
        //     boomerangRb.AddForce(boomerangRb.transform.forward * force);
        // }
        
        for (int i = 0; i < 20; i++)
        {
            Rigidbody boomerangRb = Instantiate(boomerangPrefab, boomerangSpawnPoint.position, boomerangSpawnPoint.rotation).GetComponent<Rigidbody>();
            boomerangRb.AddForce(boomerangRb.transform.forward * force);
        }
    }
}
