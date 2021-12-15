using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "playerData", menuName = "ScriptableObjects/PlayerData")]
public class PlayerData : ScriptableObject
{
    public string firstName;
    public string lastName;
    public string school;
    public string grade;
    public string username;

    public void ClearPlayerData()
    {
       firstName = "";
       lastName = "";
       school = "";
       grade = "";
       username = "";
    }
}
