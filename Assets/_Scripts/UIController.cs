using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [SerializeField]
    private float timerSpeed;
    [SerializeField]
    private Slider timingBar;
    [SerializeField]
    private Button bowl;

    private BallMovement ball;
    private bool timeIncreasing;


    private void Start()
    {
        timeIncreasing = true;
    }

    private void Update()
    {
        if (timeIncreasing) timingBar.value += Time.deltaTime * timerSpeed;
        else timingBar.value -= Time.deltaTime * timerSpeed;

        if (timingBar.value >= timingBar.maxValue && timeIncreasing) timeIncreasing = false;
        else if (timingBar.value <= 0 && !timeIncreasing) timeIncreasing = true;

        if (ball is null || !ball.Throwable) bowl.enabled = true;
        else bowl.enabled = true;
    }

    public float GetSliderValue()
    {
        return timingBar.value;
    }

    public void SetBall(BallMovement ball)
    {
        this.ball = ball;
    }

}
