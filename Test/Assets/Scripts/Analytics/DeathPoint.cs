using UnityEngine;

public class DeathPoint : Unity.Services.Analytics.Event
{
    public DeathPoint() : base("DeathPoint")
    {
    }

    public int Level { set { SetParameter("userLevel", value); } }
    public float DeathPointX { set { SetParameter("deathPointX", value); } }
    public float DeathPointY { set { SetParameter("deathPointY", value); } }
}