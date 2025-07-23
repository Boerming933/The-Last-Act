using UnityEngine;

public class FixedWorldTransform : MonoBehaviour
{
    private Vector3 fixedPosition;
    private Quaternion fixedRotation;
    private Vector3 fixedScale;

    void Start()
    {
        fixedPosition = transform.position;
        fixedRotation = transform.rotation;
        fixedScale = transform.lossyScale;
    }

    void LateUpdate()
    {
        // Restaurar posición y rotación mundial
        transform.position = fixedPosition;
        transform.rotation = fixedRotation;

        // Calcular escala local necesaria para mantener la escala mundial
        if (transform.parent != null)
        {
            Vector3 parentLossyScale = transform.parent.lossyScale;
            Vector3 newLocalScale = new Vector3(
                fixedScale.x / parentLossyScale.x,
                fixedScale.y / parentLossyScale.y,
                fixedScale.z / parentLossyScale.z
            );
            transform.localScale = newLocalScale;
        }
        else
        {
            transform.localScale = fixedScale;
        }
    }
}
