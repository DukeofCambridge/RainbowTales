using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Rainbow.Dialogue;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public GameObject dialogueBox;
    public TextMeshProUGUI dailogueText;
    public Image faceRight, faceLeft;
    public TextMeshProUGUI nameRight, nameLeft;
    public GameObject continueBox;

    private void Awake()
    {
        continueBox.SetActive(false);
    }

    private void OnEnable()
    {
        EventHandler.ShowDialogueEvent += OnShowDailogueEvent;
    }

    private void OnDisable()
    {
        EventHandler.ShowDialogueEvent -= OnShowDailogueEvent;
    }

    private void OnShowDailogueEvent(DialoguePiece piece)
    {
        StartCoroutine(ShowDialogue(piece));
    }

    private IEnumerator ShowDialogue(DialoguePiece piece)
    {
        if (piece != null)
        {
            piece.isDone = false;

            dialogueBox.SetActive(true);
            continueBox.SetActive(false);

            dailogueText.text = string.Empty;

            if (piece.name != string.Empty)
            {
                if (piece.onLeft)
                {
                    faceRight.gameObject.SetActive(false);
                    faceLeft.gameObject.SetActive(true);
                    faceLeft.sprite = piece.faceImage;
                    nameLeft.text = piece.name;
                }
                else
                {
                    faceRight.gameObject.SetActive(true);
                    faceLeft.gameObject.SetActive(false);
                    faceRight.sprite = piece.faceImage;
                    nameRight.text = piece.name;
                }
            }
            else
            {
                faceLeft.gameObject.SetActive(false);
                faceRight.gameObject.SetActive(false);
                nameLeft.gameObject.SetActive(false);
                nameRight.gameObject.SetActive(false);
            }
            yield return DOTween.To(() => string.Empty, value => dailogueText.text = value, piece.dialogueText, 1f).SetEase(Ease.Linear);
            yield return new WaitForSeconds(1f);
            piece.isDone = true;
            //Debug.Log("isDone!");

            if (piece.hasToPause && piece.isDone)
                continueBox.SetActive(true);
        }
        else
        {
            dialogueBox.SetActive(false);
            faceLeft.gameObject.SetActive(false);
            faceRight.gameObject.SetActive(false);
            continueBox.gameObject.SetActive(false);
            yield break;
        }
    }
}
