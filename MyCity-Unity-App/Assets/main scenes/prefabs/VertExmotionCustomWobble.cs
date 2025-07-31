using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kalagaan;

public class VertExmotionCustomWobble : MonoBehaviour {


	public VertExmotionSensor m_sensor;
	public bool m_startWobble = true;

	//public float m_bouncing = 10f;
	//public float m_damping = 1f;

	public float speed=10;



	BounceSystem_V3 m_bs;

	void Start()
	{
		if (m_sensor == null)
			m_sensor = GetComponent<VertExmotionSensor>();

		m_sensor.m_envelopRadius = 6;




/*	m_sensor.

		//this is an integrated functio
		m_bs = new BounceSystem_V3();

		StartCoroutine(dobe());*/
    }
    /*IEnumerator dobe()
    {

        while (true)
        {

            m_sensor.gameObject.transform.Translate(Vector3.right * speed);

            yield return new WaitForSeconds(0.2f);

            m_sensor.gameObject.transform.Translate(-Vector3.right * speed);

            yield return new WaitForSeconds(0.2f);



        }


        yield break;



    }*/












    void Update()
	{
		m_sensor.m_params.translation.worldOffset = Mathf.Sin(Time.time * 10f) * Vector3.up;


		/*
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast(ray, out hit, 10000) && Input.GetMouseButtonDown(0))
				{





					Debug.Log(hit.collider.gameObject);

				;
					if (hit.collider.gameObject.GetType() == typeof(GameObject))
					{








						//	hit.collider.gameObject.GetComponent<JellyMeshReferencePoint>().transform.position = hit.collider.gameObject.GetComponent<JellyMeshReferencePoint>().ParentJellyMesh.GetComponent<JellyMesh>().CentralPoint.transform.position;

						// e.transform.parent.position = cam.transform.position + cam.transform.forward * distance;
					}


				}*/


		if (m_sensor == null)
			return;


		

		/*		m_bs.bouncing = m_bouncing;
				m_bs.damping = m_damping;


				 //This is an impulse, do it only when needed
					m_startWobble = false;

				//set the impulse vector
				m_bs.m_target = Vector3.up;
				//init the bounce system
				m_bs.Init();
				//set the new target (the bounce will decrease to 0)
				m_bs.m_target = Vector3.zero;


				//set the world offset using the bounce system
				m_sensor.m_params.translation.worldOffset = m_bs.Compute(m_sensor.m_params.translation.worldOffset);
		*/

	}
}
