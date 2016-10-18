﻿/*===============================================================================================
|   Project:		            FMOD Demo                                                       |
|   Developer:	                Matthew Zelenko - http://www.mzelenko.com                       |
|   Company:		            Firelight Technologies                                          |
|   Date:		                20/09/2016                                                      |
|   Scene:                      Player Parameter                                                |
|   Fmod Related Scripting:     Yes                                                             |
|   Description:                Controls the Sounds rpm parameter.                              |
===============================================================================================*/
using UnityEngine;
using System.Collections;

public class Car : MonoBehaviour
{
    public Pedal m_pedal;
    public GearShift m_gearShift;

    /*===============================================Fmod====================================================
    |   This is where the StudioEventEmitter component will be stored                                      |
    =======================================================================================================*/
    public FMODUnity.StudioEventEmitter[] m_sound;

    bool m_isActive;
    float m_acceleration;
    float m_rpm;
    int m_gear;

    void Start()
    {
        m_gear = 1;
        m_isActive = false;
        m_acceleration = 0.0f;
        m_rpm = 0.0f;
        for (int i = 0; i < m_sound.Length; i++)
        {
            m_sound[i].GetComponentInChildren<ParticleSystem>().EnableEmission(false);
        }
    }
    void Update()
    {
        if (m_isActive)
        {
            m_rpm += (m_acceleration - m_rpm) * Time.deltaTime * (1 / ((float)m_gear * 2.0f));
        }
        else
        {
            m_rpm = Mathf.Max(-1.0f, m_rpm - (Time.deltaTime));
            if (m_rpm <= 0.0f)
            {
                for (int i = 0; i < m_sound.Length; i++)
                {
                    m_sound[i].Stop();
                    m_sound[i].GetComponentInChildren<ParticleSystem>().EnableEmission(false);
                }
            }
        }
        /*===============================================Fmod====================================================
        |   The SetParameter function simply sets the event parameter to the value passed in.                   |
        |   The first param is the name of the parameter, the second is the value.                              |
        |   To see what parameters looks like in studio, open the file:                                         |
        |   FMOD\Fmod Demo Sounds\Fmod Demo Sounds.fspro                                                        |
        =======================================================================================================*/
        for (int i = 0; i < m_sound.Length; i++)
        {
            m_sound[i].SetParameter("RPM", (m_rpm + 1.0f) * 2000.0f);
        }
    }

    public void IgnitionOn()
    {
        m_isActive = true;
        for (int i = 0; i < m_sound.Length; i++)
        {
            m_sound[i].Play();
            m_sound[i].GetComponentInChildren<ParticleSystem>().EnableEmission(true);
        }
        m_pedal.StartGlow();
        m_gearShift.StartGlow();
    }
    public void IgnitionOff()
    {
        m_isActive = false;
        m_gear = 1;
        m_acceleration = 0.0f;
        m_gearShift.Reset();
        m_pedal.Reset();
        m_pedal.StopGlow();
        m_gearShift.StopGlow();
    }
    public void UpGear()
    {
        if (m_gear < 5)
        {
            m_gear++;
            if (m_isActive)
            {
                m_rpm -= m_rpm * 0.25f;
                m_rpm = Mathf.Max(m_rpm, 0.0f);
            }
        }
    }
    public void DownGear()
    {
        if (m_gear > 1)
        {
            if (m_isActive)
            {
                m_gear--;
                m_rpm += m_rpm * 0.25f;
                m_rpm = Mathf.Min(5.0f, m_rpm);
            }
        }
    }
    public void Accelerate(float a_acceleration)
    {
        m_acceleration = a_acceleration;
    }
}