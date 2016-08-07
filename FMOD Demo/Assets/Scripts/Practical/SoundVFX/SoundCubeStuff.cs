﻿/*=================================================================
Project:		AIE FMOD
Developer:		Cameron Baron  
Company:		FMOD
Date:			01/08/2016
==================================================================*/

using UnityEngine;

public class SoundCubeStuff : MonoBehaviour 
{
    // Public Vars
    public int m_gridWidth = 15;
    public MainSound m_soundRef;

    // Private Vars
    [SerializeField]    GameObject m_cubePrefab;        // Prefab to use for visualisation.
    GameObject[][] m_cubes;                             // Array (grid) of prefabs.
    float[] m_soundFFTData;

	void Start () 
	{
        m_cubes = new GameObject[m_gridWidth][];

        for (int i = 0; i < m_gridWidth; i++)
        {
            m_cubes[i] = new GameObject[m_gridWidth];
            for (int j = 0; j < m_gridWidth; j++)
            {
                Vector3 pos = transform.position + new Vector3(20 - (i * 3), 0, 20 - (j * 3));
                m_cubes[i][j] = Instantiate(m_cubePrefab, pos, Quaternion.identity) as GameObject;
                m_cubes[i][j].transform.SetParent(transform);
            }
        }
		
    }
	
	void Update () 
	{
        m_soundFFTData = m_soundRef.m_fftArray;

        for (int row = 0; row < 15; row++)
        {
            for (int col = 0; col < 15; col++)
            {
				int newCol = col;
				if (col < 14)
				{
					 newCol += 1;
				}
                m_cubes[row][col].GetComponent<CubeReshaping>().m_material.SetFloat("_Amount", m_soundFFTData[(row+1) * (newCol)]);
            }
        }
    }

	#region Private Functions

	#endregion
}
