namespace _Project.Develop.Runtime.Gameplay.Features.Main.Characters
{
    public interface IDamageble
    {
        DamageAffiliation Affiliation { get; }

        void TakeDamage(int damage);
    }
}
