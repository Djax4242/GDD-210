using System;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    /// <summary>
    ///
    ///     First person player controller
    /// 
    /// </summary>
    
    
    [Header("--- References ---")]
    [Tooltip("Used top get cam relative input")]
    [SerializeField] private Transform mainCameraTransform;
    [Tooltip("Rb on the player")]
    [SerializeField] private Rigidbody playerRb;
    [Tooltip("Gets input")]
    [SerializeField] private PlayerInput playerInput;
    
    [Space]
    [Header("--- Movement Settings ---")]
    [Tooltip("Base grounded move speed of the player")]
    [SerializeField] private float baseMoveSpeed;
    [Tooltip("Base speed gets multiplied by this when the player is trying to change direction")]
    [SerializeField] private float counterAccelerationMultiplier;
    [Tooltip("When the player stops moving, apply this amount of counter movement to slow them down")]
    [SerializeField] private float counterMovement;
    [Tooltip("Extra gravity gets applied at the end")]
    [SerializeField] private float gravity;
    [Tooltip("Top speed using linearveloctity magnitutde")]
    [SerializeField] private float topSpeed;
    private float _moveSpeed;
    
    [Space]
    [Header("--- Jump Settings ---")]
    [SerializeField] private float jumpForce;
    [SerializeField] private LayerMask invalidGroundedLayer;
    [SerializeField] private Vector3 spherecastOffset;
    [SerializeField] private float spherecastRadius;
    [SerializeField] private float spherecastDistance;
    private bool _isGrounded = true;
    
    
    
    private void Awake()
    {
        playerRb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void FixedUpdate()
    {
        Movement();
        CounterMovement();
        
        CheckForJump();
        CheckForGrounded();
    }

    private void Movement()
    {
        // Get input and set input to zero when above top speed
        Vector2 moveInput = playerInput.GetMoveInput();
        
        // Rotate the player rigidbody to face the cameras forward direction.
        // We flatten the Y to make sure the rigidbody doesn't tilt up and down
        Vector3 desiredForward = mainCameraTransform.forward;
        desiredForward.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(desiredForward);
        playerRb.MoveRotation(lookRotation);
        
        // After the rb is rotated, we use its foward and right to calulate an rb relative input
        Vector3 rbRelativeInput = (moveInput.x * playerRb.transform.right) + (moveInput.y * playerRb.transform.forward);

        // Counter movement that gets applied when the player is trying to change direction
        float desiredVsMove = Vector3.Dot(playerRb.linearVelocity.normalized, rbRelativeInput.normalized);
        if (desiredVsMove < 0) _moveSpeed = baseMoveSpeed * counterAccelerationMultiplier;
        else _moveSpeed = baseMoveSpeed;
        
        // Move the player using the rb relative input
        playerRb.AddForce(rbRelativeInput * (baseMoveSpeed * Time.deltaTime));

        // Clamp the players movement so they dont exceed the max speed
        Vector2 movementPlane = new Vector2(playerRb.linearVelocity.x, playerRb.linearVelocity.z);
        movementPlane = Vector2.ClampMagnitude(movementPlane, topSpeed);
        playerRb.linearVelocity = new Vector3(movementPlane.x, playerRb.linearVelocity.y, movementPlane.y);

        // Extra gravity
        playerRb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
    }

    private void CounterMovement()
    {
        // More counter movement that gets applied when the player stops moving to stop them from sliding around
        
        Vector2 moveInput = playerInput.GetMoveInput();
        if (moveInput.sqrMagnitude < 0.1f)
        {
            playerRb.AddForce(-playerRb.linearVelocity * (counterMovement * Time.deltaTime));
        }
    }

    private void CheckForJump()
    {
        if (playerInput.GetJumpInput() && _isGrounded)
        {
            _isGrounded = false;
            playerRb.AddForce(Vector3.up * jumpForce);
        }
    }

    private void CheckForGrounded()
    {
        if (Physics.SphereCast(playerRb.transform.position + spherecastOffset, spherecastRadius, Vector3.down, out RaycastHit hit, spherecastDistance, ~invalidGroundedLayer))
            _isGrounded = true;
        else
            _isGrounded = false;
    }

    
    
#if UNITY_EDITOR
    
    [Space]
    [Header("=== DEBUG SETTINGS ===")]
    [SerializeField] private bool showGizmos;
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        if (!Application.isPlaying) return;
        
        Vector2 moveInput = playerInput.GetMoveInput();
        Vector3 rbRelativeInput = (moveInput.x * playerRb.transform.right) + (moveInput.y * playerRb.transform.forward);
        
        Gizmos.color = Color.red;
        Vector3 spherePosition = playerRb.transform.position + spherecastOffset + new Vector3(0, -spherecastDistance, 0);
        Gizmos.DrawSphere(spherePosition, spherecastRadius);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(playerRb.position, playerRb.linearVelocity);

        Gizmos.color = Color.orangeRed;
        Gizmos.DrawRay(playerRb.position, rbRelativeInput);
    }
#endif
}
