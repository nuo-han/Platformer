using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBInput : PlayerInput
{
    public override float AxesX => Input.GetAxis("PlayerBHorizontal");
    public override bool Dash => Input.GetKeyDown(KeyCode.LeftShift);
    public override bool Jump => Input.GetKeyDown(KeyCode.W) ? true : false;
    public override float Climb => Input.GetAxisRaw("PlayerBJump");
    public override bool StopJump => Input.GetKeyUp(KeyCode.W) ? true : false;
    public override bool Crouch => Input.GetAxis("PlayerBCrouch") > 0 ? true : false;
}
