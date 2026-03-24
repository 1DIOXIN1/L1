using System;
using System.Collections.Generic;
using _Project.Develop.Runtime.Utilities.AssetsManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace _Project.Develop.Runtime.UI.Core
{
    public class ViewsFactory
    {
        private readonly ResourcesAssetsLoader _resourcesAssetsLoader;

        private readonly Dictionary<string, string> _viewIDToResourcePath = new()
        {
            {ViewIDs.Currency, "UI/Wallet/CurrencyView"},
            {ViewIDs.MainMenuScreen, "UI/MainMenu/MainMenuScreenView"},
            {ViewIDs.ResetProgressPopup, "UI/Progress/ResetProgressPopup"},
            {ViewIDs.ProgressItem, "UI/Progress/ProgressItemView"},
            {ViewIDs.GameplayScreen, "UI/Gameplay/GameplayScreenView"}
        };

        public ViewsFactory(ResourcesAssetsLoader resourcesAssetsLoader)
        {
            _resourcesAssetsLoader = resourcesAssetsLoader;
        }

        public TView Create<TView>(string viewID, Transform parent = null) where TView : MonoBehaviour, IView
        {
            if(_viewIDToResourcePath.TryGetValue(viewID, out string resourcePath) == false)
                throw new ArgumentException($"You didn`t set path for {typeof(TView)}, searched id: {viewID}");
            
            GameObject prefab = _resourcesAssetsLoader.Load<GameObject>(resourcePath);
            GameObject instance = Object.Instantiate(prefab, parent);
            TView view = instance.GetComponent<TView>();

            if (view == null)
                throw new InvalidOperationException($"Not found {typeof(TView)} component on view instance");
            
            return view;
        }

        public void Release<TView>(TView view) where TView : MonoBehaviour, IView
        {
            Object.Destroy(view.gameObject);
        }
    }
}