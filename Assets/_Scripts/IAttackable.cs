using UnityEngine;
public enum SourceType { Player, Enemy }

public interface IAttackable
{
    SourceType SourceType { get; }
    public void IsAttacked(GameObject source);
}