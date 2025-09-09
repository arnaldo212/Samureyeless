using UnityEngine;

public enum TeamIndex : sbyte
{
    None = -1,
    Neutral = 0,
    Player,
    Enemy,
    Count

}
public class TeamComponent : MonoBehaviour
{
   [SerializeField] private TeamIndex _teamindex = TeamIndex.None;

    public TeamIndex teamindex
    {
        set { _teamindex = value; }

        get { return _teamindex; }
    }
}
