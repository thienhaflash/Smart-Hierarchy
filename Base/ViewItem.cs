﻿using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using static UnityEditor.EditorGUIUtility;

namespace AV.Hierarchy
{
    internal class ViewItem
    {
        internal Rect rect;
        internal TreeViewItem view;
        
        internal readonly int id;
        internal readonly GameObject instance;
        internal readonly Transform transform;
        internal readonly Components components;
        internal readonly Texture2D icon;
        
        internal readonly ViewItem child;
        
        internal readonly bool isPrefab;
        internal readonly bool isRootPrefab;
        internal readonly bool isFolder;
        internal readonly bool isEmpty;

        private ObjectPreviewEditor previewEditor;
        
        private static Texture2D folderIcon = IconContent("Folder Icon").image as Texture2D;
        private static Texture2D folderEmptyIcon = IconContent("FolderEmpty Icon").image as Texture2D;

        public ViewItem(GameObject instance)
        {
            this.instance = instance;
            
            id = instance.GetInstanceID();
            
            transform = instance.transform;
            components = new Components(instance);
            
            icon = components.icon;

            isPrefab = PrefabUtility.GetPrefabAssetType(instance) == PrefabAssetType.Regular;
            isRootPrefab = PrefabUtility.IsAnyPrefabInstanceRoot(instance);
            isFolder = instance.TryGetComponent<Folder>(out _);
            isEmpty = instance.transform.childCount == 0;

            if (!isEmpty)
                child = new ViewItem(transform.GetChild(0).gameObject);
        }

        public ObjectPreviewEditor GetPreviewEditor()
        {
            if (previewEditor == null)
                previewEditor = new ObjectPreviewEditor(instance);
            return previewEditor;
        }
        
        public bool EnsureViewExist(SceneHierarchy hierarchy)
        {
            if (view == null)
            {
                view = hierarchy.GetViewItem(id);
                if(view == null)
                    return false;
            }

            return true;
        }
        
        public void UpdateViewIcon()
        {
            var preferences = HierarchySettingsProvider.Preferences;
            
            if (isFolder)
            {
                view.icon = instance.transform.childCount == 0 ? folderEmptyIcon : folderIcon;
            }
            else
            {
                if (icon == null) 
                    return;
                
                switch (preferences.stickyComponentIcon)
                {
                    case StickyIcon.Never: 
                        break;
                    case StickyIcon.OnAnyObject:
                        view.icon = icon;
                        break;
                    case StickyIcon.NotOnPrefabs:
                        if (!isRootPrefab)
                            view.icon = icon;
                        break;
                }
            }
        }
    }
}