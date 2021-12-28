using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IConnectable 
{
    void Pulse();

    void Toggle(bool state);

    Transform GetTransform();

    void Click(DroneController drone = null);
}
