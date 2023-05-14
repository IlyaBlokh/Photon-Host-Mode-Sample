using TMPro;
using UnityEngine;

public class PlayerHud : MonoBehaviour
{
    [SerializeField] private TMP_Text _message;

    public void ShowMessage(string text)
    {
        _message.text += text + '\n';
    }
}