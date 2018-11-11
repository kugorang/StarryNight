#region

using UnityEngine;
using UnityEngine.UI;

#endregion

public class ListGrid : MonoBehaviour //각 ContentPanel에 붙이는 용도의 컴포넌트. 인스펙터에서 조작하기 쉽게 ~ListManager와 분리함.
{
    public int Columns = 5; //기본 열 수
    public int DefaultCellSize = 200; //기본 버튼 크기

    public int ReferenceWidth = 1080; //목표 가로 해상도

    // Use this for initialization
    private void Awake()
    {
        var grid = gameObject.GetComponent<GridLayoutGroup>(); //Get Grid
        var scale = (float) ReferenceWidth / Screen.width;
        var cellSize = (int) (DefaultCellSize / scale); //비례하게 정하기
        var spacing =
            ReferenceWidth / Columns -
            cellSize; //Canvas Scaler의 영향으로 인해 (cellSize+Spacing)*Columns=1080이어야 원하는 모습대로 나옴.
        var padding =
            (int) Mathf.Ceil(cellSize * (scale - 1) /
                             2); //ItemPanel의 Scale에 따라서 TopPadding이 없으면 이미지가 세로 길이*(Scale-1)/2만큼 짤림. *하단 참조.
        grid.spacing = new Vector2(spacing, spacing);
        grid.cellSize = new Vector2(cellSize, cellSize);
        grid.padding.top = padding;
        grid.padding.bottom = padding;
    }
}
/*패널의 중심이 좌 상단(0,0) 기준 (0.5,0.5)에 위치하는 데, Scale 때문에 꼭지점의 위치가 달라짐.
    원래               스케일 1.2 적용 후 *둘 다 패널의 중심은 (0.5,0.5)
    (0,0)(1,0)          (-0.1,-0.1)(1.1,-0.1)
    (0,1)(1,1)          (-0.1,1.1)(1.1,1.1)
     */