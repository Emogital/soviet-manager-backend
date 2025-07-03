namespace GameServer.Services.Gameplay
{
    public enum GameMode
    {
        None = 0,

        NewLocal = 1,
        LoadAutoSavedLocal = 2,
        LoadSavedLocal = 3,
        ArbitraryLocal = 4,

        ClassicMatch = 5,

        CoopForTwoMatch = 6,
        TwoByTwoMatch = 7,

        TwoByTwoCampaign = 8,
        DifficultCampaign = 9,
        HardcoreCampaign = 10,
        ImpossibleCampaign = 11,

        CoopHardcoreMatch = 13,
        ConfrontationMatch = 14,

        CreateOrJoinRatingMatch = 15,
    }
}
