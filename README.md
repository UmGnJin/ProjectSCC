# ProjectSCC

-이 프로젝트는 BSP, Cellular Automata, Perlin Noise를 사용한 2D Level Generation과
개인적인 기획안인 SCC(Shape Chasing Connection)에 대한 구현과 시각화, 일부 지표 측정을 목표로 하고 있습니다.

-Unity 프로젝트 폴더와 함께 Script 폴더는 매 변경마다 코드 비교 및 저장을 위해 개별로 업로드하여 주차별 진척도를 확인할 수 있습니다.

-지표 측정 시 측정 기준 / 측정 수치 / 실제 측정 방법을 통해 같은 조건에서 프로젝트를 다운로드하여 측정 가능하도록 할 예정입니다.

-직접 테스트를 진행할 경우, Unity 2020.3.12f1 버전에서 프로젝트를 실행하고 다음 절차를 따라야 합니다.
  1. 현재 Scene이 SampleScene인지 확인하고, 아닐 경우 Project 창에서 Assets > Scenes 로 이동 후 SampleScene를 열어야 합니다.
  2. Hierarchy 창에서 LevelManager 오브젝트를 선택, Inspector 창을 확인합니다.
  3. Transform 하단에 아무것도 없는지 확인하고, 있다면 우클릭 > Remove Component로 제거합니다.
  4. Project 창에서 Assets > Scripts 창으로 이동합니다.
  5. CA, BSP, PN의 확인을 원할 경우, 각 이름이 쓰인 폴더로 들어가 ~Manager로 끝나는 스크립트 파일을 찾습니다.
  5-1. SCC의 확인을 원할 경우, Assets > Scripts에 있는 LevelManager를 찾습니다.
  6. 파일을 Inspector 창의 Transform 아래 드래그&드롭하여 추가합니다.
  7. 파일을 더블클릭하여 스크립트를 엽니다.
  8. 파일 상단의 주석을 통해 세부 사항을 조절합니다.
  9. 유니티 창으로 돌아와, 상단의 재생 버튼을 눌러 실행합니다.
  10. Console 창과 Game 창을 통해 정보를 확인합니다.

*Shape Chasing Connection(SCC)이란?

- SCC는 방의 배치와 형태를 중심으로 하는 절차적 레벨 생성 방식을 의미합니다.

- 도형, 선 등의 레벨 형태를 사전 설정하여 이를 기반으로 방을 배치할 수 있습니다.

- 배치된 방의 외부 방향으로 가지 형태의 추가 방을 배치할 수 있습니다.

- 방의 배치는 이전 방의 이웃이 될 거리로 결정되어, 이들 간의 통로 연결이 진행됩니다.
