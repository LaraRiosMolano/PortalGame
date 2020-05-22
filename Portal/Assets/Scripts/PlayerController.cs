using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    //Camera and rotation
    float m_Yaw;
    float m_Pitch;
    public float m_YawRotationalSpeed = 360.0f;
    public float m_PitchRotationalSpeed = 180.0f;
    public float m_MinPitch = -80.0f;
    public float m_MaxPitch = 50.0f;
    public Transform m_PitchControllerTransform;
    public bool InvertedYaw;
    public bool InvertedPitch;

    //Movement control
    CharacterController m_CharacterController;
    public float m_Speed = 10.0f;
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_ForwardKeyCode = KeyCode.W;
    public KeyCode m_BackwardsKeyCode = KeyCode.S;
    private bool IsMoving = false;

    //Gravity
    float m_VerticalSpeed = 0.0f;
    bool m_OnGround = false;

    //Jump and run
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public float m_FastSpeedMultiplier = 10.0f;
    public float m_JumpSpeed = 10.0f;

    public GameObject Cube;
    public GameObject p1;
    public GameObject p2;
    public Rigidbody quadColor;
    public Transform SpawnPoint;
    public bool m_AttachedObject = false;
    public bool m_AttachingObject = false;
    public Transform m_AttachingPosition;
    public float m_AttachingObjectSpeed = 10f;
    public int m_MouseShootButton1 = 0;
    public int m_MouseShootButton2 = 1;
    public LayerMask m_ShootLayerMask;
    public LayerMask m_HoldLayerMask;
    public Camera cam;
    public Transform m_PlayerCamera;
    public static float Tamao = 1f;
    float time = 0f;
    float timeTel = 1f;
    float timeCube = 0f;
    public static float force = 0f;

    private AudioSource source;
    public AudioClip cubeSpawn;
    public AudioClip portal1;
    public AudioClip portal2;
    public AudioClip dead;
    public AudioClip atach;

    public Transform points;
    public List<Transform> m_ValidPoints;

    private void Awake()
    {
        m_Yaw = transform.rotation.eulerAngles.y;
        m_Pitch = m_PitchControllerTransform.localRotation.eulerAngles.x;

        m_CharacterController = GetComponent<CharacterController>();
    }
    
    void Start()
    {
        source = GetComponent<AudioSource>();
        Cursor.lockState = CursorLockMode.Locked;
        p1.SetActive(false);
        p2.SetActive(false);
        //Tamao = 1f;
    }
    
    void Update()
    {
        timeCube -= Time.deltaTime;
        Ray l_CameraRay = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        RaycastHit l_RaycastHit = new RaycastHit();
        if (Physics.Raycast(l_CameraRay, out l_RaycastHit, 200.0f, m_HoldLayerMask.value))
        {
            Rigidbody raycasted = l_RaycastHit.transform.GetComponent<Rigidbody>();

            if (Input.GetMouseButtonDown(m_MouseShootButton1))
            {
                m_AttachingObject = true;

            }

            if (m_AttachingObject)
            {
                raycasted.isKinematic = true;
                UpdateAttachedObject(raycasted);
            }
        }

        time -= Time.deltaTime;
        if (Input.GetMouseButton(m_MouseShootButton1) && m_AttachingObject == false && m_AttachedObject == false && time <= 0f)
        {
            UpdatePortal(quadColor, p1);
        }

        if (Input.GetMouseButton(m_MouseShootButton2) && m_AttachingObject == false && m_AttachedObject == false && time <= 0f)
        {
            UpdatePortal(quadColor, p2);
        }
        
        ///////////////////////////////////////////////////////////////////////////////77777


        //Camera and rotation
        float l_MouseAxisY = Input.GetAxis("Mouse Y");
        m_Pitch += l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime;
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);
        m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        float l_MouseAxisX = Input.GetAxis("Mouse X");
        m_Yaw += l_MouseAxisX * m_YawRotationalSpeed * Time.deltaTime;
        transform.localRotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);


        //Movement control
        float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
        float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
        Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
        Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));
        Vector3 l_Movement = Vector3.zero;

        if (Input.GetKey(m_ForwardKeyCode))
        {
            l_Movement = l_Forward;
            
        }
        else if (Input.GetKey(m_BackwardsKeyCode))
        {
            l_Movement = -l_Forward;
            
        }

        if (Input.GetKey(m_RightKeyCode))
        {
            l_Movement += l_Right;
            
        }
        else if (Input.GetKey(m_LeftKeyCode))
        {
            l_Movement -= l_Right;
            
        }

        l_Movement.Normalize();
        l_Movement = l_Movement * Time.deltaTime * m_Speed;
        

        //Jump and run
        float l_SpeedMultiplier = 1.0f;
        if (Input.GetKey(m_RunKeyCode))
        {
            l_SpeedMultiplier = m_FastSpeedMultiplier;
        }
        l_Movement *= l_SpeedMultiplier;
        

        if (m_OnGround && Input.GetKeyDown(m_JumpKeyCode))
        {
            m_VerticalSpeed = m_JumpSpeed;

        }


        //Gravity
        m_VerticalSpeed += Physics.gravity.y*3 * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);

        if ((l_CollisionFlags & CollisionFlags.Below) != 0)
        {
            m_OnGround = true;
            m_VerticalSpeed = 0.0f;
            
        }
        else
        {
            m_OnGround = false;
        }

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }

        timeTel -= Time.deltaTime;
        Debug.Log(timeTel);
    }

        //UPDATES

    void UpdatePortal(Rigidbody portal, GameObject portalGame)
    {
        if(portalGame == p1)
        {
            if (Input.GetAxis("Mouse ScrollWheel") > 0f && Tamao < 2f)
            {

                if (Tamao == 0.5f)
                {
                    portalGame.gameObject.transform.localScale *= 4f;
                    points.localScale *= 4f;
                }
                else
                {
                    portalGame.gameObject.transform.localScale *= 2f;
                    points.localScale *= 2f;
                }
                Tamao = 2f;

            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0f && Tamao > 0.5f)
            {

                if (Tamao == 2f)
                {
                    portalGame.gameObject.transform.localScale /= 4f;
                    points.localScale /= 4f;
                }
                else
                {
                    portalGame.gameObject.transform.localScale /= 2f;
                    points.localScale /= 2f;
                }
                Tamao = 0.5f;

            }
        }
        else
        {
            points.localScale = new Vector3 (1,1,1);
        }
        
            Ray l_Ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
            RaycastHit l_RaycastHit;
        
        if (Physics.Raycast(l_Ray, out l_RaycastHit, 200.0f, m_ShootLayerMask.value))
        {

            Vector3 l_EulerAngles = l_RaycastHit.normal;
            Vector3 l_Direction = l_RaycastHit.point - portal.transform.position;
            float l_Distance = l_Direction.magnitude;

            //portal.MovePosition(portal.transform.position + l_Direction);
            portal.MovePosition(l_RaycastHit.point + l_RaycastHit.normal*0.1f);
            //portal.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, l_EulerAngles.y, l_EulerAngles.z), 1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));
            portal.MoveRotation(Quaternion.Lerp(portal.transform.rotation, Quaternion.Euler(0, l_EulerAngles.y, l_EulerAngles.z), 0f));
            portal.transform.forward = l_RaycastHit.normal;
            if (IsValidPosition())
            {
                portalGame.SetActive(true);
                portalGame.transform.position = portal.transform.position;
                portalGame.transform.rotation = portal.transform.rotation;

            }
            else
            {
                portalGame.SetActive(false);
            }
        }
    }

    public bool IsValidPosition()
    {
        Vector3 l_Normal = Vector3.zero;
        bool isValid = true;
        for (int i = 0; i < m_ValidPoints.Count; ++i)
        {
            Transform l_ValidPoint = m_ValidPoints[i];
            Ray l_Ray = new Ray(m_PlayerCamera.position, l_ValidPoint.position - m_PlayerCamera.position);
            RaycastHit l_RaycastHit;
            if (Physics.Raycast(l_Ray, out l_RaycastHit, 200.0f, m_ShootLayerMask.value))
            {
                isValid = true;
            }
            else
            {
                return false;
            }
        }
        return isValid;
    }

    void UpdateAttachedObject(Rigidbody m_ObjectAttached)
    {
        Vector3 l_EulerAngles = m_AttachingPosition.rotation.eulerAngles;
        

        if (!m_AttachedObject)
        {
            Vector3 l_Direction = m_AttachingPosition.transform.position - m_ObjectAttached.transform.position;
            float l_Distance = l_Direction.magnitude;
            float l_Movement = m_AttachingObjectSpeed * Time.deltaTime;
            if (l_Movement >= l_Distance)
            {
                m_AttachedObject = true;
                m_ObjectAttached.MovePosition(m_AttachingPosition.position);
                m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
            }
            else
            {
                l_Direction /= l_Distance;
                m_ObjectAttached.MovePosition(m_ObjectAttached.transform.position + l_Direction * l_Movement);
                m_ObjectAttached.MoveRotation(Quaternion.Lerp(transform.rotation, Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z), 
                    1.0f - Mathf.Min(l_Distance / 1.5f, 1.0f)));
            }
        }
        else
        {
            m_ObjectAttached.MoveRotation(Quaternion.Euler(0.0f, l_EulerAngles.y, l_EulerAngles.z));
            m_ObjectAttached.MovePosition(m_AttachingPosition.position);

            if (Input.GetMouseButtonDown(m_MouseShootButton2))
            {
                force = 0.0f;
                DetachObject(force, m_ObjectAttached);
            }

            if (Input.GetMouseButtonDown(m_MouseShootButton1))
            {
                force = 500.0f;
                DetachObject(force, m_ObjectAttached);
            }
        }

        
    }

    //OTHERS

    void DetachObject(float Force, Rigidbody m_ObjectAttached)
    {
        m_ObjectAttached.GetComponent<Rigidbody>();
        //m_AttachedObject = false;
        source.PlayOneShot(portal2);
        //m_ObjectAttached.GetComponent<Companion>().SetTeleport(true);
        m_AttachingObject = false;
        m_AttachedObject = false;
        m_ObjectAttached.isKinematic = false;
        m_ObjectAttached.AddForce(m_AttachingPosition.forward * Force);
        time = 1f;

    }

    public void Teleport(Portal _Portal)
    {
        if (_Portal != null && timeTel <= 0f)
        {
            Vector3 l_Position = _Portal.transform.InverseTransformPoint(transform.position);
            float m_teleportOffset = 1f;
            transform.position = _Portal.m_MirrorPortal.transform.TransformPoint(new Vector3(-l_Position.x, l_Position.y, -l_Position.z)) + _Portal.m_MirrorPortal.transform.forward * m_teleportOffset;
            Vector3 l_Direction = _Portal.transform.InverseTransformDirection(-transform.forward);
            transform.forward = _Portal.m_MirrorPortal.transform.TransformDirection(l_Direction);
            m_Yaw = transform.rotation.eulerAngles.y;
            timeTel = 1f;
            
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Portal") || other.CompareTag("Portal1"))
        {
            Teleport(other.gameObject.GetComponent<Portal>());
            source.PlayOneShot(portal1);
        }

        if (other.CompareTag("Button") && timeCube <=0f)
        {
            Instantiate(Cube, SpawnPoint.transform.position, SpawnPoint.transform.rotation);
            source.PlayOneShot(dead);
            timeCube = 2f;
        }

        if (other.CompareTag("DeadZone"))
        {

            
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadSceneAsync("GameOver");
        }
    }
}
