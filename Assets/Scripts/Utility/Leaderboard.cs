using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    List<PlayerDataSave> scoresSave = new List<PlayerDataSave>();
    private string remoteDirectory = "C:/Users/BVS/My Drive/leaderboards";
    private string remoteDirectory2 = "C:/Users/spele/My Drive/leaderboards";//"C:/Users/Jazeps Private/Documents";//
    string path;
    public static Leaderboard instance;
    public VerticalLayoutGroup[] layouts;
    public TMP_Text textPrefab;
    int tempInt = 0;
    private int scoresToShow = 15;
    private string lastNameAdded = "";
    private int lastScoreAdded = -1;
    bool boldedResult = false;
    // add some values to the collection here
    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.H))
        {
            AddEntry(scoresSave[tempInt%scoresSave.Count], tempInt);
            tempInt++;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (Directory.Exists(remoteDirectory))
        {
            path = remoteDirectory;
            Debug.LogError("found remote dir!");
        }
        else if(Directory.Exists(remoteDirectory2))
        {
            path = remoteDirectory2;
        }
        else
        {
            path = Application.persistentDataPath; 
        }
        string diff = Var.isEasy ? "_easy" :"_hard";
        path = path + "/" + TopBarController.instance.gameNameString+diff + ".txt";
       // Debug.LogError("path: " + path);
    }

    public void AddScoreToLeaderboard(string nickname, int score)
    {
        scoresSave.Add(new PlayerDataSave(nickname, score, System.DateTime.Now));
        lastNameAdded = nickname;
        lastScoreAdded = score;
       // LogLeaderboard();
        Save();
    }

    public void SetupLeaderboardVisualisation()
    {
        Sort();
        boldedResult = false;
        for (int i =0;i< scoresToShow; i++)
        {
            if(i< scoresSave.Count)
                AddEntry(scoresSave[i], i);
        }
        if(!boldedResult)
        {
            AddToLayout("-", layouts[0]);
            AddToLayout("-", layouts[1]);
            AddToLayout("-", layouts[2]);
            AddToLayout("-", layouts[3]);
            for (int i = 0; i < scoresSave.Count; i++)
            {
                if (ShouldBoldResult(scoresSave[i]))
                {
                    AddEntry(scoresSave[i], i);
                    break;
                }
            }

        }
    }
    private bool ShouldBoldResult(PlayerDataSave playerData)
    {
      return  playerData.playerName == lastNameAdded && playerData.score == lastScoreAdded && !boldedResult;
    }

    private void AddEntry(PlayerDataSave playerData, int ID)
    {
        bool shouldBeBold = ShouldBoldResult(playerData);
        if (shouldBeBold)
        {
            AddToLayout("<b>"+(ID + 1).ToString() + ".</b>", layouts[0]);
            AddToLayout("<b>" + playerData.score.ToString() + "</b>", layouts[1]);
            AddToLayout("<b>" + playerData.playerName + "</b>", layouts[2]);
            AddToLayout("<b>" + playerData.date.ToShortDateString() + "</b>", layouts[3]); 
            boldedResult = true;
        }
        else
        {
            AddToLayout((ID + 1).ToString() + ".", layouts[0]);
            AddToLayout(playerData.score.ToString(), layouts[1]);
            AddToLayout(playerData.playerName, layouts[2]);
            AddToLayout(playerData.date.ToShortDateString(), layouts[3]);
        }
    }

    private void AddToLayout(string text, VerticalLayoutGroup layout)
    {
        TMP_Text obj = Instantiate(textPrefab, layout.transform);
        obj.text = text;
        
    }

    public void Sort()
    {
        scoresSave = scoresSave.OrderBy(o => o.score).ToList();
        scoresSave.Reverse();
    }


    public bool Load()
    {
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(path, FileMode.Open);
            scoresSave = (List<PlayerDataSave>)bf.Deserialize(file);
            file.Close();
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Save()
    {

        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(path, FileMode.OpenOrCreate);
        bf.Serialize(file, scoresSave);
        file.Close();
    }

    public void LogLeaderboard()
    {
        for (int i = 0; i < scoresSave.Count; i++)
        {
            Debug.Log(scoresSave[i].playerName+ ": "+ scoresSave[i].score + " date: " + scoresSave[i].date.ToShortDateString());
        }
    }
}
[System.Serializable]
public class PlayerDataSave
{
    public string playerName;
    public int score;
    public System.DateTime date;

    public PlayerDataSave(string playerName, int score, System.DateTime date)
    {
        this.playerName = playerName;
        this.score = score;
        this.date = date;
    }
}
