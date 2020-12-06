# Custom Scriptable Renderer
This repository contains a template to create a custom scriptable renderer for URP. 
It contains a minimal / empty scriptable renderer ready to use with URP. 

## How to use
+ Create a custom renderer asset by clicking on `Assets -> Create -> Custom Renderer`.
+ Add the newly created custom renderer to the `Renderer List` in the current used URP pipeline asset.
+ You can use it by making it the Default renderer. This will make all cameras renderer using this renderer, or by overriding the renderer in the camera.
+ By default this custom renderer will not drawn anything. Game view will be drawn black. 
+ The custom renderer already work with Renderer Features out of the box. You can create or use existing renderer features and add then in the renderer inspector.

## How does it work

When creating a custom renderer for URP you have to implement a`ScriptableRendererData` and a `ScriptableRenderer`.
The ScriptableRendererData is responsible for creating the renderer runtime instance and to hold any resources required by it, f.ex, shaders, materials, renderer features.

The ScriptableRenderer implements a lighting strategy and builds a list of `ScriptableRenderPass` to be executed by the renderer.
Choosing the lighting strategy is key when designing a custom renderer feature. The lighting strategy will greatly impact the performance and the workflow of a graphics application renderer with this renderer. A few examples of lighting strategies commonly used are, classic forward renderer with per-object light classification, stencil deferred lights, tile/cluster renderer. 

### Creating a custom ScriptableRendererData

In order to create a custom renderer data we need to subclass `ScriptableRendererData` and implement `Create` method.
ScriptableRendererData already contains logic to handle and display `ScriptableRendererFeatures`. This makes it easier to extend our custom renderer.

```csharp
using UnityEngine;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(fileName = "CustomRenderer", menuName = "Create Custom Renderer", order = 50)]
public class CustomScriptableRendererData : ScriptableRendererData
{
    protected override ScriptableRenderer Create()
    {
        return new CustomScriptableRenderer(this);
    }
}
```

First we add a menu entry to Unity editor to create a renderer data. This renderer data is an asset that can be assigned to a URP renderer.
`[CreateAssetMenu(fileName = "CustomRenderer", menuName = "Create Custom Renderer", order = 50)]`

Then we implement `Create` by creating an instance of the `CustomScriptableRenderer` class. 

### Creating a custom `ScriptableRenderer`

Similarly to the custom renderer data, we have to subclass `ScriptableRenderer` to create our custom renderer runtime class.
A custom rendere mainly wants to override the following methods:

`Setup`: Builds and enqueue a list of render passes to be executed by the renderer. Some examples here could be, a depth prepass, draw opaque and transparent objects, draw post-processing. Render passes don't get executed by the renderer in the order they were enqueued, they get sorted by the render pass event and are executed in that order. If two or more render passes have the same event, the render pass that was enqueued first gets executed first. 

`SetupLights`: Override this method to implement the lighting setup for the renderer. You can use this to compute and upload light buffers to GPU for example.

`SetupCullingParameters`: Override this method to configure the culling parameters for the renderer. You can use this to configure if lights should be culled per-object or the maximum shadow distance for example.

```csharp
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomScriptableRenderer : ScriptableRenderer
{
    public CustomScriptableRenderer(ScriptableRendererData data) : base(data)
    {
    }

    public override void Setup(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // Calls AddRenderPasses for each renderer feature added to this renderer
        AddRenderPasses(ref renderingData);

        // Tell the pipeline the default render pipeline texture to use. When a scriptable render pass doesn't
        // set a render target using ConfigureTarget these render textures will be bound as color and depth by default.
        ConfigureCameraTarget(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget);
    }

    public override void SetupLights(ScriptableRenderContext context, ref RenderingData renderingData)
    {
    }

    public override void SetupCullingParameters(ref ScriptableCullingParameters cullingParameters, ref CameraData cameraData)
    {
    }
}
```

