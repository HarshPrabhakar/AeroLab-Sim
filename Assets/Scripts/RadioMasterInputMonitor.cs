
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class RadioMasterInputMonitor : MonoBehaviour
{
    private Vector2 scrollPosition;

    private void OnGUI()
    {
        const float panelWidth = 280f;
        const float panelHeight = 330f;
        const float margin = 10f;

        float panelX = Mathf.Max(
            margin,
            Screen.width - panelWidth - margin
        );

        GUILayout.BeginArea(
            new Rect(panelX, margin, panelWidth, panelHeight),
            GUI.skin.box
        );

        GUILayout.Label("AEROLAB SIM - FLIGHT INPUTS");
        GUILayout.Space(6);

        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        if (Joystick.all.Count == 0)
        {
            GUILayout.Label("No joystick detected.");
        }
        else
        {
            foreach (Joystick joystick in Joystick.all)
            {
                GUILayout.Label("Device: " + joystick.displayName);
                GUILayout.Space(5);

                // Show only the main flight axes.
                ShowAxis(joystick, "x", "Roll / Horizontal");
                ShowAxis(joystick, "y", "Pitch / Vertical");
                ShowAxis(joystick, "z", "Throttle / Auxiliary");
                ShowAxis(joystick, "rz", "Yaw / Auxiliary");

                GUILayout.Space(8);
                GUILayout.Label("Selected Buttons");

                // Display only buttons currently assigned to flight use.
                ShowButton(joystick, 0, "Button 1");
                ShowButton(joystick, 1, "Button 2");
                ShowButton(joystick, 2, "Button 3");
                ShowButton(joystick, 3, "Button 4");
            }
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

    private void ShowAxis(Joystick joystick, string controlName, string label)
    {
        InputControl control = joystick.TryGetChildControl(controlName);

        if (control is AxisControl axis)
        {
            GUILayout.Label(label + ": " + axis.ReadValue().ToString("F2"));
        }
    }

    private void ShowButton(Joystick joystick, int buttonIndex, string label)
    {
        if (buttonIndex < joystick.allControls.Count)
        {
            foreach (InputControl control in joystick.allControls)
            {
                if (control is ButtonControl &&
                    control.name == "button" + buttonIndex)
                {
                    GUILayout.Label(
                        label + ": " +
                        (control.ReadValueAsObject().ToString())
                    );
                    return;
                }
            }
        }
    }
}
