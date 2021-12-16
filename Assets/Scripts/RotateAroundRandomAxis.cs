using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundRandomAxis : MonoBehaviour {

	[SerializeField] float m_RotationDuration;
	[SerializeField] float m_RotationSpeed;

	[SerializeField] float m_QuaternionLerpCoef;

	Vector3 m_RotationAxis;
	Quaternion m_RotQuaternion;

	Transform m_Transform;

	private void Awake()
	{
		m_Transform = GetComponent<Transform>();
	}

	// Use this for initialization
	IEnumerator Start () {
		m_RotQuaternion = Quaternion.identity;

		while(true)
		{
			yield return StartCoroutine(RotationCoroutine(m_RotationDuration, m_RotationSpeed, Random.onUnitSphere));
		}
	}
	
	IEnumerator RotationCoroutine(float rotDuration, float rotSpeed,Vector3 rotAxis)
	{
		float elapsedTime = 0;

		Quaternion m_TargetRotQuaternion = Quaternion.AngleAxis(rotSpeed * Time.deltaTime,rotAxis);

		while(elapsedTime<rotDuration)
		{
			m_RotQuaternion = Quaternion.Slerp(m_RotQuaternion, m_TargetRotQuaternion, Time.deltaTime * m_QuaternionLerpCoef);

			m_Transform.rotation = m_RotQuaternion * m_Transform.rotation;

			elapsedTime += Time.deltaTime;
			yield return null;
		}
	}
}
