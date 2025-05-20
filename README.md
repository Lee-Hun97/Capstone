1. app.py에서 Unity 경로 수정
app.py 파일 맨 아래에 있는 build_assetbundle 함수에서

Unity Editor 실행 경로 (UNITY_EXE)와

Unity 프로젝트 경로 (UNITY_PROJECT)를

자기 컴퓨터 환경에 맞게 바꿔야 함

이때, Unity 프로젝트 경로는 아래 2번에서 만들 Unity 프로젝트 위치와 같아야 함

2. Unity 프로젝트 생성
Unity Hub에서 새 프로젝트를 하나 만든다

생성 경로는 1번에서 설정한 Unity 프로젝트 경로와 같아야 함

템플릿은 아무거나 상관없다 (3D 권장)

3. Assets/ImportedModels 폴더 생성
Unity 프로젝트 안에서 Assets 폴더 안에 ImportedModels라는 폴더를 만든다

이 폴더는 서버에서 자동으로 FBX 파일을 복사해 넣는 용도로 사용됨

4. Assets/Editor/AssetBundleBuilder.cs 추가
Assets 폴더 안에 Editor라는 폴더를 새로 만든다 (없으면 직접 생성)

그 안에 AssetBundleBuilder.cs 파일을 추가한다

이 파일에는 서버에서 Unity를 CLI로 실행할 때 호출되는 번들 생성 코드가 들어 있어야 한다

5. Unity 플랫폼을 Android로 전환
Unity 상단 메뉴에서 File > Build Settings를 연다

Android 플랫폼을 선택한 뒤, 오른쪽 아래 Switch Platform을 누른다

플랫폼이 Android로 전환되어야 Android용 번들이 정상적으로 만들어짐
