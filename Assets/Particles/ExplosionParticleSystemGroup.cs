using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticleSystemGroup : MonoBehaviour {

    public void StartExplosionAndCleanupForTeam(Team team)
    {
        float longestDuration = 0;

        // Cycle through particle system children
        foreach (Transform child in transform)
        {
            var system               = child.GetComponent<ParticleSystem>();
            var customizableParticle = child.GetComponent<TeamCustomizableParticle>();

            if (customizableParticle != null)
            {
                customizableParticle.CustomizeForTeam(team);
            }

            system.Play();

            if (longestDuration < system.main.duration) {
                longestDuration = system.main.duration;
            }
        }

        Destroy(gameObject, longestDuration);
    }
}
