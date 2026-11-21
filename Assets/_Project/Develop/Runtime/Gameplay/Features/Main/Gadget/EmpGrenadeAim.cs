using _Project.Develop.Runtime.Utilities.InputManagement;

namespace _Project.Develop.Runtime.Gameplay.Features.Main.Gadget
{
    public sealed class EmpGrenadeAim
    {
        private readonly EmpGrenade _emp;
        private bool _aiming;

        public EmpGrenadeAim(EmpGrenade emp) => _emp = emp;

        public bool TryThrow(IInputService input)
        {
            if (input.EmpPressed)
                _aiming = true;

            if (!_aiming)
                return false;

            if (input.EmpReleased)
            {
                Cancel();
                _emp.Throw();
                return true;
            }

            if (input.EmpHeld)
                _emp.ShowTrajectory();
            else
                Cancel();

            return false;
        }

        public void Cancel()
        {
            if (!_aiming)
                return;

            _aiming = false;
            _emp.HideTrajectory();
        }
    }
}
