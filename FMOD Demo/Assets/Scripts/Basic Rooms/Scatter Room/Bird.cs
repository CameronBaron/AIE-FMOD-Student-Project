﻿using UnityEngine;
using System.Collections;

public class Bird : MonoBehaviour
{
    public float m_xOffset, m_zOffset;
    public float m_speed;
    public Vector3 m_lastLocalPosition;
	void Start ()
    {
	
	}
	void Update ()
    {
	
	}
    void LateUpdate()
    {
        m_lastLocalPosition = transform.localPosition;
    }
}