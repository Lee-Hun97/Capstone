계량 스크립트 자체에는 수동으로 할당해야 하는 필드는 없애고 다 자동으로 할당되게 만들어놨습니다.
작동 원리는 Assets/Editor 폴더에 CupModel Postprocessor 스크립트와 MugAutoPlacer 스크립트를 저장해놓고
스캔한 mesh를 Assets/Mugs 폴더에 import하는 과정을 거치면 자동적으로 Scene배치 및 필요한 컴포넌트들이 할당되게 됩니다.
폴더를 코드에 지정해 놓는 형식이기에 폴더의 이름(Mugs와 같은)을 변경하시면 안되고 변경을 하실거면 CupModelPostprocessor 스크립트를 변경해야합니다.
또한 Scene상의 EventSystem과 Canvas에 배치 해 놓은 inputField, Button의 Tag를 각각 VolumeInputField, GenerateButton으로 할당하셔야 문제가 없습니다.
