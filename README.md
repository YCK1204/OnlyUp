## 기술 스택 (Tech Stack)
  카메라 시스템

  - Cinemachine: 2.9.7 (3인칭 카메라 시스템)
  - 3rd Person Follow: 부드러운 카메라 추적
  - Dynamic Camera Damping: 카메라 흔들림 방지

  아키텍처 패턴

  - State Pattern: 플레이어 상태 관리 (Idle, Walk, Run, Sprint, Jump)
  - Singleton Pattern: 매니저 클래스들 (UIManager, GameManager)
  - Command Pattern: 입력 처리 시스템

  ## 구현된 게임 시스템

  플레이어 컨트롤

  - 정교한 지면 감지: 8점 Raycast 시스템
  - 스태미나 시스템: 상태별 차등 소모/회복
  - 물리 기반 점프: Rigidbody ForceMode.Impulse

  UI/UX 시스템

  - 실시간 스탯 표시: HP/Stamina 바
  - 마우스 호버 상호작용: 오브젝트 정보 표시
  - 키보드 상호작용: E키 표준화

  데이터 관리

  - ScriptableObject: 아이템/오브젝트 데이터 관리
  - 포션 시스템: 지속 효과 아이템

  애니메이션

  - Animator Controller: Blend Tree 기반 이동 애니메이션
  - 상태 연동: 플레이어 상태와 애니메이션 동기화
  - Human Animation: Kevin Iglesias 휴먼 애니메이션 에셋

  ## 핵심 기능
  플레이어 시스템

  // State Pattern 구현
  IPlayerState currentState = PlayerIdleState.Instance;
  
  PlayerStateContext context; // 공유 데이터 컨텍스트

  상호작용 시스템

  - 마우스 기반 오브젝트 감지
  - E키 상호작용 표준화
  - 정보 UI 자동 표시

  카메라 시스템

  - Cinemachine 3rd Person Follow
  - 부드러운 Damping (1, 0.8, 1)
  - 카메라 거리 2.5 유닛

  물리 시스템

  - 커스텀 지면 감지 (Unity CharacterController 미사용)
  - Transform 기반 이동 (물리 독립적)
  - 점프만 Rigidbody 물리 적용
