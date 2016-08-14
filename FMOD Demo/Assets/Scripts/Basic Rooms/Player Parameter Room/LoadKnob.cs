﻿using UnityEngine;
using System.Collections;

public class LoadKnob : MonoBehaviour {

    public ActorControls m_actor;
    public FMODUnity.StudioEventEmitter m_emitter;

    Material m_material;
    bool m_active;
    float m_loadValue;

    // Use this for initialization
    void Start ()
    {
        m_material = GetComponent<Renderer>().material;
        m_active = false;
        m_loadValue = 0.0f;
    }
	
	// Update is called once per frame
	void Update () {
        RaycastHit info;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out info, 10.0f))
        {
            if (info.collider.name == "Load Knob")
            {
                m_material.SetInt("_OutlineEnabled", 1);
                if (Input.GetMouseButtonDown(0))
                {
                    m_actor.Disabled = true;
                    m_active = true;
                }
            }
            else
            {
                m_material.SetInt("_OutlineEnabled", 0);
            }
        }
        if (Input.GetMouseButton(0) && m_active)
        {
            float mouseX = Input.GetAxis("Mouse X");
            m_loadValue += mouseX / 100.0f;
            m_loadValue = Mathf.Clamp(m_loadValue, -1.0f, 1.0f);
            transform.Rotate(new Vector3(0.0f, -mouseX * 10.0f, 0.0f));
            m_emitter.SetParameter("Load", m_loadValue);
        }
        if (Input.GetMouseButtonUp(0) && m_active)
        {
            m_actor.Disabled = false;
            m_active = false;
        }
    }
}
