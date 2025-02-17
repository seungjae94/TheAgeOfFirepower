using Mathlife.ProjectL.Gameplay;
using Mathlife.ProjectL.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace Mathlife.ProjectL.Editor
{
    public static class WorldScenePageNavigationMenuItems
    {
        const int wordSceneBuildIndex = 1;

        [MenuItem("Project L/World Scene/Page Navigation", isValidateFunction: true)]
        static bool isWorldScene()
        {
            return EditorSceneManager.GetActiveScene().buildIndex == wordSceneBuildIndex;
        }

        [MenuItem("Project L/World Scene/Page Navigation/Home Page", priority = 11)]
        static void GoToHome()
        {
            Home();
        }

        [MenuItem("Project L/World Scene/Page Navigation/Team Page", priority = 12)]
        static void GoToTeamPage()
        {
            Navigate<PartyPage>();
        }

        [MenuItem("Project L/World Scene/Page Navigation/Character Page", priority = 13)]
        static void GoToCharacterPage()
        {
            Navigate<CharacterPage>();
        }

        [MenuItem("Project L/World Scene/Page Navigation/Inventory Page", priority = 14)]
        static void GoToInventoryPage()
        {
            Navigate<InventoryPage>();
        }

        [MenuItem("Project L/World Scene/Page Navigation/Shop Page", priority = 15)]
        static void GoToShopPage()
        {
            Navigate<ShopPage>();
        }

        [MenuItem("Project L/World Scene/Page Navigation/Stage Selection Page", priority = 15)]
        static void GoToStageSelectionPage()
        {
            Navigate<StageSelectionPage>();
        }

        static List<Page> FindPages()
        {
            List<Page> allPages = new();

            foreach (GameObject gameObject in EditorSceneManager.GetActiveScene().GetRootGameObjects())
            {
                List<Page> pages = gameObject.transform.FindAllRecursive<Page>();
                allPages.AddRange(pages);
            }

            return allPages;
        }

        static void Home()
        {
            foreach (Page page in FindPages())
            {
                if (page is HomePage)
                    continue;

                CanvasGroup group = page.GetComponent<CanvasGroup>();
                group.Hide();
            }
        }

        static bool IsHome()
        {
            foreach (Page page in FindPages())
            {
                if (page is HomePage)
                    continue;

                CanvasGroup group = page.GetComponent<CanvasGroup>();
                if (group.alpha > 0.0f)
                    return false;
            }

            return true;
        }

        static void Navigate<T>() where T : Page
        {
            foreach (Page page in FindPages())
            {
                CanvasGroup group = page.GetComponent<CanvasGroup>();

                if (page is T)
                {
                    group.Show();
                }
                else if (page is not HomePage)
                {
                    group.Hide();
                }
            }
        }
    }
}
