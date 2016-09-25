using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_InGame : SingletonMonoBehaviour<UI_InGame>
{
    public List<GraveNameText> graveNameTextList = new List<GraveNameText>();

    public Canvas canvas_Game;
    public Canvas canvas_MobileController;
    public Canvas canvas_Pause;
    public Canvas canvas_DeathMessage;
    public Canvas canvas_Win;
    public Canvas canvas_GraveCounter;

    public Text text_UserNameAndDate;
    public Text text_DeathMessage;
    public Text text_LifePoint;
    // private Canvas thisCanvas;

    public Text text_GraveCounter;


    public Image image_LifeBar;
    private float defaultLifeBarWidth;

    public Color lifebar_green;
    public Color lifebar_yellow;
    public Color lifebar_red;

    public Text totalDeathCountNumber;

    private void Awake()
    {
        if (this != Instance)
        {
            Destroy(this);
            return;
        }
        graveNameTextList.ForEach(graveNameText => graveNameText.enabled = false);
    }

    private void Start()
    {
        defaultLifeBarWidth = image_LifeBar.rectTransform.sizeDelta.x;
        //Debug.Log("defaultLifeBarWidth" + defaultLifeBarWidth);
        canvas_Win.enabled = false;

        Resume();

        canvas_GraveCounter.enabled = false;//Closeを呼ぶとゲームスタートが呼ばれてしまうため//
        CloseDeathMessageWindow();

    }

    public void SetLifeBar(int lifePoint)
    {
        text_LifePoint.text = lifePoint.ToString();

        image_LifeBar.rectTransform.localPosition = new Vector3(
            Mathf.InverseLerp(100, 0, lifePoint) * defaultLifeBarWidth,
            image_LifeBar.rectTransform.localPosition.y,
            image_LifeBar.rectTransform.localPosition.z);

        if (lifePoint > 70)
        {
            image_LifeBar.color = lifebar_green;
        }
        else if (lifePoint > 40)
        {
            image_LifeBar.color = lifebar_yellow;

        } else {
            image_LifeBar.color = lifebar_red;
        }
    }

    public void SetDeathCounter(int count)
    {
        totalDeathCountNumber.text = count.ToString();
    }

    public void Pause()
    {
        canvas_Pause.enabled = true;

        graveNameTextList.ForEach(graveNameText => graveNameText.Pause());
    }

    public void ExitGame()
    {
        State_InGame.Instance.ExitGame();
    }

    public void Resume()
    {
        canvas_Pause.enabled = false;

        graveNameTextList.ForEach(graveNameText => graveNameText.Resume());
    }

    public void ShowDeathMessageWindow(string deathMessage, string userNameAndDate)
    {
        text_UserNameAndDate.text = userNameAndDate;
        text_DeathMessage.text = deathMessage;

        canvas_Game.enabled = false;
        canvas_MobileController.enabled = false;
        canvas_DeathMessage.enabled = true;

        graveNameTextList.ForEach(graveNameText => graveNameText.Clear());
    }

    public void CloseDeathMessageWindow()
    {
        canvas_Game.enabled = true;
        canvas_MobileController.enabled = true;        
        canvas_DeathMessage.enabled = false;
    }

    public void ShowGraveCounterWindow(GraveInfo lastGraveInfo)
    {
        string message = lastGraveInfo.deathMessage;
        int count = lastGraveInfo.checkCounter;

        string counterMessage = string.Empty ;

        switch (lastGraveInfo.curseType)
        {
            case GraveInfo.CurseType.None:
                counterMessage = "あなたは墓になにもありませんが、\n"+ count + "人が調べました。\n辞世の句: "+ message;
                break;
            case GraveInfo.CurseType.Damage:
                counterMessage = "あなたが墓に仕掛けた罠に\n" + count + "人が引っかかりました。\n辞世の句: "+ message;
                break;
            case GraveInfo.CurseType.Heal:
                counterMessage = "あなたが墓にかけた回復魔法で\n" + count + "人が救済されました。\n辞世の句: "+ message;
                break;
            default:
                break;
        }

        text_GraveCounter.text = counterMessage;

        canvas_Game.enabled = false;
        canvas_MobileController.enabled = false;
        canvas_GraveCounter.enabled = true;

        graveNameTextList.ForEach(graveNameText => graveNameText.Clear());
    }

    public void CloseGraveCounterWindow()
    {
        canvas_Game.enabled = true;
        canvas_MobileController.enabled = true;
        canvas_GraveCounter.enabled = false;

        State_InGame.Instance.StartGame();
    }

    public void SetGraveName(Transform textFloatPointTransform, Grave grave)
    {
        if (graveNameTextList.Any(graveNameText => graveNameText.CurrentGraveMessageID == grave.graveInfo.objectId) == false)
        {
            GraveNameText graveNameText = graveNameTextList.FirstOrDefault(t => t.enabled == false);

            if (graveNameText != null)
            {
                graveNameText.SetGraveReference(textFloatPointTransform, grave);
            }
        }
    }

    public void ShowWin()
    {
        canvas_Game.enabled = false;
        canvas_MobileController.enabled = false;
        canvas_Win.enabled = true;
    }

}