using UnityEngine;

public class LevelClear : Unity.Services.Analytics.Event
{
    public LevelClear() : base("LevelClear")
    {
    }

    public int Level { set { SetParameter("userLevel", value); } }
    public float TimeTaken { set { SetParameter("timeTaken", value); } }
}