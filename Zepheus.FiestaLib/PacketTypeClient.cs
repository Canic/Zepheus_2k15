
namespace Zepheus.FiestaLib
{
    public enum CH2Type : byte
    {
        Pong = 5,
        BackToSeverSelect = 13,
    }

    public enum CH3Type : byte
    {
        Version = 101,
        Login = 56,
        WorldReRequest = 15,
        FileHash = 4,
        WorldSelect = 11,
        BackToWorldSelect = 51,
        GameLogout = 24,

        //Actually used in World
        WorldClientKey = 15,
    }

    public enum CH4Type : byte
    {
        CharSelect = 1,

        ReviveToTown = 78,
        SetPointOnStat = 92,
    }

    public enum CH5Type : byte
    {
        CreateCharacter = 1,
        DeleteCharacter = 7,
    }

    public enum CH6Type : byte
    {
        TransferKey = 1,
        ClientReady = 3,
    }

    public enum CH7Type : byte
    {
        UnknownSomethingWithMobs = 1,
    }

    public enum CH8Type : byte
    {
        ChatNormal = 1,
        BeginInteraction = 10,
        LoginGMAuthentication = 11,
        Whisper = 12,
        Stop = 18,
        PartyChat = 20,
        Walk = 23,
        Run = 25,
        NpcOpenWindow = 29,
        Shout = 30,
        Emote = 32,
        Jump = 36,
        BeginRest = 39,
        EndRest = 42,
    }

    public enum CH9Type : byte
    {
        SelectObject = 1,
        DeselectObject = 8,

        AttackEntityMelee = 43,

        StopAttackingMelee = 50,

        AttackEntitySkill = 61,
        UseSkillWithTarget = 64,
        UseSkillWithPosition = 65,
    }

    //items
    public enum CH12Type : byte
    {
        DropItem = 7,
        LootItem = 9,
        MoveItem = 11,
        Equip = 15,
        Unequip = 18,
        UseItem = 21,
        ItemEnhance = 23,
    }

    public enum CH14Type : byte
    {
        PartyInviteToCharacter = 2,
        PartyInviteFromCharacter = 4,
        PartyInviteDecline = 5,
        LeaveParty = 10,

        PartyInformation = 72,
        PartyMatchingNewMessage = 80,
        PartyMatchingSystem = 84,
    }

    public enum CH15Type : byte
    {
        AnswerQuestion = 2,
    }

    public enum CH19Type : byte
    {
        TradeRequest = 1,
    }

    public enum CH20Type : byte
    {
        BuyHPStones = 1,
        BuySPStones = 2,
        UseHPStone = 7,
        UseSPStone = 9,
    }

    public enum CH21Type : byte
    {
        FriendList = 1,

    }

    public enum CH22Type : byte
    {

    }

    public enum CH28Type : byte
    {
        GetQuickBar = 2, //Get 1
        GetQuickBarState = 4, //Get 2
        GetGameSettings = 10, //Get 5
        GetClientSettings = 12, //Get 3
        GetShortCuts = 14, //Get 4

        SaveQuickBar = 16,
        SaveQuickBarState = 17,
        SaveGameSettings = 20,
        SaveClientSettings = 21,
        SaveShortCuts = 22,
    }

    public enum CH29Type : byte
    {
        GuildNameRequest = 118,
    }

    public enum CH31Type : byte
    {
        GetUnknown = 6, //Get 6
    }
}
