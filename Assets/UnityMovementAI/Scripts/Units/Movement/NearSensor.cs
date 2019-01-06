using UnityEngine;
using System.Collections.Generic;

namespace UnityMovementAI
{
    public class NearSensor : MonoBehaviour
    {
        HashSet<MovementAIRigidbody> _targets = new HashSet<MovementAIRigidbody>();

        public HashSet<MovementAIRigidbody> targets
        {
            get
            {
                /* Remove any MovementAIRigidbodies that have been destroyed */
                _targets.RemoveWhere(IsNull);
                return _targets;
            }
        }

        static bool IsNull(MovementAIRigidbody r)
        {
            return (r == null || r.Equals(null));
        }

        void TryToAdd(Component other)
        {
            MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
            if (rb != null)
            {
                _targets.Add(rb);
            }
        }

        void TryToRemove(Component other)
        {
            MovementAIRigidbody rb = other.GetComponent<MovementAIRigidbody>();
            if (rb != null)
            {
                _targets.Remove(rb);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            TryToAdd(other);
        }

        void OnTriggerExit(Collider other)
        {
            TryToRemove(other);
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            TryToAdd(other);
        }

        void OnTriggerExit2D(Collider2D other)
        {
            TryToRemove(other);
        }
    }
}