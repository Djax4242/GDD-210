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
    [SerializeField] private Transform mainCameraTransform;
    [SerializeField] private Rigidbody playerRb;
    [SerializeField] private PlayerInput playerInput;
    
    [Space]
    [Header("--- Movement Settings ---")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float gravity;
    [SerializeField] private float topSpeed;
    
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
        CheckForJump();
        CheckForGrounded();
    }

    private void Movement()
    {
        // Get input and set input to zero when above top speed
        Vector2 moveInput = playerInput.GetMoveInput();
        if (playerRb.linearVelocity.magnitude > topSpeed) moveInput = Vector2.zero;
        
        // Rotate the player rigidbody to face the cameras forward direction.
        // We flatten the Y to make sure the rigidbody doesn't tilt up and down
        Vector3 desiredForward = mainCameraTransform.forward;
        desiredForward.y = 0;
        Quaternion lookRotation = Quaternion.LookRotation(desiredForward);
        playerRb.MoveRotation(lookRotation);
        
        // Move the player using the rb relative input
        Vector3 rbRelativeInput = (moveInput.x * playerRb.transform.right) + (moveInput.y * playerRb.transform.forward);
        playerRb.AddForce(rbRelativeInput * (moveSpeed * Time.deltaTime));

        // Extra gravity
        playerRb.AddForce(Vector3.down * gravity, ForceMode.Acceleration);
        
        print(playerRb.linearVelocity.magnitude);
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 spherePosition = playerRb.transform.position + spherecastOffset + new Vector3(0, -spherecastDistance, 0);
        Gizmos.DrawSphere(spherePosition, spherecastRadius);
    }
}
