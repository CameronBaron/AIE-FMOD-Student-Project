﻿using UnityEngine;
using System.Collections;

public class CannonBall : MonoBehaviour {
    Explosion m_explosion;
    ParticleSystem m_particle;
	// Use this for initialization
	void Start () {
        m_explosion = FindObjectOfType<Explosion>();
        m_particle = FindObjectOfType<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    void OnCollisionEnter(Collision a_collision)
    {
        if(a_collision.gameObject.tag == "Ground")
        {
            m_particle.transform.position = transform.position;
            m_explosion.SetPosition(transform.position);
            m_explosion.PlaySound();
            m_particle.Play();
            Destroy(this.gameObject);
        }
    }
}