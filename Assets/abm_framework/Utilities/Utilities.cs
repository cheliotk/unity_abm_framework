namespace ABM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    public static class Utilities{
        public static Vector3 RandomPointInBounds(Bounds bounds) {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }

        public delegate void Del();

        public enum StepperQueueOrder
        {
            EARLY,
            NORMAL,
            LATE
        }
    }
}