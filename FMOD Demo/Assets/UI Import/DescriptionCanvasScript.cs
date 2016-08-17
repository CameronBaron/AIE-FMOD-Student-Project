/*=================================================================
Project:		AIE FMOD
Developer:		Cameron Baron
Company:		#COMPANY#
Date:			16/08/2016
==================================================================*/

using UnityEngine;
using System.Collections;


public class DescriptionCanvasScript : MonoBehaviour
{
	public bool m_active = false;
	public float m_timeUntilFade = 2.0f;

	public CanvasGroup m_canvas;

	void Start ()
	{
		m_canvas = GetComponentInChildren<CanvasGroup>();
        m_canvas.alpha = 0;
	}
	

	void Update ()
	{
		if (m_active)
		{
			transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward, Vector3.up);
		}
		if (!m_active && m_canvas.alpha > 0)
		{
			m_canvas.alpha -= Time.deltaTime / 2.0f;
		}
		else if (m_active && m_canvas.alpha < 1)
		{
			m_canvas.alpha += Time.deltaTime / 2.0f;
		}
	}

	public void FadeIn()
    {
        m_active = true;
    }

	public void FadeOut()
	{
		//StartCoroutine(WaitThenFadeOut());
        m_active = false;
    }

	IEnumerator WaitThenFadeOut()
	{
		yield return new WaitForSeconds(m_timeUntilFade);
		m_active = false;
	}
}
