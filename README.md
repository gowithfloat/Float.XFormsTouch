# Float.XFormsTouch [![No Maintenance Intended](http://unmaintained.tech/badge.svg)](http://unmaintained.tech/) [![Test](https://github.com/gowithfloat/Float.XFormsTouch/actions/workflows/test.yml/badge.svg)](https://github.com/gowithfloat/Float.XFormsTouch/actions/workflows/test.yml) [![NuGet](https://img.shields.io/nuget/v/Float.XFormsTouch)](https://www.nuget.org/packages/Float.XFormsTouch/)

# [DEPRECATED]

This NuGet library provides touch event notifications for Android and iOS via Xamarin.Forms.Effect mechanism as shown in one of the xamarin samples.

The code is copied from https://github.com/xamarin/xamarin-forms-samples/tree/master/Effects/TouchTrackingEffect and adapted to be used as a separate library.
Xamarin explains how it works at https://docs.microsoft.com/en-us/xamarin/xamarin-forms/app-fundamentals/effects/touch-tracking.

Original copyright is owned by Xamarin Inc (see LICENSE and NOTICE).

## Example

```csharp
public class App : Application
{
   public App()
   {
       var label = new Label()
       {
           Text = "Touch Me",
           Margin = new Thickness(20, 50),
       };

       var touchEffect = new TouchEffect();
       touchEffect.TouchAction += (s, e) => label.Text = e.Type.ToString("f");

       label.Effects.Add(touchEffect);

       MainPage = new ContentPage()
       {
           Content = label
       };
   }
}
```
## Building

This project can be built using [Visual Studio for Mac](https://visualstudio.microsoft.com/vs/mac/) or [Cake](https://cakebuild.net/). It is recommended that you build this project by invoking the boostrap script:

    ./build.sh

There are a number of optional arguments that can be provided to the bootstrapper that will be parsed and passed on to Cake itself. See the [Cake build file](./build.cake) in order to identify all supported parameters.

    ./build.sh \
        --task=Build \
        --projectName=Float.XFormsTouch \
        --configuration=Debug \
        --nugetUrl=https://nuget.org \
        --nugetToken=####

## License

All content in this repository is shared under an Apache 2.0 license. See [license.md](./license.md) for details.
