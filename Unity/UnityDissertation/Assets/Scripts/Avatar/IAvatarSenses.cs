using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Interface for defining the senses of an Avatar, including sight, touch, and hearing.
/// </summary>
public interface IAvatarSenses
{
    /// <summary>
    /// Gets or sets the list of objects currently in the Avatar's sight.
    /// </summary>
    public List<IWorldObject> SightList { get; set; }

    /// <summary>
    /// Gets or sets the list of temperature emitters currently being touched by the Avatar.
    /// </summary>
    public List<ITemperatureEmitter> TouchList { get; set; }

    /// <summary>
    /// Gets or sets the list of sound emitters currently being heard by the Avatar.
    /// </summary>
    public List<ISoundEmitter> HearingList { get; set; }

    /// <summary>
    /// Updates the senses list by adding a new collider.
    /// </summary>
    /// <param name="other">The collider to add, related with the object.</param>
    void ActualizeSensesListAdding(Collider other);

    /// <summary>
    /// Updates the senses list by removing a collider.
    /// </summary>
    /// <param name="other">The collider to remove, related with the object.</param>
    void ActualizeSensesListRemoving(Collider other);

    /// <summary>
    /// Uses the sight sense to detect objects in front of the Avatar.
    /// </summary>
    /// <param name="avatarTransform">The transform of the Avatar.</param>
    void UseSightSense(Transform avatarTransform);

    /// <summary>
    /// Calculates the temperature felt by the Avatar based on nearby temperature emitters.
    /// </summary>
    /// <param name="ambientTemp">The ambient temperature.</param>
    /// <param name="currentPosition">The current position of the Avatar.</param>
    /// <returns>The temperature felt by the avatar.</returns>
    float CalculateTemperature(float ambientTemp, Vector3 currentPosition);

    /// <summary>
    /// Calculates the sound intensity felt by the Avatar based on nearby sound emitters.
    /// </summary>
    /// <param name="currentPosition">The current position of the Avatar.</param>
    /// <returns>The sound intensity .</returns>
    float CalculateSound(Vector3 currentPosition);

    /// <summary>
    /// Gets the amount of glare currently felt by the Avatar.
    /// </summary>
    public float Glare { get; }
}
