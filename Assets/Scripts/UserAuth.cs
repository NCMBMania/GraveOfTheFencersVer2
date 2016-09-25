using NCMB;
using System;
using UnityEngine;
using System.Collections.Generic;

public class UserAuth : SingletonMonoBehaviour<UserAuth>
{
    private bool isLoggedIn = false;
    private string guestPlayerName = string.Empty;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }

        DontDestroyOnLoad(this.gameObject);
    }

    public bool IsLoggedIn
    {
        get { return isLoggedIn; }
    }


    // mobile backendに接続してログイン ------------------------

    public void LogIn(string id, string pw, Action callback)
    {
        NCMBUser.LogInAsync(id, pw, (NCMBException e) =>
        {
            // 接続成功したら
            if (e == null)
            {
                isLoggedIn = true;
                if (callback != null) callback();
            }
            else
            {
                Debug.Log("connection failed prease retry");
            }
        });
    }

    // mobile backendに接続して新規会員登録 ------------------------

    public void SignUp(string id, string mail, string pw, Action callback)
    {
        NCMBUser user = new NCMBUser();
        user.UserName = id;
        user.Email = mail;
        user.Password = pw;
        user.Add("LifePoint", 100);
        user.Add("DeathCount", 0);

        user.SignUpAsync((NCMBException e) =>
        {
            if (e == null)
            {
                isLoggedIn = true;
                if (callback != null) callback();
            }
        });
    }

    public int GetDeathCount()
    {
        if (NCMBUser.CurrentUser != null)
        {
            return Convert.ToInt32(NCMBUser.CurrentUser["DeathCount"]);
        }
        else
        {
            return 0;
        }
    }

    public void CoutupDeathCount()
    {
        if (NCMBUser.CurrentUser != null)
        {
            NCMBUser.CurrentUser.Increment("DeathCount");
            NCMBUser.CurrentUser.SaveAsync();
        }
    }

    public int GetCurrentLifePoint()
    {
        if(NCMBUser.CurrentUser != null)
        {
            return Convert.ToInt32(NCMBUser.CurrentUser["LifePoint"]);
        }
        else
        {
            return 100;
        }
    }

    public void SetCurrentLifePoint(int lifePoint, Action callback = null)
    {
        if (NCMBUser.CurrentUser != null)
        {
            NCMBUser.CurrentUser["LifePoint"] = lifePoint;
            NCMBUser.CurrentUser.SaveAsync((NCMBException e) =>
            {
                if (e == null)
                {
                    if (callback != null) callback();
                }
            });
        }
    }

    public void SetLastGraveObject(NCMBObject graveObject)
    {
        if (NCMBUser.CurrentUser != null)
        {
            NCMBUser.CurrentUser["LastGraveObject"] = graveObject;
            NCMBUser.CurrentUser.SaveAsync();
        }
    }


    public NCMBObject GetLastGraveObject()
    {
        return (NCMBObject)NCMBUser.CurrentUser["LastGraveObject"];
    }

    public bool HasLastGraveObject { get {
            if(NCMBUser.CurrentUser.ContainsKey("LastGraveObject"))
            {
                return NCMBUser.CurrentUser["LastGraveObject"] != null;
            }
            else
            {
                return false;
            }
        }
    }
    // mobile backendに接続してログアウト ------------------------

    public void LogOut()
    {
        NCMBUser.LogOutAsync((NCMBException e) =>
        {
            if (e == null)
            {
                isLoggedIn = false;
            }
        });
    }

    // 現在のプレイヤー名を返す --------------------
    public string CurrentPlayerName()
    {
        if(isLoggedIn)
        {
            return NCMBUser.CurrentUser.UserName;
        }
        else
        {
            return guestPlayerName;
        }
    }

    public string SessionToken()
    {
        return NCMBUser._getCurrentSessionToken();
    }

    //ゲストモードでユーザー名を保存する//
    public void SetGuestName(string name, Action callback)
    {
        guestPlayerName = name;
        if (callback != null) callback();
    }
}