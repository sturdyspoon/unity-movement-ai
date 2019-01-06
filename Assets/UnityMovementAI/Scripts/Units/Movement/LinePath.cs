using UnityEngine;
using System;

namespace UnityMovementAI
{
    [Serializable]
    public class LinePath
    {
        public Vector3[] nodes;

        [NonSerialized]
        public float maxDist;

        [NonSerialized]
        public float[] distances;

        /// <summary>
        /// Indexer declaration.
        /// </summary>
        public Vector3 this[int i]
        {
            get
            {
                return nodes[i];
            }

            set
            {
                nodes[i] = value;
            }
        }

        public int Length
        {
            get
            {
                return nodes.Length;
            }
        }

        public Vector3 EndNode
        {
            get
            {
                return nodes[nodes.Length - 1];
            }
        }

        /// <summary>
        /// This function creates a path of line segments
        /// </summary>
        /// <param name="nodes">Nodes.</param>
        public LinePath(Vector3[] nodes)
        {
            this.nodes = nodes;

            CalcDistances();
        }

        /// <summary>
        /// Loops through the path's nodes and determines how far each node in
        /// the path is from the starting node.
        /// </summary>
        public void CalcDistances()
        {
            distances = new float[nodes.Length];
            distances[0] = 0;

            for (var i = 0; i < nodes.Length - 1; i++)
            {
                distances[i + 1] = distances[i] + Vector3.Distance(nodes[i], nodes[i + 1]);
            }

            maxDist = distances[distances.Length - 1];
        }

        /// <summary>
        /// Draws the path in the scene view
        /// </summary>
        public void Draw()
        {
            for (int i = 0; i < nodes.Length - 1; i++)
            {
                Debug.DrawLine(nodes[i], nodes[i + 1], Color.cyan, 0.0f, false);
            }
        }

        /// <summary>
        /// Gets the param for the closest point on the path given a position
        /// </summary>
        public float GetParam(Vector3 position, MovementAIRigidbody rb)
        {
            int closestSegment = GetClosestSegment(position);

            float param = this.distances[closestSegment] + GetParamForSegment(position, nodes[closestSegment], nodes[closestSegment + 1], rb);

            return param;
        }

        public int GetClosestSegment(Vector3 position)
        {
            /* Find the first point in the closest line segment to the path */
            float closestDist = DistToSegment(position, nodes[0], nodes[1]);
            int closestSegment = 0;

            for (int i = 1; i < nodes.Length - 1; i++)
            {
                float dist = DistToSegment(position, nodes[i], nodes[i + 1]);

                if (dist <= closestDist)
                {
                    closestDist = dist;
                    closestSegment = i;
                }
            }

            return closestSegment;
        }

        /// <summary>
        /// Given a param it gets the position on the path
        /// </summary>
        public Vector3 GetPosition(float param, bool pathLoop = false)
        {
            /* Make sure the param is not past the beginning or end of the path */
            if (param < 0)
            {
                param = (pathLoop) ? param + maxDist : 0;
            }
            else if (param > maxDist)
            {
                param = (pathLoop) ? param - maxDist : maxDist;
            }

            /* Find the first node that is farther than given param */
            int i = 0;
            for (; i < distances.Length; i++)
            {
                if (distances[i] > param)
                {
                    break;
                }
            }

            /* Convert it to the first node of the line segment that the param is in */
            if (i > distances.Length - 2)
            {
                i = distances.Length - 2;
            }
            else
            {
                i -= 1;
            }

            /* Get how far along the line segment the param is */
            float t = (param - distances[i]) / Vector3.Distance(nodes[i], nodes[i + 1]);

            /* Get the position of the param */
            return Vector3.Lerp(nodes[i], nodes[i + 1], t);
        }

        /// <summary>
        /// Gives the distance of a point to a line segment.
        /// p is the point, v and w are the two points of the line segment
        /// </summary>
        float DistToSegment(Vector3 p, Vector3 v, Vector3 w)
        {
            Vector3 vw = w - v;

            float l2 = Vector3.Dot(vw, vw);

            if (l2 == 0)
            {
                return Vector3.Distance(p, v);
            }

            float t = Vector3.Dot(p - v, vw) / l2;

            if (t < 0)
            {
                return Vector3.Distance(p, v);
            }

            if (t > 1)
            {
                return Vector3.Distance(p, w);
            }

            Vector3 closestPoint = Vector3.Lerp(v, w, t);

            return Vector3.Distance(p, closestPoint);
        }

        /// <summary>
        /// Finds the param for the closest point on the segment vw given the point p
        /// </summary>
        /// <returns>The parameter for segment.</returns>
        float GetParamForSegment(Vector3 p, Vector3 v, Vector3 w, MovementAIRigidbody rb)
        {
            Vector3 vw = w - v;

            vw = rb.ConvertVector(vw);

            float l2 = Vector3.Dot(vw, vw);

            if (l2 == 0)
            {
                return 0;
            }

            float t = Vector3.Dot(p - v, vw) / l2;

            if (t < 0)
            {
                t = 0;
            }
            else if (t > 1)
            {
                t = 1;
            }

            /* Multiple by (v - w).magnitude instead of Sqrt(l2) because we want the magnitude of the full 3D line segment */
            return t * (v - w).magnitude;
        }

        public void RemoveNode(int i)
        {
            Vector3[] newNodes = new Vector3[nodes.Length - 1];

            int newNodesIndex = 0;
            for (int j = 0; j < newNodes.Length; j++)
            {
                if (j != i)
                {
                    newNodes[newNodesIndex] = nodes[j];
                    newNodesIndex++;
                }
            }

            this.nodes = newNodes;

            CalcDistances();
        }

        public void ReversePath()
        {
            Array.Reverse(nodes);

            CalcDistances();
        }
    }
}