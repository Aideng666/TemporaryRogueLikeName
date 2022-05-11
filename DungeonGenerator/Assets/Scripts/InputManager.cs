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
    InputAction aimAction;
    InputAction lightAttackAction;
    InputAction heavyAttackAction;
    InputAction completeRoomAction;
    InputAction skill1Action;
    InputAction skill2Action;
    InputAction skill3Action;
    InputAction skill4Action;

    public static InputManager Instance { get; set; }
    private void Awake()
    {
        Instance = this;

        //controls = new PlayerControls();
        playerInput = GetComponent<PlayerInput>();

        dashAction = playerInput.actions["Dash"];  
        moveAction = playerInput.actions["Move"];  
        aimAction = playerInput.actions["Aim"];  
        lightAttackAction = playerInput.actions["LightAttack"];
        heavyAttackAction = playerInput.actions["HeavyAttack"];
        completeRoomAction = playerInput.actions["CompleteRoom"];
        skill1Action = playerInput.actions["Skill1"];
        skill2Action = playerInput.actions["Skill2"];
        skill3Action = playerInput.actions["Skill3"];
        skill4Action = playerInput.actions["Skill4"];
    }

    public bool Dash()
    {
        return dashAction.triggered;
    }

    public bool LightAttack()
    {
        return lightAttackAction.triggered;
    }

    public bool HeavyAttack()
    {
        return heavyAttackAction.triggered;
    }

    public Vector2 Move()
    {
        return moveAction.ReadValue<Vector2>();
    }

    public Vector2 Aim()
    {
        return aimAction.ReadValue<Vector2>();
    }

    public bool CompleteRoom()
    {
        return completeRoomAction.triggered;
    }

    public bool Skill1()
    {
        return skill1Action.triggered;
    }

    public bool Skill2()
    {
        return skill2Action.triggered;
    }

    public bool Skill3()
    {
        return skill3Action.triggered;
    }

    public bool Skill4()
    {
        return skill4Action.triggered;
    }
}
