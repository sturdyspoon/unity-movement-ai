using UnityEngine;

namespace UnityMovementAI
{
    /// <summary>
    /// This class will create an infinite grid in the x/y directions for a
    /// camera using perspective. Put this class on your camera.
    /// </summary>
    public class InfiniteGrid : MonoBehaviour
    {
        public bool show = true;

        public float cellSize = 1;

        public Vector3 gridLocation;

        Vector3 bottomLeft;
        Vector3 topRight;

        Vector3 upDir;
        Vector3 rightDir;
        Vector3 oneByOneDiagonal;

        Vector2 widthHeight;

        void GetGridBounds()
        {
            float distAway = Camera.main.WorldToViewportPoint(gridLocation).z;

            bottomLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distAway));
            topRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 1, distAway));

            Vector3 bottomRight = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distAway));
            Vector3 topLeft = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, distAway));

            rightDir = bottomRight - bottomLeft;
            rightDir.Normalize();

            upDir = topLeft - bottomLeft;
            upDir.Normalize();

            oneByOneDiagonal = (rightDir + upDir).normalized * Mathf.Sqrt(cellSize * cellSize + cellSize * cellSize);

            /* Convert the camera bounds to the grid bounds */
            ConvertToGridBounds();
        }

        void ConvertToGridBounds()
        {
            Vector3 rightComponent, upComponent;

            rightComponent = ProjectAndExtend(bottomLeft, rightDir, true);
            upComponent = ProjectAndExtend(bottomLeft, upDir, true);
            bottomLeft = rightComponent + upComponent - oneByOneDiagonal;

            rightComponent = ProjectAndExtend(topRight, rightDir, false);
            upComponent = ProjectAndExtend(topRight, upDir, false);
            topRight = rightComponent + upComponent + oneByOneDiagonal;

            Vector3 diagonalDir = topRight - bottomLeft;

            widthHeight.x = Vector3.Project(diagonalDir, rightDir).magnitude;
            widthHeight.y = Vector3.Project(diagonalDir, upDir).magnitude;
        }

        Vector3 ProjectAndExtend(Vector3 vector, Vector3 onNormal, bool shouldFloor)
        {
            Vector3 projection = Vector3.Project(vector, onNormal);

            float newMagnitude = Mathf.Floor(projection.magnitude / cellSize) * cellSize;

            projection = projection.normalized * newMagnitude;

            return projection;
        }

        public Material lineMat;

        void OnPostRender()
        {
            GL.Begin(GL.LINES);

            if (show)
            {
                Material lineMaterial = lineMat;
                lineMaterial.SetPass(0);

                GetGridBounds();

                /* X axis lines */
                for (float j = 0; j <= widthHeight.y; j++)
                {
                    Vector3 p1 = bottomLeft + upDir * j;
                    Vector3 p2 = p1 + rightDir * widthHeight.x;

                    GL.Vertex3(p1.x, p1.y, p1.z);
                    GL.Vertex3(p2.x, p2.y, p2.z);
                }

                /* Y axis lines */
                for (float k = 0; k <= widthHeight.x; k++)
                {

                    Vector3 p1 = bottomLeft + rightDir * k;
                    Vector3 p2 = p1 + upDir * widthHeight.y;

                    GL.Vertex3(p1.x, p1.y, p1.z);
                    GL.Vertex3(p2.x, p2.y, p2.z);
                }
            }

            GL.End();
        }
    }
}