using UnityEngine;

public class LevelStarted : Unity.Services.Analytics.Event
{
    public LevelStarted() : base("LevelStarted")
    {
    }

    public int Level { set { SetParameter("userLevel", value); } }
}