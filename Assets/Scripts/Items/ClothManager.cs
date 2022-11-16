using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace R2
{
    public class ClothManager : MonoBehaviour
    {
        Dictionary<ClothItemType,ClothItemHook> clothHooks = new Dictionary<ClothItemType, ClothItemHook>();

        public void Init(){
            ClothItemHook[] clothItemHooks = GetComponentsInChildren<ClothItemHook>();
            foreach(ClothItemHook hook in clothItemHooks){
                hook.Init();
            }
        }

        public void RegisterClothHook(ClothItemHook clothItemHook){
            if(!clothHooks.ContainsKey(clothItemHook.clothItemType)){
                clothHooks.Add(clothItemHook.clothItemType, clothItemHook);
            }else{

            }
        }

        public void LoadListOfItems(List<ClothItem> clothItems){
            UnloadAllItems();

            foreach(ClothItem clothItem in clothItems){
                LoadItem(clothItem);
            }
        }

        public void UnloadAllItems(){
            foreach(ClothItemHook clothItemHook in clothHooks.Values){
                clothItemHook.UnloadItem();
            }
        }

        ClothItemHook GetClothHook(ClothItemType target){
            clothHooks.TryGetValue(target, out ClothItemHook retVal);
            return retVal;
        }

        public void LoadItem(ClothItem clothItem){

            ClothItemHook itemHook = null;

            if(clothItem == null){
                return;
            }else{
                itemHook = GetClothHook(clothItem.clothType);
                itemHook.LoadClothItem(clothItem); 
            }
        }
    }
}
