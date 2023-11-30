using UnityEngine;
using TMPro;

public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _hpText;

    private void LateUpdate()
    {
        _hpText.text = "HP : " + PlayerLinks.instance.PlayerCombat.HP;
    }
}
