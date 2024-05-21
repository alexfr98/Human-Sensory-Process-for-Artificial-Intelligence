using System;

/// <summary>
/// Represents the drives (motivations or needs) of an Avatar, such as health and hunger.
/// Implements the IAvatarDrives interface.
/// </summary>
public class AvatarDrives : IAvatarDrives
{
    private float health = 100;
    public float Health { get { return health; } }

    private float hunger = 0;
    public float Hunger { get { return hunger; } }

    public void ActualizeDrives(Tuple<bool, float, float> tuple, float temperature, float glare, float sound)
    {
        //First, check if the agent has tried to Eat
        float newHealth = this.health;
        float newHunger = this.hunger;

        if (tuple.Item1 == true)
        {
            newHunger += tuple.Item2;
            newHealth += tuple.Item3;
        }

        // Calculate health lost due to environmental factors
        float temperatureLost = TempCurve(temperature);
        float glareLost = GlareCurve(glare);
        float soundLost = SoundCurve(sound);

        newHealth -= (float)Math.Round(temperatureLost, 1);
        newHealth -= (float)Math.Round(soundLost, 1);
        newHealth -= (float)Math.Round(glareLost, 1);

        // Adjust health and hunger values
        if (this.health == newHealth)
        {
            this.health += 1;
        }
        else
        {
            this.health = newHealth;
        }

        if (this.hunger == newHunger)
        {
            this.hunger += 1;
        }
        else
        {
            this.hunger = newHunger;
        }
    }

    public bool CheckRestart()
    {
        if (health <= 0)
        {
            health = 100;
            hunger = 0;
            return true;
        }
        else if (hunger >= 100)
        {
            hunger = 0;
            health = 100;
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Calculates the health loss due to temperature.
    /// </summary>
    /// <param name="tempValue">The current temperature affecting the Avatar.</param>
    /// <returns>The health loss due to temperature.</returns>
    private float TempCurve(float tempValue)
    {
        float rmin = 45;
        float rmax = 68;

        if (tempValue < rmin)
        {
            return 0;
        }
        else if (tempValue > rmax)
        {
            return 100;
        }
        else
        {
            // y = 3E-38 * tempValue^21.857
            float y = (float)(3E-38 * Math.Pow(tempValue, 21.857f));
            return y;
        }
    }

    /// <summary>
    /// Calculates the health loss due to glare.
    /// </summary>
    /// <param name="glareValue">The current glare level affecting the Avatar.</param>
    /// <returns>The health loss due to glare.</returns>
    private float GlareCurve(float glareValue)
    {
        float damage = 0;
        if (glareValue > 60)
        {
            // damage = 0.3f * glareValue - 17f;
            damage = 0.3f * glareValue - 17f;
        }

        return damage;
    }

    /// <summary>
    /// Calculates the health loss due to sound.
    /// </summary>
    /// <param name="soundValue">The current sound level affecting the Avatar.</param>
    /// <returns>The health loss due to sound.</returns>
    private float SoundCurve(float soundValue)
    {
        float damage;
        if (soundValue < 70)
        {
            damage = 0;
        }
        else if (soundValue >= 153.7)
        {
            damage = 100;
        }
        else
        {
            damage = (float)(1E-08 * Math.Exp(0.15 * soundValue));
        }

        return damage;
    }
}
