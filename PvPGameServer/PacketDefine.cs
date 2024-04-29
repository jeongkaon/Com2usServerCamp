﻿namespace PvPGameServer;


public enum ERROR_CODE : short
{
    NONE = 0, 

    // 서버 초기화 에러
    REDIS_INIT_FAIL = 1,    // Redis 초기화 에러

    // 로그인 
    LOGIN_INVALID_AUTHTOKEN = 1001, // 로그인 실패: 잘못된 인증 토큰
    ADD_USER_DUPLICATION = 1002,
    REMOVE_USER_SEARCH_FAILURE_USER_ID = 1003,
    USER_AUTH_SEARCH_FAILURE_USER_ID = 1004,
    USER_AUTH_ALREADY_SET_AUTH = 1005,
    LOGIN_ALREADY_WORKING = 1006,
    LOGIN_FULL_USER_COUNT = 1007,

    DB_LOGIN_INVALID_PASSWORD = 1011,
    DB_LOGIN_EMPTY_USER = 1012,
    DB_LOGIN_EXCEPTION = 1013,

    //방
    ROOM_ENTER_INVALID_STATE = 1021,
    ROOM_ENTER_INVALID_USER = 1022,
    ROOM_ENTER_ERROR_SYSTEM = 1023,
    ROOM_ENTER_INVALID_ROOM_NUMBER = 1024,
    ROOM_ENTER_FAIL_ADD_USER = 1025,

    ROOM_ENTER_FAILED_USERFULL = 1028,

    ROOM_NOTALL_READY=1026,     //방안에 다 아직 레디안됨
    ROOM_ALL_READY = 1027,      //다 준비함

    
    
}

public enum PACKET_ID : int
{
    REQ_SC_TEST_ECHO = 101,


    // 클라이언트
    CS_BEGIN = 1001,

    CS_LOGIN = 1002,
    SC_LOGIN = 1003,
    NTF_MUST_CLOSE = 1005,

    CS_ROOM_ENTER = 1015,
    SC_ROOM_ENTER = 1016,
    NTF_ROOM_USER_LIST = 1017,
    NTF_ROOM_NEW_USER = 1018,

    CS_ROOM_LEAVE = 1021,
    SC_ROOM_LEAVE = 1022,
    NTF_ROOM_LEAVE_USER = 1023,

    CS_ROOM_CHAT = 1026,
    NTF_ROOM_CHAT = 1027,

    //게임관련
    CS_READY_GAME = 1031,
    SC_READY_GAME = 1032,
    NTR_READY_GAME = 1033,

    NTF_START_GAME = 1034,

    CS_PUT_OMOK = 1035,
    SC_PUT_OMOK = 1036,
    NTF_PUT_OMOK = 1037,

    //

    CS_ROOM_DEV_ALL_ROOM_START_GAME = 1091,
    SC_ROOM_DEV_ALL_ROOM_START_GAME = 1092,

    CS_ROOM_DEV_ALL_ROOM_END_GAME = 1093,
    SC_ROOM_DEV_ALL_ROOM_END_GAME = 1094,

    CS_END = 1100,


    // 시스템, 서버 - 서버
    SS_START = 8001,

    NTF_IN_CONNECT_CLIENT = 8011,
    NTF_IN_DISCONNECT_CLIENT = 8012,

    CS_SS_SERVERINFO = 8021,
    SC_SS_SERVERINFO = 8023,

    CS_IN_ROOM_ENTER = 8031,
    SC_IN_ROOM_ENTER = 8032,

    NTF_IN_ROOM_LEAVE = 8036,


    // DB 8101 ~ 9000
    REQ_DB_LOGIN = 8101,
    RES_DB_LOGIN = 8102,
}

