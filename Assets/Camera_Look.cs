using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Look : MonoBehaviour
{
    private Vector3 vRotation = Vector3.zero;

    public float fSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        this.vRotation.y = Input.GetAxis("Mouse X");
        this.vRotation.x = -Input.GetAxis("Mouse Y");

        transform.rotation = Quaternion.LookRotation(this.vRotation * fSpeed * Time.deltaTime);
    }
}
