# 통신 구현 

MsgPack 기반. 약간 더 일반화 해서 Tcp 상에서 임의의 프로토콜 실행이 가능하도록 정리한다. 



## 역할 

- IProtocol 
  - 최상위 인터페이스 
- ProtocolTcp
  - SessionTcp 를 내부에서 사용 
  - Tcp 통신은 여기서 다 처리 
- MsgPackProtocol 
  - MsgPack 통신이 거의 다 됨 



