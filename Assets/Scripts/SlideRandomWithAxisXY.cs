using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideRandomWithAxisXY : MonoBehaviour {

	[SerializeField] Transform m_minSlide;
	[SerializeField] Transform m_maxSlide;

	[SerializeField] float m_slideDuration;

	private void OnEnable()
    {
		StartCoroutine(StartSlide());
	}

    // Use this for initialization
    IEnumerator StartSlide () {
		while(true)
		{
			yield return StartCoroutine(SlideCoroutine(m_slideDuration));
		}
	}
	
	IEnumerator SlideCoroutine(float slideDuration)
	{
		float elapsedTime = 0;

		Vector3 m_TargetPosition = new Vector3(Random.Range(m_minSlide.position.x, m_maxSlide.position.x), Random.Range(m_minSlide.position.y, m_maxSlide.position.y), 0f);

		while(elapsedTime<slideDuration)
		{
			elapsedTime += Time.deltaTime;
			Vector3 pos = Vector3.Lerp(transform.position, m_TargetPosition, elapsedTime / slideDuration);
			transform.position = pos;

			yield return null;
		}
	}
}
