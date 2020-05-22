using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeController : MonoBehaviour {

    float m_Yaw;
    public GameObject Cube;
    public Transform SpawnPoint;
    public PlayerController pc;
    float timer = 0f;
    //public GameObject portal1;

    private void Awake()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
    }

    void Start () {
        
	}
	
	void Update () {
        timer -= Time.deltaTime;
	}

    private void OnTriggerEnter(Collider other)
    {
        PlayerController pc = GetComponent<PlayerController>();

        if (other.CompareTag("Portal"))
        {
            float a = PlayerController.Tamao;
            transform.localScale /= a;
            Teleport(other.gameObject.GetComponent<Portal>());
        }

        if (other.CompareTag("Portal1"))
        {
            float a = PlayerController.Tamao;
            transform.localScale *= a;
            Teleport(other.gameObject.GetComponent<Portal>());
        }

        if (other.CompareTag("Button"))
        {
            Instantiate(Cube, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
        }
    }


    public void Teleport(Portal _Portal)
    {
        if (_Portal != null && timer <= 0f)
        {
            PlayerController pc = GetComponent<PlayerController>();
            float a = PlayerController.force;

            Vector3 l_Position = _Portal.transform.InverseTransformPoint(transform.position);
            float m_teleportOffset = 0.6f;
            transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(l_Position) + _Portal.m_MirrorPortal.transform.forward * m_teleportOffset;
            Vector3 l_Direction = _Portal.transform.InverseTransformDirection(-transform.forward);
            transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_Direction);

            this.GetComponent<Rigidbody>().AddForce(transform.forward * a);
            m_Yaw = transform.rotation.eulerAngles.y;
            timer = 1f;
        }
    }
}
