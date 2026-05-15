using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoaderAssetBundle : MonoBehaviour
{
    public static LoaderAssetBundle instance;
    [SerializeField] internal AssetBundle bundle = null;
}
