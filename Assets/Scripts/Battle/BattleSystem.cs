using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum BattleState { Start, ActionSelection, MoveSelection, PerformMove, Busy, EnemySelection }

public class BattleSystem : MonoBehaviour
{
    [SerializeField] BattleUnit playerUnit;
    [SerializeField] BattleUnit enemyUnit;
    [SerializeField] BattleUnit enemyUnit2;
    [SerializeField] BattleHud playerHud;
    [SerializeField] BattleHud enemyHud;
    [SerializeField] BattleHud enemyHud2;
    [SerializeField] BattleDialogBox dialogBox;

    BattleState state;
    int currentAction;
    int currentMove;
    int currentEnemy;

    private void Start()
    {
        StartCoroutine(SetupBattle());
    }

    public IEnumerator SetupBattle()
    {
        List<BattleUnit> enemyUnits = new List<BattleUnit>();
        enemyUnits.Add(enemyUnit);
        enemyUnits.Add(enemyUnit2);
        playerUnit.Setup();
        enemyUnit.Setup();
        playerHud.SetData(playerUnit.Pokemon);
        enemyHud.SetData(enemyUnit.Pokemon);
        if (enemyUnit2 && enemyHud2)
        {
            enemyUnit2.Setup();
            enemyHud2.SetData(enemyUnit2.Pokemon);
        }

        dialogBox.SetMovesNames(playerUnit.Pokemon.Moves);

        dialogBox.SetEnemyNames(enemyUnits);

        yield return dialogBox.TypeDialog($"A rotten {enemyUnit.Pokemon.Base.Name} appeared.");

        ActionSelection();
    }

    void ActionSelection()
    {
        state = BattleState.ActionSelection;
        StartCoroutine(dialogBox.TypeDialog("What will you do?"));
        dialogBox.EnableActionSelector(true);
    }

    void MoveSelection()
    {
        state = BattleState.MoveSelection;
        dialogBox.EnableActionSelector(false);
        dialogBox.EnableDialogText(false);
        dialogBox.EnableMoveSelector(true);
    }
    void EnemySelection()
    {
        state = BattleState.EnemySelection;
        dialogBox.EnableMoveSelector(false);
        dialogBox.EnableEnemySelector(true);
    }

    IEnumerator PlayerMove()
    {
        state = BattleState.PerformMove;

        var move = playerUnit.Pokemon.Moves[currentMove];
        move.PP--;
        yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} used {move.Base.Name}");

        playerUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        enemyUnit.PlayerHitAnimation();
        var damageDetails = enemyUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetail(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{enemyUnit.Pokemon.Base.Name} Fainted");
            enemyUnit.PlayFaintAnimation();
            if (enemyUnit2)
            {
                if (enemyUnit.Pokemon.HP == 0 && enemyUnit2.Pokemon.HP == 0)
                    StartCoroutine(GoBack());
            }
            if (enemyUnit2 == false)
                StartCoroutine(GoBack());
        }
        else
        {
            StartCoroutine(EnemyMove(enemyUnit));
        }
    }

    IEnumerator EnemyMove(BattleUnit battleUnit)
    {
        state = BattleState.PerformMove;

        var move = battleUnit.Pokemon.GetRandomMove();
        move.PP--;
        yield return dialogBox.TypeDialog($"{battleUnit.Pokemon.Base.Name} used {move.Base.Name}");

        battleUnit.PlayAttackAnimation(move.Base.AnimationCategory);
        yield return new WaitForSeconds(1f);

        playerUnit.PlayerHitAnimation();
        var damageDetails = playerUnit.Pokemon.TakeDamage(move, playerUnit.Pokemon);
        yield return playerHud.UpdateHP();
        yield return ShowDamageDetail(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{playerUnit.Pokemon.Base.Name} Fainted");
            playerUnit.PlayFaintAnimation();

            StartCoroutine(GoBack());
        }
        else
        {
            if (!enemyUnit2)
                ActionSelection();

            if (enemyUnit2 && battleUnit == enemyUnit)
                StartCoroutine(EnemyMove(enemyUnit2));

            if (battleUnit == enemyUnit2)
                ActionSelection();
        }
    }

    IEnumerator RunMove(BattleUnit sourceUnit, BattleUnit targetUnit, Move move)
    {
        move.PP--;
        yield return dialogBox.TypeDialog($"{sourceUnit.Pokemon.Base.Name} used {move.Base.Name}");

        sourceUnit.PlayAttackAnimation();
        yield return new WaitForSeconds(1f);

        targetUnit.PlayerHitAnimation();
        var damageDetails = targetUnit.Pokemon.TakeDamage(move, sourceUnit.Pokemon);
        yield return enemyHud.UpdateHP();
        yield return ShowDamageDetail(damageDetails);

        if (damageDetails.Fainted)
        {
            yield return dialogBox.TypeDialog($"{targetUnit.Pokemon.Base.Name} Fainted");
            targetUnit.PlayFaintAnimation();
        }
    }

    IEnumerator ShowDamageDetail(DamageDetails damageDetails)
    {
        if (damageDetails.Critical > 1f)
            yield return dialogBox.TypeDialog("A critical hit!");

        if (damageDetails.TypeEffectiveness > 1)
            yield return dialogBox.TypeDialog("It's super effective!");
        else if (damageDetails.TypeEffectiveness < 1f)
            yield return dialogBox.TypeDialog("It's not very effective!");
    }

    private void Update()
    {
        if (state == BattleState.ActionSelection)
        {
            HandleActionSelection();
        }
        else if (state == BattleState.MoveSelection)
        {
            HandleMoveSelection();
        }
        else if (state == BattleState.EnemySelection)
        {
            HandleEnemySelection();
        }
    }

    void HandleActionSelection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentAction < 1)
                ++currentAction;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentAction > 0)
                --currentAction;
        }

        dialogBox.UpdateActionSelection(currentAction);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentAction == 0)
            {
                // Fight
                MoveSelection();
            }
            else if (currentAction == 1)
            {
                // Run
                SceneManager.LoadScene(0);
            }
        }
    }

    void HandleMoveSelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 1)
                ++currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentMove > 0)
                --currentMove;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (currentMove < playerUnit.Pokemon.Moves.Count - 2)
                currentMove += 2;
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (currentMove > 1)
                currentMove -= 2;
        }

        dialogBox.UpdateMoveSelection(currentMove, playerUnit.Pokemon.Moves[currentMove]);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            EnemySelection();
        }
    }

    void HandleEnemySelection()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (currentEnemy < playerUnit.Pokemon.Moves.Count - 1)
                ++currentEnemy;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (currentEnemy > 0)
                --currentEnemy;
        }

        dialogBox.UpdateEnemySelection(currentEnemy);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dialogBox.EnableEnemySelector(false);
            dialogBox.EnableDialogText(true);   
            StartCoroutine(PlayerMove());
        }
    }


    IEnumerator GoBack()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(0);
    }
}
