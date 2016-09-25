using UnityEngine;
using System.Collections;
using NCMB;
using System;
using System.IO;

/// <summary>
/// 外部ファイルからAppKey ClientKeyを注入する//
/// </summary>

[RequireComponent(typeof(NCMBSettings))]
public class NCMBSettingKeyLoadFromExternalFile : MonoBehaviour
{
    public string filePath;

    void Start()
    {
        if(!string.IsNullOrEmpty(filePath))
        {
            TextAsset textAsset = Resources.Load(filePath) as TextAsset;

            if(textAsset != null)
            {
                NCMBKey ncmbKey = JsonUtility.FromJson<NCMBKey>(textAsset.text);
                NCMBSettings.ApplicationKey = ncmbKey.applicationKey;
                NCMBSettings.ClientKey = ncmbKey.clientKey;
            }
        }
    }

}

[Serializable]
class NCMBKey
{
    public string applicationKey;
    public string clientKey;
}