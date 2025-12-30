using UnityEngine;

public class PlayerController : MonoBehaviour
{

    [SerializeField]
    private PositionController marker;
    [SerializeField]
    private BallMovement ball;
    [SerializeField]
    private UIController ui;

    private BallEffect activeEffect;
    private bool activeSide;


    private void Start()
    {
        activeEffect = BallEffect.Swing;
        activeSide = false;
        ui.SetBall(ball);
    }

    public void SetSwingActive()
    {
        activeEffect = BallEffect.Swing;
        print("Swing");
    }

    public void SetSpinActive()
    {
        activeEffect = BallEffect.Spin;
        print("Spin");
    }

    public void ThrowBall()
    {
        if (ball is null || marker is null || ui is null) return;
        if (!ball.Throwable) return;

        float sliderValue = ui.GetSliderValue();
        float effectStrength = CalculateEffectStrength(sliderValue);

        ball.Activate();
        ball.ThrowBall(marker.GetPosition(), activeEffect, activeSide, effectStrength);
    }

    public void ChangeSide()
    {
        activeSide = !activeSide;
    }

    private float CalculateEffectStrength(float value)
    {
        if (value < 1) return 0;
        else if (value < 2) return .4f;
        else if (value < 3) return .7f;
        else if (value < 4) return 1;
        else if (value < 5) return .4f;
        else if (value < 6) return .7f;
        else return 0;
    }

}
