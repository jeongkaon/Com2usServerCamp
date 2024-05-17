using APIServer.Controllers;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Threading;

namespace APIServer;

public interface IMatchWoker : IDisposable
{
    public void AddUser(string userID);

    public (bool, CompleteMatchingData) GetCompleteMatching(string userID);
}

public class MatchWoker : IMatchWoker
{
    //스케일업인지 먼지 그거하기위한 거인듯.
    List<string> _pvpServerAddressList = new();

    System.Threading.Thread _reqWorker = null;      //펍ㅇ되는거..?필요한가??

    //여기다가 id를 넣는다 2명이상되면 2명 id를 묶어서 레디스 리스트에 넣는다.
    //레디스를 통해서 통신한다.
    //그걸 소켓서버(따로 쓰레드)는 요청오면 매칭된 2명의 id 들어있는 걸 꺼낸다 레디스에서
    //roomgr에서 대전이 가능한 방. 0명인방을 하나 골라서 서버주소(스케일아웃용)랑, 포트번호, 룸번호, 매칭된 유저2명보내서
    //아까 레디스리스트거기에 list해서 따로 만들어서 넣어.
    //그럼 api서버에서 스레드 돌고있어서 레디스에서 꺼내서 적혀잇는 데이터대로 
    //클라에서는 업데이트함수같은거로 매칭요청을 보낸이후에 

    //매칭자를 등록한다. 매칭순서로
    
    ConcurrentQueue<string> _reqQueue = new();

    System.Threading.Thread _completeWorker = null;     //응답을 저장하는 역할, 섭이 되는거다.

    // key는 유저ID
    //매칭 완료된 데이터만 들고있다.
    //string을 저장하고 있어야하나?CompleteMatchingData이게 아니라?
    //매칭된애들 id, id 저장하는듯?
    ConcurrentDictionary<string, string> _completeDic = new();

    //TODO: 2개의 Pub/Sub을 사용하므로 Redis 객체가 2개 있어야 한다.
    // 매칭서버에서 -> 게임서버, 게임서버 -> 매칭서버로

    string _redisAddress = "";
    


    public MatchWoker(IOptions<MatchingConfig> matchingConfig)
    {
        Console.WriteLine("MatchWoker 생성자 호출");
        //얘는 언제 넣는거임??
        _pvpServerAddressList.Add(matchingConfig.Value.)


        _redisAddress = matchingConfig.Value.RedisAddress;

        //TODO: Redis 연결 및 초기화 한다
        //redis connect생성해서 연결해야한다.


        _reqWorker = new System.Threading.Thread(this.RunMatching);
        _reqWorker.Start();

        _completeWorker = new System.Threading.Thread(this.RunMatchingComplete);
        _completeWorker.Start();
    }
    
    public void AddUser(string userID)
    {
        //이 큐에서 매칭자처리를 해야한다.
        _reqQueue.Enqueue(userID);
    }

    public (bool, CompleteMatchingData) GetCompleteMatching(string userID)
    {
        //겜서버 요청오면 여기서 뒤진다.
        //Check컨트롤로에서 Post그거 오면 이게 호출된다.

        //TODO: _completeDic에서 검색해서 있으면 반환한다.
        var res = _completeDic.ContainsKey(userID);
        if (res == false)
        {
            return (false, null);
            
        }
        //있으면
        var temp = _completeDic.Values;
        CompleteMatchingData matchingRes = new CompleteMatchingData()
        {
            //값 채워넣기.
            //방이랑, 서버주소, 포트번호 등등 넣어야함
            //그리고 보내줘야한다.
        };

        return (res, matchingRes);
        
    }

    void RunMatching()
    {
        while (true)
        {
            try
            {
               
                if (_reqQueue.Count < 2)
                {
                    System.Threading.Thread.Sleep(1);
                    continue;
                }

                //TODO: 큐에서 2명을 가져온다. 두명을 매칭시킨다
                //매칭처리하는거임
                //간단하게 이 큐에 2명 이상이면 게임을 할 수 있는거다
                //2명되면 레디스에 list로 묶어서 전달한다.
                if (_reqQueue.TryDequeue(out string player1) && _reqQueue.TryDequeue(out string player2))
                {
                    // 매칭된 결과를 생성
                    string matchResult = $"{player1},{player2}";

                    // 매칭된 결과를 Redis의 List에 추가
                    db.ListRightPush("match_requests", matchResult);
                }


                //2명을 매칭시키고 레디스에 물어본다.?
                //겜서버한테 물어본다??
                //A와 B에 빈방줄수있는 게임서버가 있다면 나한테 알려조!



            }
            catch (Exception ex)
            {

            }
        }
    }

