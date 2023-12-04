public enum GameState
{
    Overworld,
    Map,
    Pause,
    Dialogue
}

public static class GameStateMachine
{
    public delegate void OnStateChangedHandler(GameState next);
    public static OnStateChangedHandler OnStateChanged;
    public static GameState Previous { get; private set; } = GameState.Overworld;
    public static GameState Current { get; private set; } = GameState.Overworld;

    public static void SetState(GameState next) 
    {
        if (Current == next)
            return;

        Previous = Current;
        Current = next;

        OnStateChanged?.Invoke(next);
    }
}