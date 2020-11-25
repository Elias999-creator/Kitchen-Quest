using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleDialogBox : MonoBehaviour
{
    [SerializeField] int letterPerSecond;
    [SerializeField] Color highlightedColor;
    [SerializeField] Text dialogText;
    [SerializeField] GameObject actionSelector;
    [SerializeField] GameObject moveSelector;
    [SerializeField] GameObject moveDetails;
    [SerializeField] GameObject enemySelector;

    [SerializeField] List<Text> actionTexts;
    [SerializeField] List<Text> moveTexts;
    [SerializeField] List<Text> enemyTexts;

    [SerializeField] Text ppText;
    [SerializeField] Text typeText;

    public void SetDialog(string dialog)
    {
        dialogText.text = dialog;
    }

    public IEnumerator TypeDialog(string dialog)
    {
        dialogText.text = "";
        foreach (var letter in dialog.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(1f / letterPerSecond);
        }

        yield return new WaitForSeconds(1f);
    }

    public void EnableDialogText(bool enabled)
    {
        dialogText.enabled = enabled;
    }

    public void EnableActionSelector(bool enabled)
    {
        actionSelector.SetActive(enabled);
    }

    public void EnableMoveSelector(bool enabled)
    {
        moveSelector.SetActive(enabled);
        moveDetails.SetActive(enabled);
    }

    public void EnableEnemySelector(bool enabled)
    {
        enemySelector.SetActive(enabled);
    }


    public void UpdateActionSelection(int selectedAction)
    {
        for (int i = 0; i < actionTexts.Count; ++i)
        {
            if (i == selectedAction)
                actionTexts[i].color = highlightedColor;
            else
                actionTexts[i].color = Color.black;
        }
    }

    public void UpdateMoveSelection(int selectionMove, Move move)
    {
        for (int i = 0; i < moveTexts.Count; ++i)
        {
            if (i == selectionMove)
                moveTexts[i].color = highlightedColor;
            else
                moveTexts[i].color = Color.black;
        }

        ppText.text = $"PP {move.PP}/{move.Base.PP}";
        typeText.text = move.Base.Type.ToString();
    }


    public void UpdateEnemySelection(int selectedEnemy)
    {
        for (int i = 0; i < enemyTexts.Count; ++i)
        {
            if (i == selectedEnemy)
                enemyTexts[i].color = highlightedColor;
            else
                enemyTexts[i].color = Color.black;
        }
    }

    public void SetMovesNames(List<Move> moves)
    {
        for (int i = 0; i < 4; i++)
        {
            if (i < moves.Count)
                moveTexts[i].text = moves[i].Base.Name;
            else
                moveTexts[i].text = "    -";
        }
    }

    public void SetEnemyNames(List<BattleUnit> enemyUnits)
    {
        for (int i = 0; i < 2; i++)
        {
            if (i < enemyUnits.Count)
                enemyTexts[i].text = enemyUnits[i].Pokemon.Base.Name;
            else
                enemyTexts[i].text = "";
        }
    }
    
}