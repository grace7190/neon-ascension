using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionParticleSystemGroup : MonoBehaviour {

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartExplosionAndCleanup() {

        float longestDuration = 0;

        // Cycle through particle system children
        foreach (Transform child in transform)
        {
            var system = child.GetComponent<ParticleSystem>();
            system.Play();

            if (longestDuration < system.main.duration) {
                longestDuration = system.main.duration;
            }
        }

        Destroy(gameObject, longestDuration);
    }
}
