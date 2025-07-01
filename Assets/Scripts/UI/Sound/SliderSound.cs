using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSound : UIInteractionSound
{
    private Slider slider;

    public void Start()
    {
        this.slider = this.GetComponent<Slider>();
    }

    public override void OnMove(AxisEventData eventData)
    {
        bool isMovementHorizontal = eventData.moveDir == MoveDirection.Left || eventData.moveDir == MoveDirection.Right;
        bool isSliderHorizontal = this.slider.direction == Slider.Direction.LeftToRight || this.slider.direction == Slider.Direction.RightToLeft;

        if (isMovementHorizontal != isSliderHorizontal)
        {
            base.OnMove(eventData);
        }
        else
        {
            PlayConfirmSound();
        }
    }

}