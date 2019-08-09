namespace ABM
{
    namespace Core
    {
        using System;
        using System.Collections;
        using System.Collections.Generic;
        using UnityEngine;
        public interface ISteppable
        {
            // int step { get; set; }
            void Step();
        }
    }
}