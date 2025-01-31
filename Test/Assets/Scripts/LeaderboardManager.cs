using UnityEngine;
using TMPro;
using System.Collections.Generic;

// NOTE: Make sure to include the following namespace wherever you want to access Leaderboard Creator methods
using Dan.Main;
using UnityEngine.SocialPlatforms.Impl;

namespace LeaderboardCreatorDemo
{
    public class LeaderboardManager : MonoBehaviour
    {
        //public static LeaderboardManager Instance;

        [SerializeField] private TMP_Text[] _entryTextObjects;
        [SerializeField] private TMP_InputField _usernameInputField;

        [SerializeField] private Transform leaderboardEntryGrid;
        [SerializeField] private GameObject leaderboardEntryPrefab;
        [SerializeField] private TMP_Dropdown dropdownCategory;

        private List<GameObject> leaderboardEntries = new List<GameObject>();
        int totalRankLimit = 1000;

        // Make changes to this section according to how you're storing the player's score:
        // ------------------------------------------------------------
        //[SerializeField] private ExampleGame _exampleGame;

        private float Score => GameManager.Instance.CalculateFinalTotalTime();
        // ------------------------------------------------------------

        private void Start()
        {
            //LoadEntries();
        }


        //private void LoadEntries()
        public void LoadEntries()
        {
            // Q: How do I reference my own leaderboard?
            // A: Leaderboards.<NameOfTheLeaderboard>

            //Delete the leaderboards entries and clear them to load new fresh times.
            foreach(var entry in leaderboardEntries)
            {
                Destroy(entry.gameObject);
            }
            leaderboardEntries.Clear();

            Leaderboards.TestLeaderboard.GetEntries(entries =>
            {
                int i = 0;
                foreach(var entry in entries)
                {
                    //Apply conversion on the score entry (two digits)
                    float exactTime = entries[i].Score / 100.00f;
                    string playerName = GameManager.Instance.playerName;
                    bool isMe = entry.Username == playerName;
                    //bool isMe = false;

                    GameObject g = Instantiate(leaderboardEntryPrefab, leaderboardEntryGrid);
                    LeaderboardEntry leaderboardEntry = g.GetComponent<LeaderboardEntry>();
                    leaderboardEntry.InitializeContent(entries[i].Rank, entries[i].Username, exactTime, isMe);
                    leaderboardEntries.Add(g);
                    i++;

                    //prevent it from spawning millions of ranking at once.
                    if (i > totalRankLimit) break;
                }


                //foreach (var t in _entryTextObjects)
                //    t.text = "";

                //var length = Mathf.Min(_entryTextObjects.Length, entries.Length);
                //for (int i = 0; i < length; i++)
                //    _entryTextObjects[i].text = $"{entries[i].Rank}. {entries[i].Username} - {entries[i].Score}";
            });
        }

        public void LoadLevelEntries(int level)
        {
            //Delete the leaderboards entries and clear them to load new fresh times.
            foreach (var entry in leaderboardEntries)
            {
                Destroy(entry.gameObject);
            }
            leaderboardEntries.Clear();

            LeaderboardReference leaderboard = GetLeaderboardByLevel(level);

            if (leaderboard == null)
            {
                Debug.LogWarning($"WARNING. Leaderboard for {level} doesn't exist.");
                return;
            }

            leaderboard.GetEntries(entries =>
            {
                int i = 0;
                foreach (var entry in entries)
                {
                    //Apply conversion on the score entry (two digits)
                    float exactTime = entries[i].Score / 100.00f;
                    string playerName = GameManager.Instance.playerName;
                    bool isMe = entry.Username == playerName;
                    //bool isMe = false;

                    GameObject g = Instantiate(leaderboardEntryPrefab, leaderboardEntryGrid);
                    LeaderboardEntry leaderboardEntry = g.GetComponent<LeaderboardEntry>();
                    leaderboardEntry.InitializeContent(entries[i].Rank, entries[i].Username, exactTime, isMe);
                    leaderboardEntries.Add(g);
                    i++;

                    //prevent it from spawning millions of ranking at once.
                    if (i > totalRankLimit) break;
                }


 
            });
        }

        public void UploadEntry()
        {
            if (GameManager.Instance.GetDevEnvironment() == DevEnvironment.Development)
                return;

            //Use the player name as the ID on the leaderboard as well.
            string playerName = _usernameInputField.text;
            LeaderboardCreator.SetUserGuid(playerName);

            //allow two digits of milliseconds to be recorded
            float finalScore = Score * 100;

            //Leaderboards.TestLeaderboard.UploadNewEntry(_usernameInputField.text, Score, isSuccessful =>
            Leaderboards.TestLeaderboard.UploadNewEntry(playerName, Mathf.FloorToInt(finalScore), isSuccessful =>
            {
                if (isSuccessful)
                    Debug.Log("Save successful. Score saved: " + Mathf.FloorToInt(finalScore));
                    //LoadEntries();
            });
        }

        //For Level Speedruns
        public void UploadEntryLevel(int level, float time)
        {
            if (GameManager.Instance.GetDevEnvironment() == DevEnvironment.Development)
                return;

            //Use the player name as the ID on the leaderboard as well.
            string playerName = _usernameInputField.text;
            LeaderboardCreator.SetUserGuid(playerName);

            //allow two digits of milliseconds to be recorded
            float finalScore = time * 100;

            Debug.Log($"Saving: {playerName} , {finalScore} , {level}");

            LeaderboardReference leaderboard = GetLeaderboardByLevel(level);

            if(leaderboard == null)
            {
                Debug.LogWarning("Warning, can't save data. No leaderboard found for level " + level);
                return;
            }

            leaderboard.UploadNewEntry(playerName, Mathf.FloorToInt(finalScore), isSuccessful =>
            {
                if (isSuccessful)
                    Debug.Log("Save successful. Score saved: " + Mathf.FloorToInt(finalScore));
                //LoadEntries();
            });

        }

        //called by the dropdown in Leaderboards menu
        public void DisplaySelectedLeaderboard()
        {
            int dropdownChosen = dropdownCategory.value;

            if (dropdownChosen == 0) LoadEntries();
            else LoadLevelEntries(dropdownChosen);

        }

        private LeaderboardReference GetLeaderboardByLevel(int level)
        {
            switch (level)
            {
                case 1: return Leaderboards.Level1;
                case 2: return Leaderboards.Level2;
                case 3: return Leaderboards.Level3;
                case 4: return Leaderboards.Level4;
                case 5: return Leaderboards.Level5;
                case 6: return Leaderboards.Level6;
                case 7: return Leaderboards.Level7;
                case 8: return Leaderboards.Level8;
            }

            return null;
        }
    }
}