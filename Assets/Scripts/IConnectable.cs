using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for connectables like PowerNode and PowerConnection.
/// </summary>
/// <seealso cref="PowerNode"/>
/// <seealso cref="PowerConnection"/>
public interface IConnectable 
{
    /// <summary>
    /// Pulses the object.
    /// </summary>
    void Pulse();

    /// <summary>
    /// Toggles the object.
    /// </summary>
    /// <param name="state">The new state</param>
    void Toggle(bool state);

    /// <summary>
    /// Gets the GameObject's transform.
    /// </summary>
    /// <returns>The transform.</returns>
    Transform GetTransform();

    /// <summary>
    /// Handles clicking.
    /// </summary>
    /// <param name="drone">Reference to the player drone</param>
    void Click(DroneController drone = null);
}
