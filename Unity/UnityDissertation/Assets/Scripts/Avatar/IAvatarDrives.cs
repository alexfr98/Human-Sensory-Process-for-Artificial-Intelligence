using System;

/// <summary>
/// Interface for defining the drives (motivations or needs) of an Avatar, such as health and hunger.
/// </summary>
public interface IAvatarDrives
{
    /// <summary>
    /// Updates the Avatar's drives based on the given inputs.
    /// </summary>
    /// <param name="tuple">A tuple containing a boolean and two float values, used to represent specific state changes.</param>
    /// <param name="temperature">The current temperature affecting the Avatar.</param>
    /// <param name="glare">The current glare level affecting the Avatar.</param>
    /// <param name="sound">The current sound level affecting the Avatar.</param>
    void ActualizeDrives(Tuple<bool, float, float> tuple, float temperature, float glare, float sound);

    /// <summary>
    /// Checks whether the Avatar needs to restart, based on its current drives.
    /// </summary>
    /// <returns>True if the Avatar needs to restart; otherwise, false.</returns>
    bool CheckRestart();

    /// <summary>
    /// Gets the current health level of the Avatar.
    /// </summary>
    public float Health { get; }

    /// <summary>
    /// Gets the current hunger level of the Avatar.
    /// </summary>
    public float Hunger { get; }
}
