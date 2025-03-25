using System.Linq;
using System;
using UnityEngine;

[Serializable]
public class AnimationParameter
{
    public enum ParameterType { Int, Bool, Float }
    public ParameterType type;

    public string name;

    public int intValue;
    public bool boolValue;
    public float floatValue;

    public void Apply(Animator animator)
    {
        switch (type)
        {
            case ParameterType.Int:
                animator.SetInteger(name, intValue);
                break;
            case ParameterType.Bool:
                animator.SetBool(name, boolValue);
                break;
            case ParameterType.Float:
                animator.SetFloat(name, floatValue);
                break;
        }
    }
}

public class AnimationSetter : MonoBehaviour
{
    [SerializeField] private AnimationParameter[] parameters;
    private Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
        foreach (var parameter in parameters)
        {
            parameter.Apply(animator);
        }
    }
}
