using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SocialApp;
using System.Linq;

namespace SocialApp
{
    public class GroupUIController : MonoBehaviour
    {
        [SerializeField]
        private Image FriendBack = default;
      /*  [SerializeField]
        private Image RequestBack = default;
        [SerializeField]
        private Image PendingBack = default;*/
        [SerializeField]
        private Image SearchBack = default;
        [SerializeField]
        private Image SearchIcon = default;
        [SerializeField]
        private InputField SearchInput = default;
        [SerializeField]
        private GameObject SearchPanel = default;
        [SerializeField]
        private RectTransform ScrollViewRect = default;
        [SerializeField]
        private float DefaultScrollHeight = default;
        [SerializeField]
        private float SearchScrollHeight = default;
        [SerializeField]
        private Color ActiveColor = default;
        [SerializeField]
        private Color DefaultColor = default;
        [SerializeField]
        private GroupsDataLoader grouploader = default;
        /*[SerializeField]
        private Text FriendsCountLabel = default;
        [SerializeField]
        private Text RequestCountLabel = default;
        [SerializeField]
        private Text PendingCountLabel = default;*/

        public GroupTabState CurrentTabState;


/*
        public static bool divide(Stack<int> stack,int num)
        {

            bool isdivider = true;
            bool isalldivider = false;


            Stack<int> s1 = new Stack<int>();
            Stack<int> S2 = new Stack<int>();





            while (!stack.IsEmpty())
            {

                int x = stack.Pop();

                if(!(x<=num) || num % x != 0)
                {
                    isdivider = false;
                }
                s1.Push(x);
                

            }
            int i = 1;

            while (i <= num)
            {
                if (i <= num && num % i == 0)
                {
                    bool isIn = false;
                    while (!s1.IsEmpty())
                    {
                        int x = s1.Pop();
                        if (x == i)
                        {
                            isIn = true;

                        }
                        S2.push(x);
                    
                    }


                    while (!S2.isEmpty())
                    {
                        int j = S2, pop();
                        s1.Push(j);
                        
                    }


               
                    if(isIn == false)
                    {

                        isalldivider = false;
                    }
                
                
                }

            }

            return isalldivider&&isdivider;
        }*/



        public void OnGroups()
        {
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            CurrentTabState = GroupTabState.group;
            ToDefaultState();
           // FriendBack.color = ActiveColor;
            grouploader.ResetLoader();
            grouploader.AutoLoadContent(true);
            ScrollViewRect.offsetMax = new Vector2(ScrollViewRect.offsetMax.x, -DefaultScrollHeight);
        }

      /*  public void OnRequest()
        {
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            CurrentTabState = FriendsTabState.Requested;
            ToDefaultState();
            RequestBack.color = ActiveColor;
            UserLoader.ResetLoader();
            UserLoader.AutoLoadContent(true);
            ScrollViewRect.offsetMax = new Vector2(ScrollViewRect.offsetMax.x, -DefaultScrollHeight);
        }

        public void OnPending()
        {
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            CurrentTabState = FriendsTabState.Pending;
            ToDefaultState();
            PendingBack.color = ActiveColor;
            UserLoader.ResetLoader();
            UserLoader.AutoLoadContent(true);
            ScrollViewRect.offsetMax = new Vector2(ScrollViewRect.offsetMax.x, -DefaultScrollHeight);
        }*/

        public void OnSearch()
        {
            AppManager.DEVICE_CONTROLLER.UnloadAssets();
            CurrentTabState = GroupTabState.Search;
            ToDefaultState();
            SearchInput.text = string.Empty;
            SearchBack.color = ActiveColor;
            SearchIcon.color = DefaultColor;
            SearchPanel.SetActive(true);
            grouploader.ResetLoader();
            ScrollViewRect.offsetMax = new Vector2(ScrollViewRect.offsetMax.x, -SearchScrollHeight);
        }

        private void ToDefaultState()
        {
            FriendBack.color = DefaultColor;
            //  RequestBack.color = DefaultColor;
            //  PendingBack.color = DefaultColor;
            SearchBack.color = Color.white;
            SearchIcon.color = Color.yellow;
            SearchPanel.SetActive(false);
        }

        public GroupsDataLoader GetGroupsDataLoader()
        {
            return grouploader;
        }

      /*  public void UpdateFriendsCount(int _count)
        {
            FriendsCountLabel.text = _count.ToString();
        }

        public void UpdateRequestCount(int _count)
        {
            RequestCountLabel.text = _count.ToString();
        }

        public void UpdatePendingCount(int _count)
        {
            PendingCountLabel.text = _count.ToString();
        }*/
    }

    public enum GroupTabState
    {
        group,
        
        Search
    }
}
