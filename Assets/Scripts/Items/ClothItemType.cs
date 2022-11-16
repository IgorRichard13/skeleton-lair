using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    [CreateAssetMenu(menuName = "R2/Items/ClothItemType")]
    public class ClothItemType : ScriptableObject
    {
        public bool isDisabledWhenNoItem;
    }
}
