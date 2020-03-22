using UnityEngine;
using UnityEditor;

// This window script is provided by user AlexanderAmeye.
//you can find the Github Repo here https://github.com/AlexanderAmeye/asset-store-support-window
//Please consider Alexander's assets on the Asset Store as well https://assetstore.unity.com/publishers/4931 

namespace PublisherSupportWindow

{
    public class SupportWindow : EditorWindow
    {
        GUIStyle PublisherNameStyle;
        GUIStyle ToolBarStyle;
        int ToolBarIndex;
        GUIContent[] toolbarOptions;
        GUILayoutOption ToolbarHeight;
        GUIStyle GreyText;
        GUIStyle CenteredVersionLabel;
        GUIStyle ReviewBanner;
        GUILayoutOption BannerHeight;

		
        bool StylesNotLoaded = true;

        [MenuItem("Tools/Volume 1: Yggdrasil")]
        static void ShowWindow()
        {
            SupportWindow myWindow = GetWindow<SupportWindow>("About");
            myWindow.minSize = new Vector2(600, 400);
			//Adjust window size
            myWindow.maxSize = myWindow.minSize;
            myWindow.titleContent = new GUIContent("About");
            myWindow.Show();
        }

        void OnEnable()
        {
            toolbarOptions = new GUIContent[2];
            toolbarOptions[1] = new GUIContent("<size=11><b> Check Out More</b></size>\n <size=11>See what else is \n going on with Buried Memories.</size>", EditorGUIUtility.Load("Assets/" + "support" + ".png") as Texture2D, "");
            toolbarOptions[0] = new GUIContent("<size=11><b> Support</b></size>\n <size=11>Get help and talk \n with others.</size>", EditorGUIUtility.Load("Assets/" + "contact" + ".png") as Texture2D, "");
            ToolbarHeight = GUILayout.Height(50);
            BannerHeight = GUILayout.Height(30);
        }

        void LoadStyles()
        {
            PublisherNameStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            ToolBarStyle = new GUIStyle("LargeButtonMid")
            {
                alignment = TextAnchor.MiddleLeft,
                richText = true
            };

            GreyText = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleLeft
            };

            CenteredVersionLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel)
            {
                alignment = TextAnchor.MiddleCenter,
            };

            ReviewBanner = new GUIStyle("TL SelectionButton")
            {
                alignment = TextAnchor.MiddleCenter,
                richText = true
            };
            StylesNotLoaded = false;
		
        }

        void OnGUI()
        {
            if (StylesNotLoaded) LoadStyles();

            EditorGUILayout.Space();
            GUILayout.Label(new GUIContent("<size=20><b><color=#666666>Buried Memories Volume 1: Yggdrasil - Icon Pack</color></b></size>"), PublisherNameStyle);
            //Place the Title of the Asset above
			EditorGUILayout.Space();

            ToolBarIndex = GUILayout.Toolbar(ToolBarIndex, toolbarOptions, ToolBarStyle, ToolbarHeight);

            switch (ToolBarIndex)
            {
                case 1:
				EditorGUILayout.Space();
                    if (GUILayout.Button("Unity Icon Collective", EditorStyles.label))
                        Application.OpenURL("https://assetstore.unity.com/publishers/39449");
                    EditorGUILayout.LabelField("High quality art packages from top industry talent. ", GreyText);
					
                    EditorGUILayout.Space();
                    if (GUILayout.Button("YouTube", EditorStyles.label))
                        Application.OpenURL("https://youtu.be/1PuGuqpHQGo");
                    EditorGUILayout.LabelField("Watch the story teaser here. ", GreyText);
					
					EditorGUILayout.Space();
                    if (GUILayout.Button("Blog Announcement", EditorStyles.label))
                        Application.OpenURL("https://blogs.unity3d.com/2018/11/15/create-iconic-worlds-with-high-end-art-content-from-the-unity-icon-collective/");
                    EditorGUILayout.LabelField("Introducing the Unity Icon Collective. ", GreyText);
                    break;

                case 0:
					EditorGUILayout.Space();
                    if (GUILayout.Button("Support Forum", EditorStyles.label))
                        Application.OpenURL("https://forum.unity.com/threads/unity-icon-collective-buried-memories-volume-1-yggdrasil.601651/");
                    EditorGUILayout.LabelField("Get help from others or leave some feedback.", GreyText);

                    EditorGUILayout.Space();
                    if (GUILayout.Button("FAQ", EditorStyles.label))
						Application.OpenURL("https://support.unity3d.com/hc/en-us/sections/360002716372-Icon-Collective");
                    EditorGUILayout.LabelField("Official FAQ on the Icon Collective.", GreyText);
                    break;
                default: break;
            }

            GUILayout.FlexibleSpace();
            EditorGUILayout.LabelField(new GUIContent("Version 1.2.1"), CenteredVersionLabel);
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent("<size=11> Please consider leaving us a review.</size>", EditorGUIUtility.Load("Assets/UnityIconCollective/Volume01_Yggdrasil/Scripts/Editor" + "Rate" + ".png") as Texture2D, ""), ReviewBanner, BannerHeight))
                Application.OpenURL("https://assetstore.unity.com/packages/templates/packs/buried-memories-volume-1-yggdrasil-icon-pack-131448");
			//Asset Page or review page
			
        }
    }
}
