using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections.Generic;

public class UIButtonEffects : MonoBehaviour
{
    [Header("Daftar Tombol")]
    [SerializeField] private List<Button> uiButtons = new List<Button>();

    [Header("Animasi")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float clickScale = 0.9f;
    [SerializeField] private float duration = 0.15f;

    private Dictionary<Button, Vector3> originalScales =
        new Dictionary<Button, Vector3>();

    private void Start()
    {
        foreach (Button button in uiButtons)
        {
            if (button == null) continue;

            Button currentButton = button;
            originalScales[currentButton] = currentButton.transform.localScale;

            EventTrigger trigger =
                currentButton.GetComponent<EventTrigger>();

            if (trigger == null)
                trigger = currentButton.gameObject.AddComponent<EventTrigger>();

            AddEvent(trigger, EventTriggerType.PointerEnter,
                () => Hover(currentButton, true));

            AddEvent(trigger, EventTriggerType.PointerExit,
                () => Hover(currentButton, false));

            AddEvent(trigger, EventTriggerType.PointerDown,
                () => Click(currentButton));

            AddEvent(trigger, EventTriggerType.PointerUp,
                () => Hover(currentButton, false));
        }
    }

    private void AddEvent(
        EventTrigger trigger,
        EventTriggerType type,
        System.Action action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;
        entry.callback.AddListener(_ => action());
        trigger.triggers.Add(entry);
    }

    private void Hover(Button button, bool isHover)
    {
        if (button == null || !button.interactable) return;

        if (isHover && Musicmanager.Instance != null)
            Musicmanager.Instance.PlayButtonHoverSFX();

        Animate(button, isHover ? hoverScale : 1f);
    }

    private void Click(Button button)
    {
        if (button == null || !button.interactable) return;

        if (Musicmanager.Instance != null)
            Musicmanager.Instance.PlayButtonClickSFX();

        Animate(button, clickScale);
    }

    private void Animate(Button button, float multiplier)
    {
        if (button == null) return;

        Transform target = button.transform;
        target.DOKill();

        target.DOScale(
            originalScales[button] * multiplier,
            duration
        ).SetEase(Ease.OutBack);
    }

    private void OnDestroy()
    {
        foreach (Button button in uiButtons)
        {
            if (button != null)
                button.transform.DOKill();
        }
    }
}
