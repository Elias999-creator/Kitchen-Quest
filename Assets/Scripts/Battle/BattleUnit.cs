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

    Animator animator;
    Vector3 originalPos;
    Color originalColor;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        originalPos = transform.localPosition;
    }

    public void Setup()
    {
       Pokemon = new Pokemon(_base, level);
        if (!isPlayerUnit && animator)
            animator.runtimeAnimatorController = Pokemon.Base.AnimationController;

        PlayerEnterAnimation();
    }

  
    public void PlayerEnterAnimation()
    {
        if (!isPlayerUnit)
            transform.localPosition = new Vector3(500f, originalPos.y);

        transform.DOLocalMoveX(originalPos.x, 1f);
    }

    public void PlayAttackAnimation()
    {
        if(animator)
            animator.SetTrigger("doPhysicalAttack");
    }

    public void PlayerHitAnimation()
    {
    }

    public void PlayFaintAnimation()
    {
        var seqence = DOTween.Sequence();
        seqence.Append(transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
    }
}