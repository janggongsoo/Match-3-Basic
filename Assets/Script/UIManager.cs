using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager> {

    public Text txtCombo;
    public Text txtScore;

    public void SetComboText(int _combo)
    {
        txtCombo.text = _combo.ToString();
    }
    public void SetScoreText(int _score)
    {
        txtScore.text = _score.ToString();
    }
}
