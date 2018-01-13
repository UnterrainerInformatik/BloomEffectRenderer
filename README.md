[![NuGet](https://img.shields.io/nuget/v/BloomEffectRenderer.svg)](https://www.nuget.org/packages/BloomEffectRenderer/) [![NuGet](https://img.shields.io/nuget/dt/BloomEffectRenderer.svg)](https://www.nuget.org/packages/BloomEffectRenderer/)
 [![license](https://img.shields.io/github/license/unterrainerinformatik/BloomEffectRenderer.svg?maxAge=2592000)](http://unlicense.org)  [![Twitter Follow](https://img.shields.io/twitter/follow/throbax.svg?style=social&label=Follow&maxAge=2592000)](https://twitter.com/throbax)  

# General

This section contains various useful projects that should help your development-process.  

This section of our GIT repositories is free. You may copy, use or rewrite every single one of its contained projects to your hearts content.  
In order to get help with basic GIT commands you may try [the GIT cheat-sheet][coding] on our [homepage][homepage].  

This repository located on our  [homepage][homepage] is private since this is the master- and release-branch. You may clone it, but it will be read-only.  
If you want to contribute to our repository (push, open pull requests), please use the copy on github located here: [the public github repository][github]  

# ![Icon](https://github.com/UnterrainerInformatik/BloomEffectRenderer/raw/master/icon.png) BloomEffectRenderer

This class is a PCL library for MonoGame that implements a bloom effect.  
If you'd like your game to truly shine, you can use this little beauty without much hassle.  
The problem with that brute-force-approach is, that the number of checks grow very fast (N² for N sprites)   

> **If you like this repo, please don't forget to star it.**
> **Thank you.**



## Getting Started

This project is based on the following blog posts:

* http://xbox.create.msdn.com/en-US/education/catalog/sample/bloom
* https://digitalerr0r.wordpress.com/2009/10/04/xna-shader-programming-tutorial-23-blur-post-process/
* https://digitalerr0r.wordpress.com/2009/10/04/xna-shader-programming-tutorial-24-bloom/

You don't have to mess with shaders since those are included in the distribution.



### Attributions

Thx to the [MonoGame.Extended](https://github.com/craftworkgames/MonoGame.Extended) project for their excellent work and teaching me how to embed shaders into a DLL.

Thx to [Jjagg](https://github.com/Jjagg) for reviewing the shaders.



### Example

```c#
private Point Resolution { get; } = new Point(1920, 1080);
private Renderer Renderer { get; } = new Renderer();

protected override void LoadContent()
{
  ...
  Renderer.LoadContent(GraphicsDevice);
  ...
}

protected override void UnloadContent()
{
  ...
  Renderer?.UnloadContent();
  ...
}

protected override void Initialize()
{
  base.Initialize();
  Renderer.Initialize(graphics.GraphicsDevice, Resolution);
}

protected override void Draw(GameTime gameTime)
{
  // Image is some Texture2D that will be drawn to the backbuffer in this example.
  // (hence the <null>).
  // To bloom your game in a post-process step, draw all your assets to a
  // RenderTarget2D first and then pass that rendertarget to this method.
  Renderer.Render(graphics.GraphicsDevice, spriteBatch, "image", Image, null, Settings.PRESET_SETTINGS[1]);
  base.Draw(gameTime);
}
```

### Debugging

You may access any of the intermediate images produced by the renderer.

For that you'll have to pass a delegate when calling `Render()` like so:

```C#
// Your delegate. This one saves the images with some unique name, but you
// could also render them directly onto another RenderTarget, for example.
private void BloomDebugDelegate(string name, RenderTarget2D t, RenderPhase phase)
{
  FileStream fs = new FileStream($"{((int)phase)+1}{name}_{phase}.png", FileMode.OpenOrCreate);
  t.SaveAsPng(fs, texture2D.Width, texture2D.Height);
  fs.Flush();
  fs.Close();
}

// And call the renderer like so:
Renderer.Render(graphics.GraphicsDevice, spriteBatch, "image", Image, null, Settings.PRESET_SETTINGS[1], BloomDebugDelegate);
```

This will call your delegate once for every render-phase.

```C#
[PublicAPI]
public enum RenderPhase
{
  /// <summary>
  /// The original texture is processed and all values above
  /// the bloomthreshold are kept.
  /// </summary>
  EXTRACT,
  /// <summary>
  /// The extract-texture is blured horizontally via a gaussian
  /// blur and resized to half the size.
  /// </summary>
  BLUR_HORIZONTAL,
  /// <summary>
  /// The horizontally blurred texture is blurred again 
  /// vertically (size is kept at half).
  /// </summary>
  BLUR_VERTICAL,
  /// <summary>
  /// This step re-combines the original texture and the 
  /// two-times-blurred texture to a new image.
  /// </summary>
  COMBINE
}
```



## TestGame

A test-project is included. You can manipulate the values of the shaders directly and see what happens.

Here are a few screenshots:

![Bloom Off](https://github.com/UnterrainerInformatik/BloomEffectRenderer/raw/master/bloom_off.png)

 ![Bloom 1](https://github.com/UnterrainerInformatik/BloomEffectRenderer/raw/master/bloom_1.png)

![Bloom 2](https://github.com/UnterrainerInformatik/BloomEffectRenderer/raw/master/bloom_2.png)

 ![Bloom 3](https://github.com/UnterrainerInformatik/BloomEffectRenderer/raw/master/bloom_3.png)

  

# References

- http://xbox.create.msdn.com/en-US/education/catalog/sample/bloom
- https://digitalerr0r.wordpress.com/2009/10/04/xna-shader-programming-tutorial-23-blur-post-process/
- https://digitalerr0r.wordpress.com/2009/10/04/xna-shader-programming-tutorial-24-bloom/
- [MG.EX](https://github.com/craftworkgames/MonoGame.Extended)

[homepage]: http://www.unterrainer.info
[coding]: http://www.unterrainer.info/Home/Coding
[github]: https://github.com/UnterrainerInformatik/BloomEffectRenderer
