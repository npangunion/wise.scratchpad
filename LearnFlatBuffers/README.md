# FlatBuffers

잘 알려진 라이브러리. 메모리 효율적이고 코드 크기가 작다. 
꽤 많은 게임 프로젝트들에서 사용하고 있다.

이를 wise.kernel에 붙이려고 한다. 내부 기능을 깊이 알 필요가 있어 
실제 구현을 통해 확인하려 한다. 

## 준비 

- c++은 flatbuffers.h 만 사용한다. 
  - base.h를 include 한다.
- flatc가 필요. 
- vscode에서 flatbuffers 지원 

- 위 파일들 복사 

## 튜토리얼 따라가기 

https://google.github.io/flatbuffers/flatbuffers_guide_tutorial.html


```
flatc --cpp monster.fbs 
```

monster_generated.h 헤더 파일 생성. 이 파일이 전부이다. 

내부 구조는 버퍼내 offset을 따라가는 구조이다. 따라서, 개별 항목들을 
버퍼내에 만들고 한번에 지정한다. 

CreateMonster() 함수를 이해하면 된다. 
미리 필요한 offset들을 확보해야 하는 것들이 있다. 
string, vector, 다른 table이 있다. 

--gen-object-api로 생성하면 Pack(), UnPackTo() 인터페이스를 제공한다. 

버퍼를 계속 들고 다녀야 하는 구조라 버퍼 메모리 관리가 꽤 까다로울 수 있다. 어떻게 하는가? 

bitsery로 만든 프로토콜이 있어 수정해서 적용한다. 
C++간에는 괜찮다. 다른 언어도 호환되게 만들 수 있을까? 


# 정리 

여기서 보는 건 여기서 마친다. 기본 사용법을 이해하는 정도이다. 

메세지를 중요한 처리 수단으로 하는 게임 서버에서 그렇게 
유연한 구조는 아니다. 매우 큰 파일에서 많은 데이터를 
읽고 쓰는데는 매우 좋은 구조일 수 있으나 논리를 작성하거나 결과를 옮겨서 결국은 보관해야 하는 경우 
중복 작업이 될 수 있다. 






