
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class AeroLabDroneController : MonoBehaviour
{
    [Header("Throttle")]
    [Range(0f, 1f)]
    public float throttle = 0.65f;

    public float throttleChangeSpeed = 0.45f;
    public float maxThrust = 24f;

    [Header("Rotation")]
    public float pitchAcceleration = 3f;
    public float rollAcceleration = 3f;
    public float yawAcceleration = 2f;

    [Header("Stability")]
    public float linearDrag = 0.12f;
    public float angularDamping = 0.15f;
    public float stickDeadzone = 0.08f;

    [Header("Read-only telemetry")]
    public float speedMetersPerSecond;
    public float altitudeMeters;

    private Rigidbody rb;

    private float pitchInput;
    private float rollInput;
    private float yawInput;
    private float throttleInput;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode =
            CollisionDetectionMode.ContinuousDynamic;
    }

    void Update()
    {
        ReadGamepad();
        UpdateTelemetry();
    }

    void ReadGamepad()
    {
        Gamepad pad = Gamepad.current;

        if (pad == null)
        {
            pitchInput = 0f;
            rollInput = 0f;
            yawInput = 0f;
            throttleInput = 0f;
            return;
        }

        Vector2 left = pad.leftStick.ReadValue();
        Vector2 right = pad.rightStick.ReadValue();

        // Mode 2-style mapping:
        // Left X = yaw, left Y = incremental throttle.
        // Right X = roll, right Y = pitch.
        yawInput = ApplyDeadzone(left.x);
        throttleInput = ApplyDeadzone(left.y);
        rollInput = ApplyDeadzone(right.x);
        pitchInput = ApplyDeadzone(right.y);

        // Optional emergency throttle cut.
        if (pad.buttonSouth.isPressed)
        {
            throttle = 0f;
        }
    }

    float ApplyDeadzone(float value)
    {
        return Mathf.Abs(value) < stickDeadzone ? 0f : value;
    }

    void FixedUpdate()
    {
        // Integrate throttle so a centered gamepad stick holds
        // the current throttle instead of snapping to zero.
        throttle += throttleInput
                    * throttleChangeSpeed
                    * Time.fixedDeltaTime;

        throttle = Mathf.Clamp01(throttle);

        // Lift acts along the drone's local up direction.
        rb.AddForce(
            transform.up * (throttle * maxThrust),
            ForceMode.Force
        );

        // Basic acro-style rotational control.
        // The values are angular accelerations, not real motor torques.
        Vector3 angularAcceleration = new Vector3(
            -pitchInput * pitchAcceleration,
             yawInput * yawAcceleration,
            -rollInput * rollAcceleration
        );

        rb.AddRelativeTorque(
            angularAcceleration,
            ForceMode.Acceleration
        );

        // Simple air resistance and rotational damping.
        rb.AddForce(
            -rb.linearVelocity * linearDrag,
            ForceMode.Acceleration
        );

        rb.AddTorque(
            -rb.angularVelocity * angularDamping,
            ForceMode.Acceleration
        );
    }

    void UpdateTelemetry()
    {
        if (rb == null) return;

        speedMetersPerSecond = rb.linearVelocity.magnitude;
        altitudeMeters = transform.position.y;
    }

    void OnGUI()
    {
        GUI.Label(new Rect(15, 15, 350, 25),
            $"Throttle: {throttle:P0}");

        GUI.Label(new Rect(15, 40, 350, 25),
            $"Speed: {speedMetersPerSecond:F2} m/s");

        GUI.Label(new Rect(15, 65, 350, 25),
            $"Altitude: {altitudeMeters:F2} m");

        GUI.Label(new Rect(15, 90, 450, 25),
            $"Position: {transform.position:F2}");

        GUI.Label(new Rect(15, 115, 450, 25),
            $"Pitch: {pitchInput:F2}  Roll: {rollInput:F2}  Yaw: {yawInput:F2}");

        GUI.Label(new Rect(15, 140, 450, 25),
            Gamepad.current == null
                ? "NO GAMEPAD DETECTED"
                : $"Gamepad: {Gamepad.current.displayName}");
    }
}
