using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Turret : MonoBehaviour {

    public LineRenderer m_LineRenderer;
    public LayerMask m_CollisionLayerMask;
    public LayerMask m_CollisionLayerMaskObject;
    public float m_MaxDistance = 250.0f;
    public float m_AngleLaserActive = 60.0f;
    public GameObject player;
    public GameObject laser;
    float l_DotAngleLaserActive;
    bool l_RayActive;

    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {

        l_DotAngleLaserActive = Mathf.Cos(m_AngleLaserActive * Mathf.Deg2Rad * 0.5f);
        l_RayActive = Vector3.Dot(transform.up, Vector3.up) > l_DotAngleLaserActive;

        if (l_RayActive)
        {
            laser.SetActive(true);
            Vector3 l_EndRaycastPosition = Vector3.forward * m_MaxDistance;
            RaycastHit l_RaycastHit;
            if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward), out l_RaycastHit, m_MaxDistance, m_CollisionLayerMask.value))
            {
                l_EndRaycastPosition = Vector3.forward * l_RaycastHit.distance;
                /*GameObject raycasted = l_RaycastHit.transform.GetComponent<GameObject>();
                if (raycasted == player)
                {
                    Debug.Log("DEAD");
                }*/
                
                Debug.Log("DEAD");
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadSceneAsync("GameOver");
            }

            if (Physics.Raycast(new Ray(m_LineRenderer.transform.position, m_LineRenderer.transform.forward), out l_RaycastHit, m_MaxDistance, m_CollisionLayerMaskObject.value))
            {
                l_EndRaycastPosition = Vector3.forward * l_RaycastHit.distance;
            }


            m_LineRenderer.SetPosition(1, l_EndRaycastPosition);
        }
        else
        {
            laser.SetActive(false);
        }
    }
}
