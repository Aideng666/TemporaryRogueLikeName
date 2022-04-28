using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public PlayerControls controls;

    public static InputManager Instance { get; set; }
    private void Awake()
    {
        Instance = this;

        controls = new PlayerControls();
    }

    private void OnEnable()
    {
        controls.Enable();
    }

    private void OnDisable()
    {
        controls.Disable();
    }

    public bool Dash()
    {
        return controls.Player.Dash.triggered;
    }

    public bool LightAttack()
    {
        return controls.Player.LightAttack.triggered;
    }

    public Vector2 Move()
    {
        return controls.Player.Move.ReadValue<Vector2>();
    }
}
