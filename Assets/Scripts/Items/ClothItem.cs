using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    
    [CreateAssetMenu(menuName = "R2/Items/ClothItem")]
    public class ClothItem : Item {
        public Material clothMaterial;
        public Mesh mesh;
        public ClothItemType clothType;
    }
}
