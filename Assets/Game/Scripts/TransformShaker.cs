using UnityEngine;

public class TransformShaker : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    [Range(0f, 360)]
    private float m_angle = 0f;

    [SerializeField] private float m_strength = 1f;
    [SerializeField] private float m_frequency = 25f;
    [SerializeField] private float m_duration = 0.2f;

    [Space][SerializeField] private bool m_waitingUntilFinished = false;

    #region INTERNAL

    private Vector3 _currPos;

    float _currTime = 0f;
    private bool _inShake = false;
    private float _currShakeMultiplier = 1f;
    private Vector2 _shakeNormal;

    #endregion

    /// <summary>
    /// Shake with parameter is being config
    /// </summary>
    public void Shake() => Shake(m_angle, m_strength, m_frequency, m_duration);

    /// <summary>
    /// Shake with parameter with input new values.
    /// </summary>
    /// <param name="angle">Angle of the shake in degree</param>
    /// <param name="strength">Strength of th shake in meter</param>
    /// <param name="frequency"></param>
    /// <param name="duration">Duration of the shake will be performed</param>
    public void Shake(float angle, float strength = 1f, float frequency = 50f, float duration = 0.5f)
    {
        if (m_waitingUntilFinished && _inShake)
            return;

        // Resetting the position back to place first when still in shake
        if (_inShake)
            transform.position = _currPos;

        _currPos = transform.position;

        // Generate the angle normal for the shakes.
        float _rad = Mathf.Deg2Rad * m_angle;
        _shakeNormal = new Vector2(Mathf.Cos(_rad), Mathf.Sin(_rad));

        _currTime = 0f;
        m_duration = duration;
        m_frequency = frequency;
        m_strength = strength;

        _currShakeMultiplier = 0f;

        _inShake = true;

    }

    private void Update()
    {
        if (_inShake && _shakeNormal != Vector2.zero)
        {
            if (_currTime <= 1f)
            {
                _currTime += Time.deltaTime;
                _currShakeMultiplier = Mathf.Lerp(1f, 0f, _currTime / m_duration);
            }
            else
            {
                _currShakeMultiplier = 0f;
                _inShake = false;
            }

            Vector3 _shakePos = _shakeNormal *
                                (((Mathf.Sin(Time.time * m_frequency) * m_strength) * _currShakeMultiplier));
            transform.position = _currPos + _shakePos;
        }
    }
}
