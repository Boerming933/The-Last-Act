using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Hook : MonoBehaviour
{
    [SerializeField] private float lenght;
    [SerializeField] private LayerMask grappleLayer;
    [SerializeField] private LineRenderer rope;
    [SerializeField] private Animator Estian;
    public Circulin circulin;
    public HoverE hoverE;

    private Vector3 grapplePoint;
    private DistanceJoint2D joint;

    [SerializeField] private AudioClip gancho;
    public bool HookDisp = false;
    private bool muerte = false;


    void Start()
    {
        joint = gameObject.GetComponent<DistanceJoint2D>();
        joint.enabled = false;
        rope.enabled = false;
    }

    void Update()
    {
        if (muerte)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.E) && HookDisp && !circulin.atacando)
        {
            circulin.Trapecio(true);
            Estian.SetTrigger("Gancho");
            Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouseWorldPos.z = 0f;
            Vector2 direction = (mouseWorldPos - transform.position).normalized;
            //Genera el movimiento hacia el mouse
            RaycastHit2D hit = Physics2D.Raycast(
            origin: transform.position,
            direction: direction,
            distance: Mathf.Infinity,
            layerMask: grappleLayer
            );

            if (hit.collider != null)
            {
                StartCoroutine(MaxHook());
                ControladorSonidos.instance.ReproducirSonido(gancho,1f);
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

        if (Input.GetKeyUp(KeyCode.E) || !HookDisp)
        {
            StartCoroutine(RecHook());
            Estian.ResetTrigger("Gancho");
            Estian.SetBool("QuietoG", false);
            Estian.SetBool("CaidaG", true);
            joint.enabled = false;
            rope.enabled = false;
            circulin.Trapecio(false);
        }

        if (rope.enabled == true)
        {
            rope.SetPosition(1, transform.position);
        }

        if (!HookDisp)
        {
            Estian.ResetTrigger("Gancho");
        }

    }

    public void Muerte(bool Muerte)
    {
        muerte = Muerte;
    }

    public void CanHook(bool CanHook)
    {
        HookDisp = CanHook; 
    }

    private IEnumerator MaxHook()
    {
        yield return new WaitForSeconds(0.4f);
        HookDisp = false;
    }

    private IEnumerator RecHook()
    {
        yield return new WaitForSeconds(1.5f);
        HookDisp = true;
    }
}
