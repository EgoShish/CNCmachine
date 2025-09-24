using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public static class ControlMachine
{
    private static int _activateSensor = 0;
    private static float _supportSpeed = 1f;
    private static bool _isEmergencyStop = false;
    private static int _move = 0;
    private static float _position = 0;
    
    public static int ActiveSensor => _activateSensor;
    public static float SupportSpeed => _supportSpeed;
    public static bool IsEmergencyStop => _isEmergencyStop;

    public static System.Action OnSupportStoped;
    public static System.Action OnSupportActivate;
    public static void EmergencyStop()
    {
        _supportSpeed = 0f;
        _isEmergencyStop = true;
        Console.Log("АВАРИЙНАЯ ОСТАНОВКА!");
    }

    private static void ResetEmergencyStop()
    {
        _supportSpeed = 1f;
        _isEmergencyStop = false;
    }

    public static void ActivateSensor(int sensorID)
    {
        _activateSensor |= (1 << sensorID);
        OnSupportStoped?.Invoke();
        RecalculateSpeed();
    }

    public static void DeactivateSensor(int sensorID)
    {
        _activateSensor &= ~(1 << sensorID);
        OnSupportActivate?.Invoke();
        RecalculateSpeed();
    }
    private static void RecalculateSpeed()
    {
        if (_isEmergencyStop) return;

        _supportSpeed = _activateSensor == 0 ? 1f : 0f;

        if (_supportSpeed == 0)
        {
            Console.Log($"Суппорт остановлен. Активные датчики: {Convert.ToString(_activateSensor, 2)}");
        }
    }
    public static float MoveCheck(float move)
    {
        int newMove = ((move != 0) && (move > 0)) ? 1 : -1; // определение направления
        if (_supportSpeed > 0) // если нет коллизий
        {
            _move = newMove; // запомнить направление
        }
        else // если есть коллизия
        {
            if (newMove != _move) // если направление изменилось
            {
                _supportSpeed = 1f; // сбросить скорость
                ResetEmergencyStop();
                OnSupportActivate?.Invoke(); // разблокировать суппорт
                return move; // разрешить движение
            }
            else return 0; // запретить движение
        }
        return move;
    }
    public static void PosCheck(float pos)
    {
        if (_activateSensor != 0) // если есть коллизия 
        {
            // если направление изменилось на противоположное
            if (((_position < 0) && (pos > _position)) || ((_position > 0) && (pos < _position)))
            {
                OnSupportActivate?.Invoke(); // разблокировать суппорт
            }
        }
        _position = pos; // запомнить текущую позицию для сравнения
    }
}