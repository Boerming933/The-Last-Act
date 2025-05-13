using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Hook : MonoBehaviour
{
    [SerializeField] private float lenght;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer rope;

    private Vector3 grapplePoint;
    private DistanceJoint2D joint;

    void Start()
    {
        joint = gameObject.GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        rope.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            //Genera el movimiento hacia el mouse
            RaycastHit2D hit = Physics2D.Raycast(
            origin: Camera.main.ScreenToWorldPoint(Input.mousePosition),
            direction: Vector2.zero,
            distance: Mathf.Infinity,
            layerMask: grappleLayer
            );

            if (hit.collider != null )
            {
                //genera la "cuerda"
                grapplePoint = hit.point;
                grapplePoint.z = 0;
                joint.connectedAnchor = grapplePoint;
                joint.enabled = true;
                joint.distance = lenght;
                rope.SetPosition(0, grapplePoint);
                rope.SetPosition(1, transform.position);
                rope.enabled = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            joint.enabled = false;
            rope.enabled = false;
        }

        if (rope.enabled == true)
        {
            rope.SetPosition(1, transform.position);
        }
    }   
}
