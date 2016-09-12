﻿using UnityEngine;
using System.Collections;
using System.IO;

public class Orchestrion : MonoBehaviour
{
    public GameObject m_ball;
    public string m_sheetPath;
    string[] m_sheetMusic;

    public float m_noteLength;
    float m_elapsedNote;

    public float m_keyDistance;

    int m_maxIndex;
    int m_index;

    bool m_isPlaying;

    void Start()
    {
        m_isPlaying = true;
        m_elapsedNote = 0.0f;
        m_sheetMusic = System.IO.File.ReadAllText(m_sheetPath).Split(' ', '\n', '\r');
        for (int i = 0; i < m_sheetMusic.Length; i++)
        {
            Debug.Log(m_sheetMusic[i]);
        }
        m_maxIndex = m_sheetMusic.Length;
        m_noteLength = 0.0f;
    }
    void FixedUpdate()
    {
        if (!m_isPlaying)
            return;
        m_elapsedNote += Time.fixedDeltaTime;
        if (m_elapsedNote >= m_noteLength)
        {
            m_elapsedNote = 0.0f;

            string key = m_sheetMusic[m_index];
            while (key == "")
            {
               m_index = (m_index + 1) % m_maxIndex;
                key = m_sheetMusic[m_index];
            }
            m_noteLength = GetNoteLength(key[key.Length - 1]);
            key = key.Substring(0, m_sheetMusic[m_index].Length - 1);

            float notePosition = 0;

            int note = GetNote(key);
            if (note != -1)
            {
                notePosition += note * m_keyDistance;
                GameObject ball = Instantiate(m_ball);
                ball.transform.parent = transform;
                ball.transform.localPosition = Vector3.zero;
                ball.transform.Translate(0.0f, 0.0f, notePosition);
            }
            m_index = (m_index + 1) % m_maxIndex;
        }
    }

    int GetNote(string a_note)
    {
        switch (a_note)
        {
            case "R":
                return -1;
            case "CN":
                return 0;
            case "CS":
                return 1;
            case "DN":
                return 2;
            case "DS":
                return 3;
            case "EN":
                return 4;
            case "FN":
                return 5;
            case "FS":
                return 6;
            case "GN":
                return 7;
            case "GS":
                return 8;
            case "AN":
                return 9;
            case "AS":
                return 10;
            case "BN":
                return 11;
            default:
                return 0;
        }

    }
    float GetNoteLength(char a_note)
    {
        switch (a_note)
        {
            case 'E':
                return 0.125f;
            case 'Q':
                return 0.25f;
            case 'H':
                return 0.5f;
            case 'T':
                return 0.75f;
            case 'W':
                return 1.0f;
            default:
                return 0.25f;

        }
    }

    public void Play()
    {
        m_isPlaying = true;
    }
    public void Stop()
    {
        m_isPlaying = false;
        m_index = 0;
        m_elapsedNote = 0.0f;
        m_noteLength = 0.0f;

    }
    public void Pause()
    {
        m_isPlaying = false;
    }
}