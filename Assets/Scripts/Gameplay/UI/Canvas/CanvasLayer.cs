using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class CanvasLayer : MonoBehaviour
    {
        public List<TPresenter> GetAllPresenters<TPresenter>() where TPresenter : Presenter
        {
            return GetComponentsInChildren<TPresenter>().ToList();
        }
        
        public void CloseAllPresenters()
        {
            GetAllPresenters<Presenter>()
                .ForEach(presenter => presenter.Close());;
        }
    }
}