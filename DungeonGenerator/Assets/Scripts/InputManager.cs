using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    //public PlayerControls controls;

    PlayerInput playerInput;

    InputAction dashAction;
    InputAction moveAction;
    InputAction lightAttackAction;
    InputAction completeRoomAction;

    public static InputManager Instance { get; set; }
    private void Awake()
    {
        Instance = this;

        //controls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();

        dashAction = playerInput.actions["Dash"];  
        moveAction = playerInput.actions["Move"];  
        lightAttackAction = playerInput.actions["LightAttack"];
        completeRoomAction = playerInput.actions["CompleteRoom"];  
    }

    public bool Dash()
    {
        return dashAction.triggered;
    }

    public bool LightAttack()
    {
        return lightAttackAction.triggered;
    }

    public Vector2 Move()
    {
        return moveAction.ReadValue<Vector2>();
    }

    public bool CompleteRoom()
    {
        return completeRoomAction.triggered;
    }
}
