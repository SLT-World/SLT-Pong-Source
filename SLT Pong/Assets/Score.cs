using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] Text ScoreUI;

    string ScoreTextTemplate;

    int _Score;

    private void Start()
    {
        ScoreTextTemplate = ScoreUI.text;
        _Score = 0;
        ScoreUI.text = string.Format(ScoreTextTemplate, _Score);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Ball _Ball = collision.gameObject.GetComponent<Ball>();
        if (_Ball)
        {
            if (_Score < 100)
            {
                _Score += 10;
                ScoreUI.text = string.Format(ScoreTextTemplate, _Score);
                if (_Score == 20 || _Score == 40 || _Score == 60 || _Score == 80)
                    Instantiate(GameManager.Instance.BallPrefab);
            }
            _Ball.Reset();
        }
    }
}
