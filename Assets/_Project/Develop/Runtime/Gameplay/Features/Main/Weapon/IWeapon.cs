namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public interface IWeapon
    {
        public int Ammo { get; }
        public float ShootSpeed { get; }
        public float ReloadSpeed { get; }
        public int Damage { get; }

        public void Shoot();
    }
}