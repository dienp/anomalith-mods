using System.Reflection;
using UnityEngine;

namespace AnomalithUpgradeTracker
{
    internal static class OvGainNotice
    {
        private const BindingFlags Fields = BindingFlags.Instance | BindingFlags.NonPublic;
        private static readonly FieldInfo GainCoroutine = typeof(MoneyUI).GetField("AnimCor", Fields);
        private static readonly FieldInfo GainStep = typeof(MoneyUI).GetField("step", Fields);
        private static readonly FieldInfo Animation = typeof(MoneyUI).GetField("anim", Fields);
        private static readonly int FadeOut = Animator.StringToHash("FadeOut");

        internal static bool IsActive(MoneyUI money)
        {
            if (money == null || !money.isActiveAndEnabled)
                return false;

            if (GainCoroutine?.GetValue(money) != null ||
                (GainStep?.GetValue(money) is int step && step > 0))
                return true;

            // The game resets the gain step before the notice finishes fading out.
            var animator = Animation?.GetValue(money) as Animator;
            if (animator == null || !animator.isActiveAndEnabled)
                return false;

            var state = animator.GetCurrentAnimatorStateInfo(0);
            return state.shortNameHash == FadeOut && state.normalizedTime < 1f;
        }
    }
}
