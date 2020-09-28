# UniSceneProcessor

すべてのシーンに対して処理を行うクラス

## 使用例

### すべてのシーンに対して処理を行う

```cs
using Kogane;
using UnityEditor;
using UnityEngine;

public static class Example
{
    [MenuItem( "Tools/Hoge" )]
    private static void Hoge()
    {
        SceneProcessor.ProcessAllScenes
        (
            scene =>
            {
                // シーンに Cube を作成して保存
                GameObject.CreatePrimitive( PrimitiveType.Cube );
                return SceneProcessResult.CHANGE;
            }
        );
    }
}
```

### 特定のシーンに対して処理を行う

```cs
using Kogane;
using UnityEditor;
using UnityEngine;

public static class Example
{
    [MenuItem( "Tools/Hoge" )]
    private static void Hoge()
    {
        SceneProcessor.ProcessAllScenes
        (
            // 「Assets/@Project」フォルダ以下のシーンを対象にする
            scenePathFilter: scenePath => scenePath.StartsWith( "Assets/@Project" ),

            scene =>
            {
                // シーンに Cube を作成して保存
                GameObject.CreatePrimitive( PrimitiveType.Cube );
                return SceneProcessResult.CHANGE;
            }
        );
    }
}
```