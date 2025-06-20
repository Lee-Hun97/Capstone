# 세종대 소프트웨어학과 25_1 Capstone 10조 V-Mersurment

AR을 활용한 요리 계량용 모바일 AR앱.(스마트 계량으로 정확도를 높이다)

[![발표 영상](https://img.youtube.com/vi/wSfLEdbrC98/0.jpg)](https://youtu.be/wSfLEdbrC98)
<p align="center">발표 영상(클릭 시 이동)</p>

## 1. 개요
- 장르 : AR, 모바일
- 플랫폼 : 모바일 (Android)
- 배포 방식 : 없음(test는 apk를 생성해서 사용)
- 개발 기간 : 2~3개월(2025.3.14 ~ 2025.05.30 : 기획 기간 포함)
- 개발 인원 : 4명 (<a href="https://github.com/kimmingu0506">김민규</a>, <a href="https://github.com/kim-you-feel">김유필(팀장)</a>, <a href="https://github.com/playharder1313" title="GitHub Profile">김준범</a>, <a href="https://github.com/leehun1997">이훈</a>)



## 2. 앱 사용 방법
1. 사용자는 정보를 입력하여 로그인 하여 저장된 정보를 가져올 수 있습니다.
2. 모델 생성을 선택하면 촬영 씬으로 이동하여 여러 장의 이미지를 촬영하여서 모델 생성을 할 수 있습니다.
3. 모델 생성 완료 후(+저장된 모델 생성 시) 마커를 인식해서 해당 모델을 생성해줍니다.
4. lock버튼을 통해서 모델을 고정시키고 원하는 계량값을 입력해 계량을 위한 가이드라인을 생성합니다.
5. 해당 가이드라인을 따라서 계량을 완료한 후 모델을 저장할지 앱을 그대로 앱을 종료할지 선택할 수 있습니다.

## 3. 사용한 도구
- 협업 도구
  - Git, Github Desktop
- 개발 도구
  - unity
  - SQLite
  - C#
  - Python
  - Kotlin
  - Unity (2022.3.17f1 ver)
  - Visual studio(+code)
- 프레임워크 와 라이브러리
  - Flask(+Reality Capture)
  - OpenCV
  - AR Core, Foundation
  - Android sdk
    
## 4. 게임의 흐름과 사용한 기술
### a. 인앱 기능(수정 중)

<details>
<summary><b>기능1</b></summary>
<br>기능1 설명
</details>

<details>
<summary><b>기능2</b></summary>
<br>기능2 설명
</details>

<details>
<summary><b>기능3</b></summary>
<br>기능3 설명
</details>

### b. 인앱 외의 기능

<details>
<summary><b>기능1</b></summary>
<br>기능1 설명
</details>

<details>
<summary><b>기능2</b></summary>
<br>기능2 설명
</details>

<details>
<summary><b>기능3</b></summary>
<br>기능3 설명
</details>

## 5. 기획
  
#### a. 시작
- "현실을 향상시키는 혼합현실" 이라는 주제를 선택하고 많은 이야기가 나오던 중 요리에 관한 논의가 이루어졌고 그 중 계량이라는 주제로 매우 좋은 아이디어가 나왔고 이를 조금 다듬어서 현재의 주제가 선정되었습니다.

#### b. 핵심주제
- 현재의 계량 방식은 "한컵 반" 과 같은 추상적인 계량 방식이 존재한다, 또한 다운로드 수가 많은 계량 앱의 경우에도 텍스트로만 제공하거나 추상적 계량 방식이 있다.
  - 계량 기구가 사용이 불가능한 사람에게도 정확한 계량이 가능하게 만들어주고 추상적인 계량 방식의 사용을 줄이는 방법을 제시하겠다.
- 아이폰의 LiDAR 같은 고급 기능이 존재하나 해당 기능은 iPhone 12 Pro 이상의 고사양 최신 버전에서만 사용이 가능하다.
  - 스마트폰만 있다면 누구나 사용이 가능한 AR 앱을 만들겠다.

 #### c. 개선해야하는 점
- 현재 앱의 정확도는 90% 정도이지만 앱의 정확도를 99%(초정밀)까지 높이는 것
- Reality Capture에서 발생하는 문제나 오류를 처리하는 자동화 방식을 만드는 것
- 마커의 사용을 줄이거나 없애는 방법에 대한 고민
