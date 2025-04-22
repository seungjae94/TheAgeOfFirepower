using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Mathlife.ProjectL.Gameplay.UI
{
    public class CanvasLayer : MonoBehaviour
    {
        [field: SerializeField]
        public ECanvasLayer LayerType { get; private set; }
        
        public List<TPresenter> GetAllPresenters<TPresenter>() where TPresenter : Presenter
        {
            return GetComponentsInChildren<TPresenter>().ToList();
        }
        
        public void DeactivateAllPresenters()
        {
            GetAllPresenters<Presenter>()
                .ForEach(presenter => presenter.Deactivate());;
        }
    }
}