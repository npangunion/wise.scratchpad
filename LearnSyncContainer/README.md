# lockfree for shared state multihreading server

다중 쓰레드로 상태 공유를 사용하는 경우 락 경쟁이 성능 저하의 원인이 될 수 있다. 

그런 문제 중의 하나로 공간 상에 배치된 사용자 목록에 대한 삽입, 삭제, 조회 간의 
경쟁이 있다. 이를 해결하는 한 방법으로 락을 사용하지 않는 기법이 활용될 수 있다. 

## lockfree

여기서는 visual c++ 기준으로 Concurrency::concurrent_vector를 사용하여 
목록의 조회와 삽입/삭제 간의 락 경쟁을 피하는 방법을 고민해 본다. 

concurent_vector는 erase가 없다. 한 원소의 값을 변경하는 것도 atomic 하지 않다. 
따라서, 락이 필요하게 된다. 


