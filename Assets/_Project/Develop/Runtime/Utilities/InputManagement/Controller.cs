namespace _Project.Develop.Runtime.Utilities.InputManagement
{
    public abstract class Controller
    {
        private bool _isEnabled;

        public virtual void Enable() => _isEnabled = true;

        public virtual void Disable() => _isEnabled = false;
    
        public void Update(float deltaTime)
        {
            if (_isEnabled == false)
                return;

            UpdateLogic(deltaTime);
        }

        protected abstract void UpdateLogic(float deltaTime);
    }
}