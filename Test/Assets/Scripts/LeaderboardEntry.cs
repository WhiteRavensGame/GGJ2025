using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeaderboardEntry : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI rankText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private Color playerColor = Color.yellow;
    private bool isMe = false;

    public void InitializeContent(int rank, string name, float score, bool isPlayer = false)
    {
        isMe = isPlayer;
        
        rankText.text = rank.ToString();
        nameText.text = name.ToString();
        scoreText.text = GameManager.Instance.ConvertFloatTimeToString(score);

        if (isMe)
        {
            rankText.color = playerColor;
            nameText.color = playerColor;
            scoreText.color = playerColor;
        }
    }
}
