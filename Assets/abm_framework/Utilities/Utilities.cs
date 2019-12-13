namespace ABM
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Utilities class, bit sparse at the moment, currently is mainly used as a dumping site for useful/frequently used code that does not 100% fit in other classes.
    /// </summary>
    public class Utilities: MonoBehaviour{

        /// <summary>
        /// Returns a random point in a given AABB
        /// </summary>
        /// <param name="bounds">The axis-aligned bounding box (AABB) to generate a point in</param>
        /// <returns>Vector3 point</returns>
        public static Vector3 RandomPointInBounds(Bounds bounds) {
            return new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );
        }
        
        /// <summary>
        /// no args
        /// </summary>
        public delegate void Del();
        
        /// <summary>
        /// 1 args
        /// </summary>
        /// <param name="arg">first arg</param>
        /// <typeparam name="T">var type</typeparam>
        /// <returns></returns>
        public delegate void Del<in T>(T arg);
        
        /// <summary>
        /// 2 args
        /// </summary>
        /// <param name="arg1">arg 1</param>
        /// <param name="arg2">arg 2</param>
        /// <typeparam name="T1">arg1 Type</typeparam>
        /// <typeparam name="T2">arg2 Type</typeparam>
        /// <returns></returns>
        public delegate void Del<in T1, in T2>(T1 arg1, T2 arg2);
        // public delegate TResult Func<in T1, out TResult>(T1 arg);
    }
}