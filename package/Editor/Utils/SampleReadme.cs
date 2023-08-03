using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace Needle
{
	internal class SampleReadme : EditorWindow
	{
		[InitializeOnLoadMethod]
		private static void Init()
		{
			EditorSceneManager.sceneOpened += OnSceneOpened;
		}

		private static async void OnSceneOpened(Scene scene, OpenSceneMode mode)
		{
			await Task.Delay(200);
			if (string.IsNullOrEmpty(scene.path)) return;
			if (scene.path.Contains("com.needle.engine-samples") || scene.path.Contains("Samples/Needle"))
			{
				var dir = Path.GetDirectoryName(scene.path);
				if (dir != null)
				{
					var readme = Path.Combine(dir, "README.md");
					if (File.Exists(readme))
					{
						var window = GetWindow<SampleReadme>();
						if (!window)
							window = CreateInstance<SampleReadme>();
						window.filePath = readme;
						window.text = null;
						window.titleText = new DirectoryInfo(dir).Name + " — Needle Engine Sample";
						window.Show();
					}
				}
			}
		}

		private string titleText;
		private string filePath;
		private string text;
		private Vector2 scroll;

		private void OnGUI()
		{
			if (titleContent.text != titleText)
			{
				titleContent = new GUIContent(titleText, AssetDatabase.LoadAssetAtPath<Texture2D>(AssetDatabase.GUIDToAssetPath("39a802f6842d896498768ef6444afe6f")));
			}
			if (string.IsNullOrEmpty(text) && File.Exists(filePath))
				text = File.ReadAllText(filePath);
			if (string.IsNullOrEmpty(text))
			{
				this.Close();
			}
			using (var scrollview = new EditorGUILayout.ScrollViewScope(scroll))
			{
				scroll = scrollview.scrollPosition;
				EditorGUILayout.LabelField(text, EditorStyles.wordWrappedLabel);
			}
		}
	}
}