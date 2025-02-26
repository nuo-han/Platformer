using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInput : MonoBehaviour
{
    [SerializeField] float jumpInputBufferTime = 0.5f;

    InputActionMap input;

    WaitForSeconds waitJumpInputBufferTime;

    public virtual float AxesX { get; set; }
    public virtual bool HasJumpInputBuffer {  get; set; }
    public virtual bool Jump { get; set; }
    public virtual float Climb { get; set; }
    public virtual bool StopJump { get; set; }
    public virtual bool Move => AxesX != 0;
    public virtual bool Dash { get; set; } = false;

    public virtual bool Crouch { get; set; }

    private void Awake()
    {
        waitJumpInputBufferTime = new WaitForSeconds(jumpInputBufferTime);
    }

    private void Start()
    {
        HasJumpInputBuffer = false;
    }

    private void Update()
    {
        //Debug.Log(axes);
    }
    //private void OnGUI()
    //{
    //    Rect rect = new Rect(200, 200, 200, 200);
    //    string message = "Has Jump Input Buffer: " + HasJumpInputBuffer;
    //    GUIStyle style = new GUIStyle();
    //    style.fontSize = 20;
    //    style.fontStyle = FontStyle.Bold;
    //    GUI.Label(rect, message, style);

    //}

    public void SetJumpInputBufferTimer()
    {
        //StopCoroutine(nameof(JumpInputBufferCoroutine));
        StartCoroutine(nameof(JumpInputBufferCoroutine));
    }

    IEnumerator JumpInputBufferCoroutine()
    {
        HasJumpInputBuffer = true;
        yield return waitJumpInputBufferTime;
        HasJumpInputBuffer = false;
    }

}
