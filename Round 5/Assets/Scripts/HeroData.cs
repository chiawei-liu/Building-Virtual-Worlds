using UnityEngine;

[CreateAssetMenu(fileName = "New HeroData", menuName = "Hero Data", order = 51)]
public class HeroData : ScriptableObject
{
    [SerializeField]
    public string heroName;
    [SerializeField]
    public Sprite sprite;
    [SerializeField]
    public AudioClip dashSound;
    [SerializeField]
    public Material afterimageMaterial;
    [SerializeField] public RuntimeAnimatorController animatorController;
}