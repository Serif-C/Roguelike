using UnityEngine;

public class LimbLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform arm; // Projectile spawn point position
    [SerializeField] private Transform fist; // Projectile
    [SerializeField] private float tilesPerUnit = 2f;

    private void Start()
    {
        arm.position = transform.position;
    }

    private void LateUpdate()
    {
        Vector3 a = arm.position;
        Vector3 b = fist.position;
        line.enabled = true;
        line.positionCount = 2;
        line.SetPosition(0, a);
        line.SetPosition(1, b);

        float length = Vector3.Distance(a, b);
        float tileU = Mathf.Max(0.01f, length * tilesPerUnit);

        var mat = line.material;
        if (!mat) return;
    }
}
