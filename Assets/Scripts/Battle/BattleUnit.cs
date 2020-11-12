using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] PokemonBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Pokemon Pokemon { get; set; }

    Image image;
    Vector3 originalPos;
    Color originalColor;
    private void Awake()
    {
        image = GetComponent<Image>();
        originalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    public void Setup()
    {
       Pokemon = new Pokemon(_base, level);
        if (isPlayerUnit)
            image.sprite = Pokemon.Base.BackSprite;
        else
            image.sprite = Pokemon.Base.FrontSprite;

        PlayerEnterAnimation();
    }

  
    public void PlayerEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, originalPos.y);
        else
            image.transform.localPosition = new Vector3(500f, originalPos.y);

        image.transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        var squence = DOTween.Sequence();
        if (isPlayerUnit)
            squence.Append(image.transform.DOLocalMoveX(originalPos.x + 50f, 0.25f));
        else
            squence.Append(image.transform.DOLocalMoveX(originalPos.x - 50f, 0.25f));

        squence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.25f));
    }

    public void PlayerHitAnimation()
    {
        var squence = DOTween.Sequence();
        squence.Append(image.DOColor(Color.gray, 0.1f));
        squence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var seqence = DOTween.Sequence();
        seqence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
        seqence.Join(image.DOFade(0f, 0.5f));
    }
}