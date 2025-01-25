using UnityEngine;

public class AnimatorDebugUI : MonoBehaviour
{
    private Animator _animator;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.updateMode = AnimatorUpdateMode.UnscaledTime; // Temporarily set to UnscaledTime
        _animator.updateMode = AnimatorUpdateMode.Normal; // Immediately switch back to Normal
    }

}
