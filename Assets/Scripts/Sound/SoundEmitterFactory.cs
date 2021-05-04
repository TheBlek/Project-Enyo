using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Audio/SoundEmitterFactory")]
public class SoundEmitterFactory : ScriptableObject
{
    [SerializeField] private SoundEmitter _prefab;

    public SoundEmitter Create()
    {
        return Instantiate(_prefab);
    }
}