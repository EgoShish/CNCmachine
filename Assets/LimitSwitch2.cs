using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LimitSwitch2 : MonoBehaviour
{
    private int sensorID = 2;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Support"))
        {
            ControlMachine.ActivateSensor(sensorID);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Support"))
        {
            ControlMachine.DeactivateSensor(sensorID);
        }
    }
}
