using UnityEngine;

public class LimbLineRenderer : MonoBehaviour
{
    [SerializeField] private LineRenderer line;
    [SerializeField] private Transform arm; // Projectile spawn point position
    [SerializeField] private Transform fist; // Projectile
    [SerializeField] private Combat combat;
    [SerializeField] private float tilesPerUnit = 2f;

    private void LateUpdate()
    {
        var fist = combat ? combat.CurrentFist : null;
        if (!fist)
        {
            if (line) line.enabled = false;
            return;
        }

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

        // Built-in vs URP property names
        if (mat.HasProperty("_MainTex"))
        {
            mat.mainTextureScale = new Vector2(tileU, 1f);
        }
        else if (mat.HasProperty("_BaseMap"))
        {
            mat.SetTextureScale("_BaseMap", new Vector2(tileU, 1f));
        }
    }
}
