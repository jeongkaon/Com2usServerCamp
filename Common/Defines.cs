
//에러종류, 패킷종류 등 필요한 enum들 여기다가 

public enum StoneType //: int
{
    None = 0,
    Black = 1,
    White = 2
}



public enum ErrorCode : short
{
    None = 0,

    // 서버 초기화 에러
    RedisInitFail = 1,    // Redis 초기화 에러

    // 로그인 
    LoginInvalidAuthToken = 1001, // 로그인 실패: 잘못된 인증 토큰
    AddUserDuplication = 1002,
    RemoveUserSearchFailureUserId = 1003,
    UserAuthSearchFailureUserId = 1004,
    UserAuthAlreadySetAuth = 1005,
    LoginAlreadyWorking = 1006,
    LoginFullUserCount = 1007,

    DBLoginInvalidPassword = 1011,
    DBLoginEmptyUser = 1012,
    DBLoginException = 1013,

    //방
    RoomEnterInvalidState = 1021,
    RoomEnterInvalidUser = 1022,
    RoomEnterErrorSystem = 1023,
    RoomEnterInvalidRoomNumber = 1024,
    RoomEnterFailAddUser = 1025,

    RoomEnterFaildUserFull = 1028,

    //방조사
    RoomCheckInputOnePlayer = 1029,
    RommCheckZeroReady = 1032,
    RoomCheckOnePlayerNotReady = 1030,
    RoomCheckTwoPlayersNotReady = 1031,

    RoomNotAllReady = 1026,
    RoomAllReady = 1027,



}
public enum PacketId : int
{
    ReqSCTestEcho = 101,


    // 클라이언트
    CSBegin = 1001,

    ReqLogin = 1002,
    ResLogin = 1003,
    NtfMustClose = 1005,

    ReqRoomEnter = 1015,
    ResRoomEnter = 1016,
    NftRoomUserList = 1017,
    NtfRoomNewUser = 1018,

    ReqRoomLeave = 1021,
    ResRoomLeave = 1022,
    NtfRoomLeaveUser = 1023,

    ReqRoomChat = 1026,
    NtfRoomChat = 1027,

    //게임관련
    ReqReadyGame = 1031,
    ResReadyGame = 1032,
    NtfReadGame = 1033,

    NtfStartGame = 1034,

    ReqPutOmok = 1035,
    ResPutOmok = 1036,
    NtfPutOmok = 1037,

    NtrWinnerOmok = 1038,

    NtrTimeOutOmok = 1039,

    ReqRoomDevAllRoomStartGame = 1091,
    ResRoomDevAllRoomStartGame = 1092,

    ReqRoomDefAllRoomEndGame = 1093,
    ResRoomDefAllRoomEndGame = 1094,

    ReqHeartBeat = 1095,
    ResHeartBeat = 1096,
    CSEnd = 1100,

    // 시스템, 서버 - 서버
    SS_START = 8001,

    NtfInConnectClient = 8011,
    NtfInDisconnectClient = 8012,
    NtfInForceDisconnectClient = 8013,

    NtfIntRoomLeave = 8036,

    NtrInUserCheck = 8037,
    NtfInRoomCheck = 8038,


    // DB 8101 ~ 9000
    ReqDBLogin = 8101,
    ResDBLogin = 8102,
}

