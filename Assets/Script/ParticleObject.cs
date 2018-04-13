using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleObject : MonoBehaviour {

    ParticleSystem particleComponent;

	// Use this for initialization
	void OnEnable () {
        if (particleComponent == null)
        {
            particleComponent = GetComponent<ParticleSystem>();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (particleComponent.isStopped)
        {
            PoolManager.Instance.pushObject("Particle", gameObject, null);
        }
	}
}
