using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrientateRandom : MonoBehaviour {

	[SerializeField] float m_MaxAngle;
	[SerializeField] float m_QuaternionLerpCoef;

	Quaternion m_StartOrientation;

	Transform m_Transform;

	void Awake()
	{
		m_Transform = GetComponent<Transform>();
	}

	// Use this for initialization
	IEnumerator Start () {
		m_StartOrientation = m_Transform.rotation;
		while (true)
		{
			Quaternion quaternionTarget = Quaternion.AngleAxis(Random.value * m_MaxAngle,m_Transform.TransformDirection(Random.insideUnitCircle.normalized)) *m_StartOrientation;

			yield return StartCoroutine(OrientationCoroutine(quaternionTarget, m_QuaternionLerpCoef));
		}
	}

	IEnumerator OrientationCoroutine(Quaternion quaternionTarget, float quaternionLerpCoef)
	{
		while (Vector3.Angle(m_Transform.rotation * Vector3.forward, quaternionTarget*Vector3.forward) >1f)
		{
			//Debug.Log(Vector3.Angle(m_Transform.rotation * Vector3.forward, quaternionTarget * Vector3.forward));
			m_Transform.rotation = Quaternion.Slerp(m_Transform.rotation, quaternionTarget, quaternionLerpCoef * Time.deltaTime);
			yield return null;
		}
	}
}
