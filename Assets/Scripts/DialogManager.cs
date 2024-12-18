using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogManager : MonoBehaviour
{
    public TextMeshProUGUI dialogText; // Reference to the Text element
    public Button nextButton; // Reference to the Button element (optional)
    public Component DialogPanel;
    
    private Queue<string> sentences = new Queue<string>();

    private void Start()
    {
        nextButton.onClick.AddListener(DisplayNextSentence);
    }

    public void StartDialog(string[] dialogSentences)
    {
        DialogPanel.gameObject.SetActive(true);
        sentences.Clear();

        foreach (string sentence in dialogSentences)
        {
            sentences.Enqueue(sentence);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count == 0)
        {
            EndDialog();
            return;
        }

        string sentence = sentences.Dequeue();
        StopAllCoroutines();
        StartCoroutine(TypeSentence(sentence));
    }

    IEnumerator TypeSentence(string sentence)
    {
        dialogText.text = "";
        foreach (char letter in sentence.ToCharArray())
        {
            dialogText.text += letter;
            yield return null; // Wait for the next two frames before adding the next letter
            yield return null;
        }
    }

    void EndDialog()
    {
        DialogPanel.gameObject.SetActive(false);
        dialogText.text = "";
    }
}