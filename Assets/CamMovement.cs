using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamMovement : MonoBehaviour
{
    public Transform target; // привязка к игровому объекту через инспектор
    public float distance = 5f; // параметры для вращения камеры
    public float rotationSpeed = 500f;
    public float zoomSpeed = 20f; // параметры для приближения/отдаления камеры
    public float minDist = 1f;
    public float maxDist = 10f;

    private float currentX = 0f; // координаты перемещения камеры
    private float currentY = 30f; 
    void Update()
    {
        if (FocusManager.Instance.IsInputFieldFocused) // проверка на наличие фокуса
        {
            return;
        }
        // при нажатии на правую кнопку мыши меняем координаты
        if (Input.GetMouseButton(1))
        {
            currentX += Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
            currentY -= Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
            currentY = Mathf.Clamp(currentY, -80, 80);
        }
        // код изменения дистанции
        distance -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;
        distance = Mathf.Clamp(distance, minDist, maxDist);
        // логика вращения камерой
        Vector3 direction = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        transform.position = target.position + rotation * direction; // само перемещение

        transform.LookAt(target.position); 
    }
}
