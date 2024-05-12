
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
    ReqBegin = 1001,

    ReqLogin = 1002,
    ReqRoomEnter = 1003,
    ReqRoomLeave = 1004,
    ReqRoomChat = 1005,
    ReqReadyGame = 1006,
    ReqPutOmok = 1007,
    ReqRoomDevAllRoomStartGame = 1008,
    ReqRoomDefAllRoomEndGame = 1009,
    ReqHeartBeat = 1010,

    ReqEnd = 1100,

    ResLogin = 1101,
    ResRoomEnter = 1102,
    ResRoomLeave = 1103,
    ResReadyGame = 1104,
    ResPutOmok = 1105,
    ResRoomDevAllRoomStartGame = 1106,
    ResRoomDefAllRoomEndGame = 1107,
    ResHeartBeat = 1108,

    NtfMustClose = 1201,
    NftRoomUserList = 1202,
    NtfRoomNewUser = 1203,
    NtfRoomLeaveUser = 1204,
    NtfRoomChat = 1205,
    NtfReadGame = 1206,
    NtfStartGame = 1207,
    NtfPutOmok = 1208,
    NtrWinnerOmok = 1209,
    NtrTimeOutOmok = 1210,
    NtfInConnectClient = 1211,
    NtfInDisconnectClient = 1212,
    NtfInForceDisconnectClient = 1213,
    NtfIntRoomLeave = 1214,
    NtrInUserCheck = 1215,
    NtfInRoomCheck = 1216,
    NtfInLoginCheck=1217,

    //DB이너패킷
    NtfInUpdateWinnerResult = 1301,
    NtfInUpdateLoserResult = 1302,
    NtfInUpdateDrawResult = 1303,


    //Redis이너패킷
   


    // 시스템, 서버 - 서버
    SS_START = 8001,


    // DB 8101 ~ 9000
    ReqDBLogin = 8101,
    ResDBLogin = 8102,

    
}

