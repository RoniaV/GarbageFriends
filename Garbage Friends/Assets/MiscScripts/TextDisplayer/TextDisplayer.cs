using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class TextDisplayer : MonoBehaviour
{
    public event Action OnEndDisplaying;

    //Public fields
    public bool isTyping { get; private set; }
    public bool isOpen { get; private set; }

    //Properties
    [Header("GUI Elements")]
    [SerializeField] Text nameText;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] GameObject continueButton;
    [SerializeField] Animator GUIAnimator;
    [Header("Display Settings")]
    [SerializeField] float charactersPerSecond = 10f;
    [SerializeField] float delayBetweenSentences = 0f;
    [SerializeField] AudioClip[] typingSounds;
    [SerializeField] string openTriggerName = "Open";
    [SerializeField] string closeTriggerName = "Close";
    [Header("Next Sentence Settings")]
    [Tooltip("Use Continue button to display next sentence")]
    [SerializeField] bool useContinueButton = true;
    [SerializeField] bool hideContinueButtonWhileTyping = true;
    [Tooltip("Clear the text before displaying the next sentence")]
    [SerializeField] bool clearTextBeforeDisplay = true;

    //Components
    private AudioSource audioSource;

    //Private fields
    private TextToDisplay actualText;
    private int sentenceIndex;
    private char[] charSentence;
    private int sentenceCharIndex;

    void Awake()
    {
        GUIAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    public void DisplayText(TextToDisplay textToDisplay)
    {
        Debug.Log("Displaying text: " + textToDisplay.name);

        actualText = textToDisplay;

        //If GUI is not open, open the GUI
        if (!isOpen)
            OpenGUI();

        if (isTyping)
            EndTyping();

        CancelInvoke("DisplayNextSentence");

        //If GUI has a name text, sets the name text
        if (nameText != null)
            nameText.text = textToDisplay.name;

        //Clear text and set the sentence index to zero
        text.text = "";
        sentenceIndex = 0;

        DisplayNextSentence();
    }

    public void CloseGUI()
    {
        if (isOpen)
        {
            Debug.Log("Closing GUI");
            isOpen = false;
            GUIAnimator.SetTrigger(closeTriggerName);

            if (isTyping)
                EndTyping();
        }
    }

    public void DisplayNextSentence()
    {
        if(sentenceIndex >= actualText.Sentences.Length)
        {
            Debug.Log("Last sentence displayed");

            if (OnEndDisplaying != null)
                OnEndDisplaying();

            CloseGUI();
            return;
        }

        if(isTyping)
        {
            text.text = "";
            text.text = actualText.Sentences[sentenceIndex];
        }

        Debug.Log("Displaying next sentence");

        charSentence = actualText.Sentences[sentenceIndex].ToCharArray();
        sentenceIndex++;

        //If clearTextBeforeDisplay is true, clear the text
        if (clearTextBeforeDisplay)
            text.text = "";

        continueButton?.SetActive(!hideContinueButtonWhileTyping);

        //Set the sentenceCharIndex to zero
        sentenceCharIndex = 0;

        //Start typing the sentence
        isTyping = true;
        InvokeRepeating("TypeChar", 0f, 1f / charactersPerSecond);
    }

    private void OpenGUI()
    {
        if(!isOpen)
        {
            Debug.Log("Opening GUI");
            isOpen = true;
            GUIAnimator.SetTrigger(openTriggerName);
        }
    }

    private void TypeChar()
    {
        //Si el indice es mayor que la longitud de la frase, deja de imprimir
        if (sentenceCharIndex >= charSentence.Length)
        {
            Debug.Log("Sentence finished");
            EndTyping();

            //Si se usa el boton de continuar, se activa...
            if (continueButton != null && useContinueButton)
                continueButton.SetActive(true);
            //...si no, se pasa a la siguiente frase
            else
                Invoke("DisplayNextSentence", delayBetweenSentences);

            return;
        }

        //Imprime el caracter y pasa al siguiente
        text.text += charSentence[sentenceCharIndex];

        //Reproduce de manera aleatoria un sonido
        if (!audioSource.isPlaying)
            audioSource.PlayOneShot(typingSounds[UnityEngine.Random.Range(0, typingSounds.Length)]);

        sentenceCharIndex++;
    }

    private void EndTyping()
    {
        Debug.Log("End typing");
        isTyping = false;
        continueButton?.SetActive(true);
        sentenceCharIndex = 0;
        CancelInvoke("TypeChar");
        CancelInvoke("DisplayNextSentence");
    }
}
