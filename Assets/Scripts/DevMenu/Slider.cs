using UnityEngine;

public class Slider : MonoBehaviour
{

    [Header("Slider GameObjects")]
    public GameObject sliderBall;
    public GameObject sliderTop;

    [Header("Slider SpriteRenderers")]
    public int lowestSortingOrder;
    public SpriteRenderer tens;
    public SpriteRenderer ones;
    public SpriteRenderer sliderUnder;
    public SpriteRenderer symbol;

    [Header("Slider Sprites")]
    public Sprite[] numberSprites = new Sprite[10];

    [Header("Slider Total Value")]
    public float sliderTotalValue;
    public int sliderCurrentValue;
    private float sliderValue; //value ranging 0 - 1 based on ball position on slider

    private Vector2 mouseWorldPosition;
    private Camera mainCamera;

    private bool sliderActive;

    void Start()
    {
        mainCamera = Camera.main;

        tens.sortingOrder = lowestSortingOrder;
        ones.sortingOrder = lowestSortingOrder;
        symbol.sortingOrder = lowestSortingOrder;
        sliderUnder.sortingOrder = lowestSortingOrder;
        sliderTop.GetComponent<SpriteRenderer>().sortingOrder = lowestSortingOrder + 1;
        sliderBall.GetComponent<SpriteRenderer>().sortingOrder = lowestSortingOrder + 2;
    }

    void Update()
    {

        Vector3 mouseScreenPosition = Input.mousePosition;

        mouseWorldPosition = mainCamera.ScreenToWorldPoint(new Vector3(
            mouseScreenPosition.x,
            mouseScreenPosition.y,
            Mathf.Abs(mainCamera.transform.position.z)));

        if (DetectMouse(sliderBall, 16, 16) && !Input.GetKey(KeyCode.Mouse0))
        {
            sliderBall.transform.localScale = new Vector2(1.5f, 1.5f);
        }
        else
        {
            sliderBall.transform.localScale = new Vector2(1f, 1f);
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && DetectMouse(sliderBall, 128, 16))
        {
            sliderActive = true;
        }

        if (sliderActive)
        {
            float tempX = Mathf.Clamp(mouseWorldPosition.x,
            sliderTop.transform.position.x, sliderTop.transform.position.x + 3f);

            sliderBall.transform.position = Vector2.Lerp(sliderBall.transform.position, new Vector2(tempX, sliderBall.transform.position.y), Time.deltaTime * 20f);
            sliderTop.transform.localScale = new Vector3(sliderValue, 1);
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            sliderActive = false;
        }

        sliderValue = (sliderBall.transform.position.x - sliderTop.transform.position.x) / 3f;
        tens.sprite = numberSprites[Mathf.FloorToInt(sliderValue * sliderTotalValue / 10)];
        ones.sprite = numberSprites[Mathf.FloorToInt(sliderValue * sliderTotalValue % 10)];
        sliderCurrentValue = Mathf.FloorToInt(sliderValue * sliderTotalValue);
    }


private bool DetectMouse(GameObject obj, float xBounds, float yBounds)
    {
        xBounds /= 32f;
        yBounds /= 32f;

        if (mouseWorldPosition.x > obj.transform.position.x - xBounds
        && mouseWorldPosition.x < obj.transform.position.x + xBounds
        && mouseWorldPosition.y > obj.transform.position.y - yBounds
        && mouseWorldPosition.y < obj.transform.position.y + yBounds)
        {
            return true;
        }
        else
        {
            return false;
        }
    }


}
