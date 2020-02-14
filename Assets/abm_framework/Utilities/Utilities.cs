/*
This code is licensed under MIT license, Copyright (c) 2019 Kostas Cheliotis
https://github.com/cheliotk/unity_abm_framework/blob/master/LICENSE
*/

namespace ABMU
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// Utilities class, bit sparse at the moment, currently is mainly used as a dumping site for useful/frequently used code that does not 100% fit in other classes.
    /// </summary>
    public class Utilities{

        /// <summary>
        /// Returns a random point in a given AABB
        /// </summary>
        /// <param name="bounds">The axis-aligned bounding box (AABB) to generate a point in</param>
        /// <returns>Vector3 point</returns>
        public static Vector3 RandomPointInBounds(Bounds bounds, System.Random rand = null) {
            if(rand == null){
                return new Vector3(
                    Random.Range(bounds.min.x, bounds.max.x),
                    Random.Range(bounds.min.y, bounds.max.y),
                    Random.Range(bounds.min.z, bounds.max.z)
                );
            }
            else{
                return new Vector3(
                    Map( (float)rand.NextDouble(),0f, 1f,bounds.min.x, bounds.max.x),
                    Map( (float)rand.NextDouble(),0f, 1f,bounds.min.y, bounds.max.y),
                    Map( (float)rand.NextDouble(),0f, 1f,bounds.min.z, bounds.max.z)
                );
            }
        }

        /// <summary>
        /// Maps a value from one range to another
        /// Similar to the map() function in processing:
        /// https://processing.org/reference/map_.html
        /// </summary>
        /// <param name="val">The input value to map</param>
        /// <param name="minin">The input range lower bound</param>
        /// <param name="maxin">The input range upper bound</param>
        /// <param name="minout">The output range lower bound</param>
        /// <param name="maxout">The output range upper bound</param>
        /// <returns>The remapped value in the new range</returns>
        public static float Map(float val, float minin, float maxin, float minout, float maxout){
            return (minout + (maxout - minout) * ((val - minin) / (maxin - minin)));
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

    public static class IListExtensions {
        /// <summary>
        /// Shuffles the element order of the specified list.
        /// </summary>
        public static void Shuffle<T>(this IList<T> ts) {
            var count = ts.Count;
            var last = count - 1;
            for (var i = 0; i < last; ++i) {
                var r = UnityEngine.Random.Range(i, count);
                var tmp = ts[i];
                ts[i] = ts[r];
                ts[r] = tmp;
            }
        }
    }
}