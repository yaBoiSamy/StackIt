using UnityEngine;

[ExecuteInEditMode]
public class BoundVisualizer : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        DrawBounds();
    }

    private void DrawBounds()
    {
        Gizmos.color = Color.yellow;

        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.size);
        }
        else
        {
            Collider collider = GetComponent<Collider>();

            if (collider != null)
            {
                Gizmos.matrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.lossyScale);
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.size);
            }
        }
    }
}
