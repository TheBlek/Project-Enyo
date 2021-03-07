using UnityEngine;
public class Behaviour : ScriptableObject
{
    public float Value;
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

}