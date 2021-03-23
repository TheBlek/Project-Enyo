using UnityEngine;
public class Behaviour : ScriptableObject
{
    public float Value;
    [SerializeField] protected float _max_value;
    [SerializeField] protected float _min_value;
    public bool IsDone { get; private set; }
    [SerializeField] protected Instruction[] Instructions;
    private int _instruction_index = 0;
    public virtual bool Trigger(State state)
    {
        return false;
    }

    public void Reset()
    {
        _instruction_index = 0;
        IsDone = false;
    }

    public Instruction NextInstruction()
    {
        if (_instruction_index == Instructions.Length - 1)
            IsDone = true;
        return Instructions[_instruction_index++];
    }

    protected float SigmoidActivation(float param) // this is arranged in the way that 0 returns max_value and 1 is almost min
    {                                              // https://www.desmos.com/calculator/c8jo9hu90k?lang=ru
        return 2 * (_max_value - _min_value) / (1 + Mathf.Exp(9 * param)) + _min_value;
    }
}