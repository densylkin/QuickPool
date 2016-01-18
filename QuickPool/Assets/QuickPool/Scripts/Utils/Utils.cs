using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Linq;

namespace QuickPool
{
    public static class Utils
    {
        public static GameObject CreateRoot(string name)
        {
            var poolsRoot = GameObject.Find("Instances");

            if (poolsRoot == null)
                poolsRoot = new GameObject("Instances");

            var root = new GameObject(name + "_Root");
            root.transform.SetParent(poolsRoot.transform);
            return root;

        }

        public static int DrawBitMaskField(Rect aPosition, int aMask, System.Type aType, GUIContent aLabel)
        {
            var itemNames = System.Enum.GetNames(aType);
            var itemValues = System.Enum.GetValues(aType) as int[];

            int val = aMask;
            int maskVal = 0;
            for (int i = 0; i < itemValues.Length; i++)
            {
                if (itemValues[i] != 0)
                {
                    if ((val & itemValues[i]) == itemValues[i])
                        maskVal |= 1 << i;
                }
                else if (val == 0)
                    maskVal |= 1 << i;
            }
            int newMaskVal = EditorGUI.MaskField(aPosition, aLabel, maskVal, itemNames);
            int changes = maskVal ^ newMaskVal;

            for (int i = 0; i < itemValues.Length; i++)
            {
                if ((changes & (1 << i)) != 0)            
                {
                    if ((newMaskVal & (1 << i)) != 0)     
                    {
                        if (itemValues[i] == 0)           
                        {
                            val = 0;
                            break;
                        }
                        else
                            val |= itemValues[i];
                    }
                    else                                  
                    {
                        val &= ~itemValues[i];
                    }
                }
            }
            return val;
        }
    }

}