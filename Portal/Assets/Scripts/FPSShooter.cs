using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPSShooter : MonoBehaviour {

    public int m_MouseShootButton = 0;
    int m_CurrentAmmoCount = 0;
    float m_CurrentHealthCount;
    
    public int TotalAmmo = 0;
    public int TotalHealth = 0;

    public LayerMask m_ShootLayerMask;
    public GameObject m_ShootHitParticles;
    public Transform m_DestroyObjects;

    public int m_StartAmmo = 10;
    public float m_StartHealth = 10;

    public GameObject m_WeaponDummy;
    public GameObject m_WeaponParticlesEffect;

    public float m_DestroyOnTime = 0.0f;

    public Animation weaponAnimation;

    public AudioClip shotSound;
    public AudioClip reloadSound;
    public AudioClip walkSound;

    private AudioSource source;

    public Text m_AmmoText;

    void Shoot()
    {
        m_CurrentAmmoCount--;
        Ray l_CameraRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(l_CameraRay, out l_RaycastHit, 200.0f, m_ShootLayerMask.value))
            CreateShootHitParticles(l_RaycastHit.point, l_RaycastHit.normal);
        GameObject raycasted = l_RaycastHit.transform.gameObject;

        if (raycasted.CompareTag("Destroyable"))
        {
            StartCoroutine(hitDestroy(raycasted));
        }
        CreateShootWeaponParticles(m_WeaponDummy.transform.position);
    }
    IEnumerator hitDestroy(GameObject go)
    {
        yield return new WaitForSeconds(m_DestroyOnTime);
        GameObject.Destroy(go);

    }

    void CreateShootWeaponParticles(Vector3 position)
    {
        Instantiate(m_ShootHitParticles, position, Quaternion.identity, m_WeaponDummy.transform);
    }

    void CreateShootHitParticles(Vector3 Position, Vector3 Normal)
    {
        GameObject.Instantiate(m_ShootHitParticles, Position, Quaternion.identity, m_DestroyObjects);
        //GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject();
        //if (bullet != null)
        //{//
           // bullet.transform.position = turret.transform.position;
            //bullet.transform.rotation = turret.transform.rotation;
            //bullet.SetActive(true);
        //}

    }

    void Start ()
    {
        
        m_CurrentAmmoCount = m_StartAmmo;
        m_CurrentHealthCount = m_StartHealth;
        weaponAnimation.CrossFade("Idle");
        source = GetComponent<AudioSource>();
	}
	
	void Update ()
    {
        m_AmmoText.text = ": " + m_CurrentAmmoCount;

        if (Input.GetMouseButtonDown(m_MouseShootButton))
        {
            if (CanShoot())
            {
                Shoot();
                source.PlayOneShot(shotSound);
                weaponAnimation.CrossFade("Shoot");
                weaponAnimation.CrossFadeQueued("Idle");
            }
            else
            {
                weaponAnimation.CrossFade("noAmmo");
                weaponAnimation.CrossFadeQueued("Idle");
            }
        }
	}

    /*void OnTriggerEnter(Collider _Collider)
    {
        if (_Collider.tag == "Item" && m_CurrentAmmoCount < m_StartAmmo)
        {
            TotalAmmo = 10;
            Reload();
            source.PlayOneShot(reloadSound);
            weaponAnimation.CrossFade("Reload");
            weaponAnimation.CrossFadeQueued("Idle");
            Item l_Item = _Collider.GetComponent<Item>();
            l_Item.TakeItem();
        }

        if (_Collider.tag == "HealthItem")
        {
            ReloadHealth();
        }
    }*/
    void Reload()
    {

        int tryReload = 10 - m_CurrentAmmoCount;
        int toReload = Mathf.Min(tryReload, TotalAmmo);
        TotalAmmo -= toReload;
        m_CurrentAmmoCount += toReload;

        /*int toReload = TotalAmmo;
        if (TotalAmmo > 10)
        {
            toReload = 10;
        }
        TotalAmmo -= toReload;
        m_CurrentAmmoCount += toReload;*/
    }

    /*void ReloadHealth()
    {
        if (m_CurrentHealthCount < TotalHealth)
        {
            m_CurrentHealthCount++;
        }
    }*/

    bool CanShoot()
    {
        return m_CurrentAmmoCount > 0;
    }


}
