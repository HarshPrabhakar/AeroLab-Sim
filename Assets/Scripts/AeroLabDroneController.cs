
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class AeroLabDroneController : MonoBehaviour
{
    [Header("Transmitter")]
    public bool useRadioMaster = true;
    public float stickDeadzone = 0.08f;

    [Header("Throttle")]
    [Range(0f, 1f)]
    public float throttle = 0.44f;
    public float maxThrust = 18f;

    [Header("Rotation")]
    public float pitchAcceleration = 3f;
    public float rollAcceleration = 3f;
    public float yawAcceleration = 2f;

    [Header("Stability")]
    public float linearDrag = 0.12f;
    public float angularDamping = 0.5f;

    [Header("Live Input - Read Only")]
    public float throttleStick;
    public float pitchInput;
    public float rollInput;
    public float yawInput;
    public string controllerStatus = "Waiting for TX16S";

    private Rigidbody rb;
    private Joystick radio;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.linearDamping = linearDrag;
        rb.angularDamping = angularDamping;
    }

    void Update()
    {
        radio = FindRadioMaster();

        if (!useRadioMaster || radio == null)
        {
            controllerStatus = "TX16S not detected";
            return;
        }

        controllerStatus = "Connected: " + radio.displayName;

        // Your measured TX16S axis mapping:
        // Throttle stick up = y -1; down = y +1
        // Throttle stick left/right = rx -1/+1
        // Other stick up/down = z +1/-1
        // Other stick left/right = x -1/+1

        float rawThrottleY = ReadAxis("y", 0f);
        float rawYaw       = ReadAxis("rx", 0f);
        float rawPitch     = ReadAxis("z", 0f);
        float rawRoll      = ReadAxis("x", 0f);

        // Convert throttle stick position to 0..1.
        // Stick down = 0 throttle; stick up = 1 throttle.
        throttleStick = Mathf.Clamp01((1f - rawThrottleY) * 0.5f);

        yawInput = ApplyDeadzone(rawYaw);
        pitchInput = ApplyDeadzone(rawPitch);
        rollInput = ApplyDeadzone(rawRoll);

        // Use the transmitter's throttle position directly.
        throttle = throttleStick;
    }
    float ReadAxis(string axisName, float fallback)
{
    if (radio == null)
        return fallback;

    foreach (var control in radio.allControls)
    {
        if (control.name == axisName &&
            control is UnityEngine.InputSystem.Controls.AxisControl axis)
        {
            return axis.ReadValue();
        }
    }

    return fallback;
}

    void FixedUpdate()
    {
        if (!useRadioMaster || radio == null)
            return;

        // Thrust along the drone's local up direction.
        rb.AddForce(
            transform.up * throttle * maxThrust,
            ForceMode.Force
        );

        // Apply rotational control using the mapped sticks.
        rb.AddRelativeTorque(
            pitchInput * pitchAcceleration,
            yawInput * yawAcceleration,
            -rollInput * rollAcceleration,
            ForceMode.Force
        );
    }

    Joystick FindRadioMaster()
    {
        foreach (Joystick device in Joystick.all)
        {
            if (device.displayName.ToLower().Contains("radiomaster"))
                return device;
        }

        // Fallback if Windows gives the joystick a different name.
        if (Joystick.all.Count > 0)
            return Joystick.all[0];

        return null;
    }

    float ApplyDeadzone(float value)
    {
        return Mathf.Abs(value) < stickDeadzone ? 0f : value;
    }


}
