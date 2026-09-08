using System;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    /// <summary>
    ///
    ///     Makes the camera target follow the player
    /// 
    /// </summary>
    
    [Header("--- Referenecs ---")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private Transform player;
    [SerializeField] private Transform boomerangContainer;
    [SerializeField] private Transform camera;
    
    

    private void LateUpdate()
    {
        cameraTarget.position = player.position + new Vector3(0, 2, 0);
        boomerangContainer.rotation = camera.rotation;
    }
}
