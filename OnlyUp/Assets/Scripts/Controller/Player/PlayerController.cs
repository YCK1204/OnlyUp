using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Player.Controller
{
    public class PlayerController : MonoBehaviour
    {
        PlayerMoveController _mc;
        PlayerStatController _stat;
        void Start()
        {
            _mc = GetComponent<PlayerMoveController>();
            _stat = GetComponent<PlayerStatController>();
            DontDestroyOnLoad(gameObject);
        }
        public void UsePotion(PotionData data)
        {
            var heal = data.HealAmount / 100f;
            var duration = data.Duration;
            var type = data.Type;
            var interval = data.Interval;

            StartCoroutine(CoUsePotion(heal, interval, duration, type));
        }
        IEnumerator CoUsePotion(float healAmount, float interval, float duration, PotionType type)
        {
            int tickCount = Mathf.Max(1, Mathf.RoundToInt(duration / interval));
            float healPerTick = healAmount / tickCount;
            float healed = 0f;

            for (int tick = 0; tick < tickCount; tick++)
            {
                float heal = (tick == tickCount - 1) ? healAmount - healed : healPerTick;
                switch (type)
                {
                    case PotionType.Health:
                        _stat.HP += heal;
                        break;
                    case PotionType.Stamina:
                        _stat.Stamina += heal;
                        break;
                }
                healed += heal;
                yield return new WaitForSeconds(interval);
            }
        }
    }
}