﻿using System;
using System.Collections.Generic;
using System.Text;

namespace CSCommon;

// 1001 ~ 2000
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

    CS_KEYINPUT = 1035,
    SC_KEYINPUT = 1036,
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


    //BEGIN = 1001,

    //SC_LOGIN = 1002,
    //CS_LOGIN = 1003,

    //NTF_MUST_CLOSE = 1005,

    //CS_ROOM_ENTER = 1015,
    //SC_ROOM_ENTER = 1016,
    //NTF_ROOM_USER_LIST = 1017,
    //NTF_ROOM_NEW_USER = 1018,

    //CS_ROOM_LEAVE = 1021,
    //SC_ROOM_LEAVE = 1022,
    //NTF_ROOM_LEAVE_USER = 1023,

    //CS_ROOM_CHAT = 1026,
    //ResRoomChat = 1027,      //흠...
    //NTF_ROOM_CHAT = 1028,

    //CS_READY_GAME = 1031,
    //SC_READY_GAME = 1032,
    //NTR_READY_GAME = 1033,

    //NTF_START_GAME = 1034,

    //CS_KEYINPUT = 1035,
    //SC_KEYINPUT = 1036,
    //NTF_PUT_OMOK = 1037,

    //NTF_END_GAME = 1038,


    //END = 1100
}
