using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    public TextMeshProUGUI rankText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI scoreText;

    private bool isMe = false;

    public void InitializeContent(int rank, string name, float score, bool isPlayer = false)
    {
        isMe = isPlayer;
        
        rankText.text = rank.ToString();
        nameText.text = name.ToString();
        scoreText.text = GameManager.Instance.ConvertFloatTimeToString(score);

        if (isMe)
        {
            rankText.color = Color.yellow;
            nameText.color = Color.yellow;
            scoreText.color = Color.yellow;
        }
    }
}
