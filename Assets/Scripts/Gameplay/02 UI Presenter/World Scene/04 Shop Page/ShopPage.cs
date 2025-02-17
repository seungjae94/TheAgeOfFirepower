using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mathlife.ProjectL.Gameplay
{
    public class ShopPage : Page
    {
        public override EPageId pageId => EPageId.ShopPage;

        public override void Initialize()
        {
            InitializeChildren();
            Close();
        }

        protected override void InitializeChildren()
        {
            
        }
    }
}
