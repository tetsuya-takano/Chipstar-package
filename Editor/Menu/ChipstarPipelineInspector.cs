using UnityEngine;
using UnityEditor;

namespace Chipstar
{
	[CustomEditor(typeof(ChipstarPipeline))]
	public class ChipstarPipelineInspector : Editor
	{
		public override void OnInspectorGUI()
		{
			DrawDefaultInspector();
			if(GUILayout.Button("Build"))
			{
				try
				{
					var buildTarget = EditorUserBuildSettings.activeBuildTarget;
					if (EditorUtility.DisplayDialog("!", "ビルドします\n" + buildTarget.ToString(), "Yes", "No"))
					{
						(target as ChipstarPipeline)?.Build(buildTarget);
					}
				}
				catch (System.Exception e)
				{
					Debug.LogError(e.ToString());
				}
				finally
				{
					GUIUtility.ExitGUI();
				}
			}
		}
	}
}