[![NuGet](https://img.shields.io/nuget/v/BloomEffectRenderer.svg?maxAge=2592000)](https://www.nuget.org/packages/BloomEffectRenderer/)
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



#### Example



[homepage]: http://www.unterrainer.info
[coding]: http://www.unterrainer.info/Home/Coding
[github]: https://github.com/UnterrainerInformatik/BloomEffectRenderer