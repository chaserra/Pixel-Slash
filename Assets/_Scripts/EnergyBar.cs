using UnityEngine;
using UnityEngine.UI;

public class EnergyBar : MonoBehaviour
{
    private Image imageFill;
    private float _energyRatio;

    private void Start()
    {
        imageFill = GetComponent<Image>();
    }

    private void Update()
    {
        EnergyFill();
    }

    private void EnergyFill()
    {
        _energyRatio = Mathf.Clamp(_energyRatio, 0, 1);
        imageFill.fillAmount = _energyRatio;
    }

    public float EnergyRatio
    {
        get { return _energyRatio; }
        set { _energyRatio = value; }
    }
}
