using UnityEngine;

public class Teste123 : MonoBehaviour
{
    float m_MaxDistance;
    //float m_Speed;
    bool m_HitDetect;

    Collider m_Collider;
    RaycastHit m_Hit;
    public Vector3 axis;

    void Start()
    {
        //Choose the distance the Box can reach to
        m_MaxDistance = 300.0f;
        //m_Speed = 20.0f;
        m_Collider = GetComponent<Collider>();
    }

    void Update()
    {
        //Simple movement in x and z axes
        //float xAxis = Input.GetAxis("Horizontal") * m_Speed;
        //float zAxis = Input.GetAxis("Vertical") * m_Speed;
        //transform.Translate(new Vector3(xAxis, 0, zAxis));
    }

    void FixedUpdate()
    {
        //Test to see if there is a hit using a BoxCast
        //Calculate using the center of the GameObject's Collider, half the GameObject's size, the direction, the GameObject's rotation, and the maximum distance as variables.
        //Also fetch the hit data
        m_HitDetect = Physics.BoxCast(m_Collider.bounds.center, m_Collider.bounds.size, axis, out m_Hit, Quaternion.LookRotation(axis), 1);
        if (m_HitDetect)
        {
            //Output the name of the Collider your Box hit
            //Debug.Log("Hit : " + m_Hit.collider.name);
            if (m_Hit.normal != Vector3.zero)
            {
                //if (results[i].collider.GetComponent<InputComponent>() == null) continue;
                Debug.Log(m_Hit.collider.name);

                //EquipmentManager.instance.Equip(gun.pickupComponent[i].equipment, results[i].collider.gameObject);
                //break;
            }
        }
    }

    //Draw the BoxCast as a gizmo to show where it currently is testing. Click the Gizmos button to see this
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        //Check if there has been a hit yet
        if (m_HitDetect)
        {
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(transform.position, axis * m_Hit.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(m_Collider.bounds.center + axis * m_Hit.distance, m_Collider.bounds.size);

        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else
        {
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(transform.position, axis * m_MaxDistance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(m_Collider.bounds.center + axis * m_MaxDistance, m_Collider.bounds.size);
        }
    }
}
