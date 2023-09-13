using Animal;
using Quest;
using UnityEngine;

public class AnimalSelector : MonoBehaviour
{
    [SerializeField] private AnimalType[] availableAnimalTypes;
    [SerializeField] private GameObject[] _animalsPrefubs;
    [SerializeField] private Transform _animalContainer;
    
    private int _indexAnimalType;

    // create random animal and set it to config
    public void SetRandomAnimal(QuestLevelConfig _config, Vector3 _offSet)
    {   
        GetRandomIndex();
        _config.animalType = GetRandomIndex();
        Instantiate(_animalsPrefubs[_indexAnimalType], _animalContainer);
        Transform childObject = _animalContainer.GetChild(0);
            childObject.position += _offSet;
        Debug.Log("_config.animalType " + _config.animalType);
    }

    // create type of animal from config
    public void GetAnimalFromConfig(QuestLevelConfig config, Vector3 _offSet)
    {
        Instantiate(_animalsPrefubs[config.animalType], _animalContainer);
        Transform childObject = _animalContainer.GetChild(0);
            childObject.position += _offSet;
    }

    // create random index for animal type
    private int GetRandomIndex()
        => _indexAnimalType = Random.Range(0, availableAnimalTypes.Length);
}
