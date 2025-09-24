using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Support : MonoBehaviour
{
    
    private float moveSpeed = 3f;
    private Rigidbody rb;
    private float position;
    private bool s_Stop;
    private void Start()
    {
        SupportMode.OnSupportModeChange += ModeChange;
        ControlMachine.OnSupportStoped += SupportStop;
        ControlMachine.OnSupportActivate += SupportActive;
        Client.OnDataFlow += GetPositionFromClient;
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }
    void Update()
    {
        if (FocusManager.Instance.IsInputFieldFocused)
            return;
        if (AppState.supportMode == AppState.SupportMode.Velocity)
        {
            // режим ручного управления
            float moveX = Input.GetAxis("Horizontal");
            moveX = ControlMachine.MoveCheck(moveX);
            if (s_Stop) { return; }
            Vector3 movement = new Vector3(moveX * moveSpeed * Time.deltaTime, 0f, 0f);
            rb.MovePosition(rb.position + movement);
        }
        else
        {
            // контроллер проверяет коллизии и разрешает/запрещает перемещать
            ControlMachine.PosCheck(position);
            if (s_Stop) return;
            Vector3 movement = new Vector3(position, 1.25f, 0f);
            rb.MovePosition(movement);
        }
    }
    void GetPositionFromClient(float pos)
    {
        // нормализация значения из диапозона АЦП (0 - 1023) в координаты сцены (-1,8 - 1,8)
        position = (float)(pos * (3.6 / 1023) - 1.8);
    }
    private void OnDestroy()
    {
        Client.OnDataFlow -= GetPositionFromClient;
        ControlMachine.OnSupportStoped -= SupportStop;
        ControlMachine.OnSupportActivate -= SupportActive;
        SupportMode.OnSupportModeChange -= ModeChange;
    }
    private void SupportStop()
    {
        s_Stop = true;
    }
    private void SupportActive()
    {
        s_Stop = false;
    }
    private void ModeChange()
    {
        if (AppState.supportMode == AppState.SupportMode.Velocity)
        {
            AppState.supportMode = AppState.SupportMode.Position;
            Console.Log($"Режим суппорта переведен в \"Pos-Mode\".");
        }
        else
        {
            AppState.supportMode = AppState.SupportMode.Velocity;
            Console.Log($"Режим суппорта переведен в \"Key-Mode\".");
        }
    }
}