    void RunMatchingComplete()      //sub하고있다가 메시지오면 요청했던 응답이 여기로온다.
    {
        //매칭된 결과를 처리하여 게임 서버로 보내는 코드를 작성해 보겠습니다.
        while (true)
        {
            try
            {
                //TODO: Redis의 Pub/Sub을 이용해서 매칭된 결과를 게임서버로 받는다

                //TODO: 매칭 결과를 _completeDic에 넣는다
                // 2명이 하므로 각각 유저를 대상으로 총 2개를 _completeDic에 넣어야 한다
                //결과값에 넣을때는 a와 b각각 넣어줘야한다. 그래야지 여기서 a,b가 mat요청할때 _complet를 뒤질거임
                //a에 대한 정보 있으면 매칭된거고 없으면 매칭해야한다.
                //데이터하나를 a용으로한번, b용으로 한번 넣는거다.

                string result = db.ListLeftPop("match_responses");
                if (!string.IsNullOrEmpty(result))
                {
                    Console.WriteLine("Received response: " + result);

                    // 응답 형식: "RoomNumber:1234, ServerIP:192.168.1.1, Players:player1,player2"
                    string[] parts = result.Split(new char[] { ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length == 6 && parts[0] == "RoomNumber" && parts[2] == "ServerIP" && parts[4] == "Players")
                    {
                        string roomNumber = parts[1];
                        string serverIP = parts[3];
                        string player1 = parts[5];
                        string player2 = parts[6];

                        // 매칭 결과를 _completeDic에 저장
                        _completeDic.TryAdd(player1, $"Room: {roomNumber}, ServerIP: {serverIP}");
                        _completeDic.TryAdd(player2, $"Room: {roomNumber}, ServerIP: {serverIP}");
                        Console.WriteLine($"Match complete: {player1} vs {player2} in Room {roomNumber} on Server {serverIP}");
                    }
                }


                //요청이왔을때 DIC을 뒤질거다.
                //여기를 뒤저서 A대한 정보가 있으면 매칭된거임.


            }
            catch (Exception ex)
            {

            }
        }        
    }



    public void Dispose()
    {
        Console.WriteLine("MatchWoker 소멸자 호출");
    }
}


//소켓서버에서 매칭서버에 답변줄때는 이거주면된다
//여기에 빠진게 있는데 player유저에 대한 2명의 id도 들어가야한다.
//그래야 id를 key값으로 사용할 수 있기때문이다.
public class CompleteMatchingData
{    
    public string ServerAddress { get; set; }
    public int RoomNumber { get; set; }
    public string User1 { get; set; }
    public string User2 { get; set; }
}


public class MatchingConfig     //레디스할때 값 들고오는거임.
{
    public string RedisAddress { get; set; }
}

//gpt 소켓서버 응답
class GameServer
{
    static void Main(string[] args)
    {
        ConnectionMultiplexer redis = ConnectionMultiplexer.Connect("localhost");
        IDatabase db = redis.GetDatabase();

        while (true)
        {
            try
            {
                // 매칭 요청을 처리
                string matchRequest = db.ListLeftPop("match_requests");
                if (!string.IsNullOrEmpty(matchRequest))
                {
                    Console.WriteLine("Received match request: " + matchRequest);

                    // 매칭 요청을 처리하고 빈 방 번호와 대전 서버 IP 주소를 매칭 API 서버로 전송
                    // 예를 들어, roomNumber와 serverIP를 생성하거나 가져오는 로직을 추가합니다.
                    string roomNumber = "1234"; // 빈 방 번호 예시
                    string serverIP = "192.168.1.1"; // 서버 IP 예시
                    string[] players = matchRequest.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    string player1 = players[0];
                    string player2 = players[1];

                    string response = $"RoomNumber:{roomNumber}, ServerIP:{serverIP}, Players:{player1},{player2}";
                    db.ListRightPush("match_responses", response);
                    Console.WriteLine("Match response sent: " + response);
                }

                // 대기열이 비어있으면 잠시 대기
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                // 예외 처리
                Console.WriteLine($"Exception: {ex.Message}");
            }
        }
    }
}
