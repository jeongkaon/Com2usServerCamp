SocketServer
- [x] 방입장(최대2명까지)
- [x] 방 나가기
- [x] 방 채팅
- [x] 게임시작
    - [x] 두명이 모두 게임 시작을 요청하면 바로 게임시작
- [x] 돌두기
- [x] 타이머
    - [x] 시간 제한 넘으면 상대에게 턴 자동으로 넘기기
    - [x] 연속으로 6번 자동으로 턴이 넘어가면 게임취소 
- [x] 오목로직
- [ ] 게임종료
    - [x] 게임이 끝나면 서버는 결과를 알려준다.
    - [ ] 클라이언트 결과표시 
- [x] 주기적으로 유저 상태 조사
    - [x] heart_beat구현 
- [x] 주기적으로 방 상태 조사
    - [x] 게임시작 중 턴을 받지 않은 유저가 돌을 두지 않은 경우
    - [x] 너무 오랫동안 게임이 진행중인 방 (-> 무승부로 처리)
     
- [ ] 로그인에 DB 연동
    - [ ] Redis에 있는 유저 정보로 로그인 체크  
- [ ] 게임결과 DB 저장
    - [ ] 게임 DB에 저장한다. 

