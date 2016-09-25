using UnityEngine;
using System.Collections;
using System;
using NCMB;

public class NCMBPlayerPrefs : MonoBehaviour
{
    public static int GetInt(string keyName)
    {
        //キーが存在する場合は値を取得します//
        //存在しない場合は defaultValue を返します。//
        int value;

        if (UserAuth.Instance.IsLoggedIn)
        {
            value = (int)NCMBUser.CurrentUser[keyName];
        }
        else
        {
            value = 0;
        }

        return value;
    }

    public static void SetInt(string keyName, int value)
    {
        NCMBUser.CurrentUser[keyName] = value;
    }

    public static void Save()
    {
        NCMBUser.CurrentUser.SaveAsync();
    }

    public static void Save(Action callback)
    {
        NCMBUser.CurrentUser.SaveAsync((NCMBException e) =>
        {
            if (e == null)
            {
                if (callback != null) callback();
            }
        });
    }

    public static bool HasKey(string key)
    {
        return NCMBUser.CurrentUser.ContainsKey(key);
    }

}
