using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using UnityEngine;

public class CliffCheckPlayer : MonoBehaviour
{
    public Transform Myobject;
    public LayerMask Player;
    public GameObject p;
    private Rigidbody playerRigidbody;
    Climb climb;
    Rigidbody rb;
    private Vector3 previousPosition;
    private Vector3 currentPosition;
    private Vector3 direction;

    private void Start()
    {
        previousPosition = p.transform.position;
    }
    // Update is called once per frame
    void Update()
    {

        playerRigidbody = p.GetComponent<Rigidbody>();
        climb=p.GetComponent<Climb>();
        rb= p.GetComponent<Rigidbody>();

        Vector3 localPosition = Myobject.InverseTransformPoint(Myobject.position);
        Vector3 localDirection = Myobject.InverseTransformDirection(Myobject.forward);

        currentPosition = p.transform.position;
        direction = currentPosition - previousPosition;
        previousPosition = currentPosition;


        for (int i = -32; i < 32; i++)
        {
            var offset = Myobject.right * i;
            Debug.DrawRay(Myobject.position + offset, Myobject.forward * 15, Color.cyan);
            
            Ray ray = new(Myobject.position+ offset, Myobject.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 15f, Player))
            {
                Vector3 worldUp = Vector3.up;

                float dotProduct = Vector3.Dot(direction, Vector3.up);

                if (dotProduct > 0)
                {
                    Debug.Log("向頂部移動");
                    climb.DectectTopOfTheWall();
                }
                else
                {
                    Debug.Log("向下掉");
                }


            }



        }


        


    }
}

