using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    public abstract class ItemAction : ScriptableObject { 
        public abstract void ExecuteAction(ItemActionContainer itemContainer, Controller controller);
    }
}
