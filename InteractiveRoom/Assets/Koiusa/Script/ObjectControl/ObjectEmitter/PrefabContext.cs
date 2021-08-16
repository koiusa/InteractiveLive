using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koiusa.InteractiveRoom
{
    public class PrefabContext
    {
        public string key = string.Empty;
        public Vector3 position = Vector3.zero;
        public Quaternion rotate = Quaternion.identity;

        public PrefabContext() { }
        public PrefabContext(string aKey)
        {
            key = aKey;
        }
    }
}
