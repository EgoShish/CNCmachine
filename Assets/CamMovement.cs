using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public Transform target; // �������� � �������� ������� ����� ���������
    public float distance = 5f; // ��������� ��� �������� ������
    public float rotationSpeed = 500f;
    public float zoomSpeed = 20f; // ��������� ��� �����������/��������� ������
    public float minDist = 1f;
    public float maxDist = 10f;

    private float currentX = 0f; // ���������� ����������� ������
    private float currentY = 30f; 
    void Update()
    {
        if (FocusManager.Instance.IsInputFieldFocused) // �������� �� ������� ������
        {
            return;
        }
        // ��� ������� �� ������ ������ ���� ������ ����������
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentY = Mathf.Clamp(currentY, -80, 80);
        }
        // ��� ��������� ���������
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDist, maxDist);
        // ������ �������� �������
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = target.position + rotation * direction; // ���� �����������

        transform.LookAt(target.position); 
    }
}
