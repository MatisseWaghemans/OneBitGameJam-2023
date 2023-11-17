using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    #region Editor Fields

    [SerializeField]
    private Camera _camera;

    [Header("Animation")]
    [SerializeField]
    private Animator _colorInvertAnim;

    [SerializeField]
    private string _dayTransitionParameter = "Day";

    [SerializeField]
    private string _nightTransitionParameter = "Night";

    #endregion

    #region Fields

    private int _dayTransitionHash;

    private int _nightTransitionHash;

    #endregion

    #region Initialization

    private void Start()
    {
        _dayTransitionHash = Animator.StringToHash(_dayTransitionParameter);
        _nightTransitionHash = Animator.StringToHash(_nightTransitionParameter);

        ///Test
        _colorInvertAnim.SetTrigger(_dayTransitionParameter);
    }

    #endregion

    #region Methods

    // Update is called once per frame
    void Update()
    {

    } 

    #endregion
}
