using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "CustomRenderer", menuName = "Create Custom Renderer", order = 50)]
public class CustomScriptableRendererAsset : ScriptableRendererData
{
    protected override ScriptableRenderer Create()
    {
        return new CustomScriptableRenderer(this);
    }
}
