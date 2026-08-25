namespace _Project.Develop.Runtime.Gameplay.Features.Main.Weapon
{
    public interface IWeapon
    {
        int Ammo { get; }
        int MagazineSize { get; }
        bool CanShoot { get; }
        bool IsReloading { get; }
        bool IsAutomatic { get; }

        void Tick(float deltaTime);
        void Shoot();
        void Reload();
    }
}
