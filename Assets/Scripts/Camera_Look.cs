using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Look : MonoBehaviour
{
    private Vector3 InputAngleAngles { get; set; }

    public float fTurnSpeed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        float rotationY = Input.GetAxis("Mouse Y") * this.fTurnSpeed * Time.deltaTime;

        //I don't fully understand what this is doing but it works
        if (rotationY > 0)
            InputAngleAngles = new Vector3(Mathf.MoveTowards(InputAngleAngles.x, -80.0f, rotationY), transform.localRotation.y, 0);
        else
            InputAngleAngles = new Vector3(Mathf.MoveTowards(InputAngleAngles.x, 80.0f, -rotationY), transform.localRotation.y, 0);

        transform.localEulerAngles = InputAngleAngles;
    }
}
