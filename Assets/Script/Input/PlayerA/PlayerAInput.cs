using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAInput : PlayerInput
{
    public override float AxesX => Input.GetAxis("PlayerAHorizontal");
    public override bool Jump => Input.GetKeyDown(KeyCode.UpArrow) ? true : false;
    public override float Climb => Input.GetAxisRaw("PlayerAJump");
    public override bool StopJump => Input.GetKeyUp(KeyCode.UpArrow) ? true : false;
    public override bool Crouch => Input.GetAxis("PlayerACrouch") > 0 ? true : false;
}
