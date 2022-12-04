using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Quiz : MonoBehaviour
{
    
    [Header("Questions")]
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    QuestionSO currentQuestion;

    [Header("Answers")]
    [SerializeField] GameObject[] answerButtons;
    int correctAnswerIndex;
    bool hasAnsweredEarly = true;

    [Header("Button Colors")]
    [SerializeField] Sprite defaultAnswerSprite;
    [SerializeField] Sprite correctAnswerSprite;
    Image answerButtonImage;

    [Header("Timer")]
    [SerializeField] Image timerImage;
    Timer timer;

    [Header("Scoring")]
    [SerializeField] TextMeshProUGUI scoreText;
    ScoreKeeper scoreKeeper;

    [Header("ProgressBar")]
    [SerializeField] Slider progressBar;

    public bool isComplete;

    void Awake() {
        timer = FindObjectOfType<Timer>();
        scoreKeeper = FindObjectOfType<ScoreKeeper>();
        progressBar.maxValue = questions.Count;
        progressBar.value = 0;
    }

    void Update() {
        UpdateTimerFillAmmount();

        if(timer.loadNextQuestion) {
            if(questions.Count <= 0) {
                isComplete = true;
                return;
            }

            hasAnsweredEarly = false;
            GetNextQuestion();
            timer.loadNextQuestion = false;
        } else if(!hasAnsweredEarly && !timer.isAnsweringQuestion) {
            DisplayAnswer(-1);
            SetButtonsState(false);
        }
    }

    void UpdateTimerFillAmmount() {
        timerImage.fillAmount = timer.fillFraction;
    }

    void GetNextQuestion() {
        GetRandomQuestion();
        DisplayQuestion();
        SetDefaultButtonsSprite();
        SetButtonsState(true);
        progressBar.value++;
        scoreKeeper.IncrementQuestionsSeen();
    }

    void GetRandomQuestion() {
        int index = Random.Range(0, questions.Count);
        currentQuestion = questions[index];

        if(questions.Contains(currentQuestion)) {
            questions.Remove(currentQuestion);
        }
    }

    void DisplayQuestion() {
        questionText.text = currentQuestion.GetQuestion();

        for (int i = 0; i < answerButtons.Length; i++) {
            TextMeshProUGUI answerText = answerButtons[i].GetComponentInChildren<TextMeshProUGUI>();
            answerText.text = currentQuestion.GetAnswer(i);
        }
    }

    void SetButtonsState(bool state) {
        for (int i = 0; i < answerButtons.Length; i++) {
            Button answerButton = answerButtons[i].GetComponent<Button>();
            answerButton.interactable = state;
        }
    }

    void SetDefaultButtonsSprite() {
        for (int i = 0; i < answerButtons.Length; i++) {
            answerButtonImage = answerButtons[i].GetComponent<Image>();
            answerButtonImage.sprite = defaultAnswerSprite;
        }
    }


    void DisplayAnswer(int index) {
        if (index == currentQuestion.GetCorrectAnswerIndex()) {
            questionText.text = "Correct Answer :)";
            answerButtonImage = answerButtons[index].GetComponent<Image>();
            answerButtonImage.sprite = correctAnswerSprite;
            scoreKeeper.IncrementCorrectAnswers();
        } else {
            correctAnswerIndex = currentQuestion.GetCorrectAnswerIndex();
            questionText.text = "Sorry :( The correct answer was:\n " + currentQuestion.GetAnswer(correctAnswerIndex);
            answerButtonImage = answerButtons[correctAnswerIndex].GetComponent<Image>();
            answerButtonImage.sprite = correctAnswerSprite;
        }
        scoreText.text = "Score: " + scoreKeeper.CalculateScore() + "%";
    }

    public void OnAnswerSelected(int index) {
        hasAnsweredEarly = true;
        DisplayAnswer(index);
        SetButtonsState(false);
        timer.CancelTimer();
    }

}
