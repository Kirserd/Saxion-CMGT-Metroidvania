public static class GameProgress
{
    public static bool HasHPUp;
    public static bool HasDamageUp;

    public static bool HasWeapon;

    public static bool HasDash;
    public static bool HasWallJump;
    public static bool HasGlide;
    public static bool HasAirJump;

    private static int _motivations = 0;
    public static int Motivations
    {
        get => _motivations;
        set
        {
            _motivations = value;
            OnMotivationAcquired?.Invoke();
        }
    }

    public delegate void OnMotivationAcquiredHandler();
    public static OnMotivationAcquiredHandler OnMotivationAcquired;

    private static int _importantDialogues = 0;
    public static int ImportantDialogues
    {
        get => _importantDialogues;
        set
        {
            _importantDialogues = value;
            OnImportantDialogueAcquired?.Invoke();
        }
    }

    public delegate void OnImportantDialogueAcquiredHandler();
    public static OnImportantDialogueAcquiredHandler OnImportantDialogueAcquired;

    public static void Reset()
    {
        HasHPUp = false;
        HasDamageUp = false;
        HasWeapon = false;
        HasDash = false;
        HasWallJump = false;
        HasGlide = false;
        HasAirJump = false;

        OnMotivationAcquired = null;
        Motivations = 0;

        OnImportantDialogueAcquired = null;
        ImportantDialogues = 0;
    }
}