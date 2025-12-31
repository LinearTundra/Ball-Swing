using System.Runtime.CompilerServices;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(SphereCollider))]
public class BallMovement : MonoBehaviour
{

    [Header("Ball Properties")]
    [SerializeField]
    private int swingStrength;
    [SerializeField]
    private int spinStrength;
    [SerializeField]
    private Transform startPosition;

    public bool Throwable;

    private Rigidbody rb;

    private Vector3 forward = new Vector3(0, 0, 1);
    private float effectStrength;
    /// <summary>
    /// true for right, false for left
    /// </summary>
    private bool effectDirection;
    private bool hitGround;


    private void Start()
    {
        transform.position = startPosition.position + Vector3.up;

        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        Deactivate();
    }

    private void OnCollisionEnter(Collision other)
    {
        hitGround = true;
#if UNITY_EDITOR
        EditorApplication.isPaused = true;
#endif
    }

    public void ThrowBall(Vector3 target, BallEffect effect, bool toRight, float effectStrength)
    {
        Throwable = false;
        hitGround = false;
        this.effectStrength = effectStrength;
        effectDirection = toRight;
        rb.useGravity = true;
        Vector3 direction = CalculateThrowDirection(target, effect);
        rb.AddForce(direction, ForceMode.Impulse);

        if (effect == BallEffect.None) return;
        else if (effect == BallEffect.Swing) SwingBall();
        else SpinBall();
    }

    private async void SwingBall()
    {
        while (!hitGround)
        {
            AddSwing();
            await Awaitable.FixedUpdateAsync();
        }
        ResetBall();
    }

    private async void SpinBall()
    {
        while (!hitGround)
        {
            await Awaitable.FixedUpdateAsync();
        }

        Vector3 spinVelocity = new Vector3(-spinStrength * effectStrength, 0, 0);
        if (effectDirection) spinVelocity *= -1;

        rb.AddForce(spinVelocity, ForceMode.Impulse);
        ResetBall();
    }

    private void AddSwing()
    {
        Vector3 swingVelocity = new Vector3(-(float)swingStrength/10 * effectStrength, 0, 0);
        if (effectDirection) swingVelocity *= -1;

        rb.AddForce(swingVelocity, ForceMode.Impulse);
    }

    private async void ResetBall()
    {
        await Awaitable.WaitForSecondsAsync(3);
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.constraints = RigidbodyConstraints.None;
        transform.position = startPosition.position + Vector3.up;
        Deactivate();
    }

    private Vector3 CalculateThrowDirection(Vector3 targetPosition, BallEffect ballEffect)
    {
        Vector3 gravity = Physics.gravity;
        float g = gravity.y;

        // position at time t = startposition + velocity*time + (acceleration*time^2)/2
        // 
        // acceleration only in y so we can solve for time in y axis
        // final position(0) = initialposition(startposition) + initialvelocity(0)*time + 0.5*gravity*time^2
        // => 0 = y + 0 - 0.5*9,81*time^2
        //
        // solve for time
        float deltaY = -startPosition.position.y;

        // Solve time of flight from vertical motion
        float t = Mathf.Sqrt((2f * deltaY) / g);

        // Swing acceleration (matches your AddSwing logic)
        float swingAccel = (swingStrength * 0.1f * effectStrength) / Time.fixedDeltaTime;
        Vector3 swingDir = Vector3.left;
        if (effectDirection) swingDir *= -1f;
        if (ballEffect != BallEffect.Swing) swingDir *= 0;

        Vector3 swingAcceleration = swingDir * swingAccel;

        // Total acceleration
        Vector3 totalAcceleration = gravity + swingAcceleration;

        // Required initial velocity
        Vector3 v0 = (targetPosition - startPosition.position - 0.5f * totalAcceleration * t * t) / t;
        v0 += Vector3.down*1.01f;
        // Convert velocity to impulse
        return v0;
    }

    public void Activate()
    {
        Throwable = true;
        gameObject.SetActive(true);
    }

    public void Deactivate()
    {
        Throwable = true;
        gameObject.SetActive(false);
    }

}

public enum BallEffect
{
    None,
    Spin,
    Swing
}
