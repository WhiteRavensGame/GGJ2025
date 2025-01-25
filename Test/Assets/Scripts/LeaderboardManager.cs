using UnityEngine;
using TMPro;
using System.Collections.Generic;

// NOTE: Make sure to include the following namespace wherever you want to access Leaderboard Creator methods
using Dan.Main;

namespace LeaderboardCreatorDemo
{
    public class LeaderboardManager : MonoBehaviour
    {
        //public static LeaderboardManager Instance;

        [SerializeField] private TMP_Text[] _entryTextObjects;
        [SerializeField] private TMP_InputField _usernameInputField;

        [SerializeField] private Transform leaderboardEntryGrid;
        [SerializeField] private GameObject leaderboardEntryPrefab;
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
                    GameObject g = Instantiate(leaderboardEntryPrefab, leaderboardEntryGrid);
                    LeaderboardEntry leaderboardEntry = g.GetComponent<LeaderboardEntry>();
                    leaderboardEntry.InitializeContent(entries[i].Rank, entries[i].Username, entries[i].Score);
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

        public void UploadEntry()
        {
            //TODO: Change leaderboard to support float time
            //Leaderboards.TestLeaderboard.UploadNewEntry(_usernameInputField.text, Score, isSuccessful =>
            Leaderboards.TestLeaderboard.UploadNewEntry(_usernameInputField.text, Mathf.RoundToInt(Score), isSuccessful =>
            {
                if (isSuccessful)
                    Debug.Log("Save successful. Score saved: " + Mathf.RoundToInt(Score));
                    //LoadEntries();
            });
        }
    }
}