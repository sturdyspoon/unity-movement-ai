using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    public class NearSensor : MonoBehaviour
    {
        private HashSet<MovementAIRigidbody> _targets = new HashSet<MovementAIRigidbody>();

        public HashSet<MovementAIRigidbody> targets
        {
            get
            {
                /* Remove any MovementAIRigidbodies that have been destroyed */
                _targets.RemoveWhere(isNull);
                return _targets;
            }
        }

        private static bool isNull(MovementAIRigidbody r)
        {
            return (r == null || r.Equals(null));
        }

        private void tryToAdd(Component other)
        {
            MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
            if (rb != null)
            {
                _targets.Add(rb);
            }
        }

        private void tryToRemove(Component other)
        {
            MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
            if (rb != null)
            {
                _targets.Remove(rb);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            tryToAdd(other);
        }

        void OnTriggerExit(Collider other)
        {
            tryToRemove(other);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            tryToAdd(other);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            tryToRemove(other);
        }
    }
}