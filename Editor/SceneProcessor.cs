using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Kogane
{
	/// <summary>
	/// リザルトタイプ
	/// </summary>
	public enum SceneProcessResult
	{
		/// <summary>
		/// 変更していない（シーンを保存しない）
		/// </summary>
		NOT_CHANGE,

		/// <summary>
		/// 変更した（シーンを保存する）
		/// </summary>
		CHANGE,
	}

	/// <summary>
	/// すべてのシーンに対して処理を行うクラス
	/// </summary>
	public static class SceneProcessor
	{
		//================================================================================
		// 関数(static)
		//================================================================================
		/// <summary>
		/// すべてのシーンに対して処理を行います
		/// </summary>
		/// <param name="onProcess">シーンに対して処理を行うデリゲート</param>
		public static void ProcessAllScenes( Func<Scene, SceneProcessResult> onProcess )
		{
			ProcessAllScenes( null, onProcess );
		}

		/// <summary>
		/// すべてのシーンに対して処理を行います
		/// </summary>
		/// <param name="scenePathFilter">処理を行うシーンを絞り込むためのデリゲート</param>
		/// <param name="onProcess">シーンに対して処理を行うデリゲート</param>
		public static void ProcessAllScenes
		(
			Func<string, bool>              scenePathFilter,
			Func<Scene, SceneProcessResult> onProcess
		)
		{
			var sceneSetups = EditorSceneManager.GetSceneManagerSetup();

			// FindAssets は Packages フォルダも対象になっているので
			// Assets フォルダ以下のシーンのみを抽出
			var scenePaths = AssetDatabase
					.FindAssets( "t:scene" )
					.Select( x => AssetDatabase.GUIDToAssetPath( x ) )
					.Where( x => x.StartsWith( "Assets/" ) )
					.Where( x => scenePathFilter == null || scenePathFilter( x ) )
					.ToArray()
				;

			try
			{
				for ( var i = 0; i < scenePaths.Length; i++ )
				{
					var scenePath = scenePaths[ i ];
					var scene     = EditorSceneManager.OpenScene( scenePath );

					var isSave = onProcess( scene ) == SceneProcessResult.CHANGE;

					if ( isSave )
					{
						EditorSceneManager.MarkSceneDirty( scene );
						EditorSceneManager.SaveScene( scene );
					}
				}
			}
			finally
			{
				// Untitled なシーンは復元できず、SceneSetup[] の要素数が 0 になる
				// Untitled なシーンを復元しようとすると下記のエラーが発生するので if で確認
				// ArgumentException: Invalid SceneManagerSetup:
				if ( 0 < sceneSetups.Length )
				{
					EditorSceneManager.RestoreSceneManagerSetup( sceneSetups );
				}
			}
		}
	}
}