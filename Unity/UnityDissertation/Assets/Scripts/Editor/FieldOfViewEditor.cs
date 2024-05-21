using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AvatarSenses))]
public class FieldOfViewEditor : Editor
{
    /// <summary>
    /// Called when the scene view is being drawn. This method is used to draw custom gizmos in the scene view.
    /// </summary>
    private void OnSceneGUI()
    {
        // Get the AvatarSenses component from the target
        AvatarSenses fov = (AvatarSenses)target;

        // Calculate the view angles
        Vector3 viewAngle01 = DirectionFromAngle(fov.transform.GetChild(0).transform.eulerAngles.y, -fov.angle / 2);
        Vector3 viewAngle02 = DirectionFromAngle(fov.transform.GetChild(0).transform.eulerAngles.y, fov.angle / 2);

        // Calculate the dimensions for the overlap box using the Pythagorean theorem
        float sizeZ = Mathf.Abs(fov.raycastPosition.z - (fov.raycastPosition + viewAngle02 * fov.radius).z);
        float sizeX = Mathf.Sqrt(Mathf.Pow(fov.radius, 2) - Mathf.Pow(sizeZ, 2));

        // Draw the vision radius
        Handles.color = Color.red;
        Handles.DrawWireDisc(fov.raycastPosition, new Vector3(0, 1, 0), fov.radius);

        // Draw the field of view lines
        Handles.color = Color.blue;
        Handles.DrawLine(fov.raycastPosition, fov.raycastPosition + viewAngle01 * fov.radius);
        Handles.DrawLine(fov.raycastPosition, fov.raycastPosition + viewAngle02 * fov.radius);

        // Draw lines to objects in the sight list
        foreach (BaseObject go in fov.SightList)
        {
            if (go != null)
            {
                Handles.color = Color.green;
                Handles.DrawLine(fov.raycastPosition, go.gameObject.transform.position);
            }
        }

        // Draw lines to objects that are not viewed
        foreach (BaseObject go in fov.GetListNotViewed())
        {
            Handles.color = Color.red;
            Handles.DrawLine(fov.raycastPosition, go.gameObject.transform.position);
        }
    }

    /// <summary>
    /// Calculates a direction vector from an angle.
    /// </summary>
    /// <param name="eulerY">The Y rotation in Euler angles.</param>
    /// <param name="angleInDegrees">The angle in degrees.</param>
    /// <returns>The direction vector.</returns>
    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
