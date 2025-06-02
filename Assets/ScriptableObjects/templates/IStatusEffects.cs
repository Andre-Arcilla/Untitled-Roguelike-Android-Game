public interface IStatusEffect
{
    string Name { get; }
    int Duration { get; set; }
    bool IsShortTerm { get; }
    bool AllowsStacking { get; }
    bool Override { get; }
    bool IsDebuff { get; }
    void OnApply(CharacterInfo target);
    void OnTurnStart();
    void OnRemove();
    public bool IsExpired => Duration <= 0;
}