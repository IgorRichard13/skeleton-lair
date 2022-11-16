using UnityEngine;
using System.Collections;

namespace R2{

    [CreateAssetMenu(fileName = "PlayerProfile", menuName = "R2/PlayerProfile", order = 0)]
    public class PlayerProfile : ScriptableObject {
        public string[] startingClothes;
        public string rightHandWeapon;
        public string leftHandWeapon;
    }
}