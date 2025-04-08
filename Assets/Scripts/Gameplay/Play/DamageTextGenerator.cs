using Mathlife.ProjectL.Gameplay.ObjectBase;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.Play
{
    public class DamageTextGenerator : MonoSingleton<DamageTextGenerator>
    {
        protected override SingletonLifeTime LifeTime => SingletonLifeTime.Scene;

        [SerializeField]
        private GameObject damageTextPrefab;

        public void Generate(ArtyController owner, int damage, bool isHeal)
        {
            var inst = Instantiate(damageTextPrefab, transform);
            var damageText = inst.GetComponent<DamageText>();
            damageText.Setup(owner, damage, isHeal);
        }
    }
}