using UnityEngine;
using System.Collections;

public class Teste1234 : MonoBehaviour
{
    public GameObject prefab;
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            var obj = Instantiate(prefab, transform.position, Quaternion.Euler(0, transform.eulerAngles.y - 90, -transform.Find("FirstPersonCamera").transform.localEulerAngles.x));
            print(transform.eulerAngles);
        }
    }
}
