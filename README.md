# Runtime Toolbox
A collection of diverse, handy runtime scripts.

## FixedUpdateInterpolation
This class interpolates any values in `Update` between two `FixedUpdate`s.

Can be used to remove stutter when using `FixedUpdate` to make non-linear processes framerate independent.

It's similar to the interpolate setting in a Rigidbody.

Example code can be found in the [snippets](https://github.com/tea-spoons/runtime-toolbox/-/snippets/170).

## RadioActivationGroup
This component has a list of GameObjects and allows for activating one of them, deactivating all others.

In the editor, selecting the GameObject with this component,any of the GameObjects in its list
or any of the descendants of one such GameObject will use the editor hiding feature
to allow for unhindered editing of otherwise overlapping objects.

For example, it can be used
- to create tabs within a UI dialog.
- for different states of a building prefab. 

## CachedTypeList
Automatically keeps a ScriptableObject asset up-to-date with every concrete type that derives from a given base class under `Assets/Resources/Generated`

Example:

```csharp
public sealed class AuthenticatorTypeList : CachedTypeList
{
    protected override bool IsValid(Type t)
    {
        return !t.IsAbstract
               && !t.ContainsGenericParameters
               && t.GetConstructor(Type.EmptyTypes) != null;
    }

    protected override IEnumerable<Type> GetTypes()
    {
        return FindDerivedTypes(typeof(Authenticator));
    }
}

//Runtime usage
var list = CachedTypeList.Load<AuthenticatorTypeList>();

foreach (var typeName in list.TypeNames)
{
    var t = Type.GetType(typeName);
}
```

## SmartEvents
A group of event classes (currently just one) that are smarter than default C# `event`s for certain use cases.

All `SmartEvent`s provide a `SmartEvent.Trigger` in their constructor that can be stored in a private field
next to the event itself in order to separate response registration from invocation access.

### InitializationEvent
An event that remembers if it has been invoked before.
If a response is added after the first invocation, the response will be invoked immediately in addition to being registered.

This can be used to react to the initialization of another object, without waiting for the event to be invoked if it had already been initialized.

## WrappedActions
Essentially what the name says, they are a wrapper around an action with a safe and easy to use interface.
They allow for an immediate response when subscribing.
There is also a `WrappedAction<T>` with the option of a parameterless callback which can come in handy sometimes for cleaner code, when a return parameter isn't needed for all cases.

## ObservableValue, PublicObservableValue
Generic classes that represent a value and come with an event that is invoked when that value changes.

- `ObservableValue`: Similar to `SmartEvent`s, the constructor provides a `ObservableValue<T>.UpdateTrigger`
that is used to split the accessibility of value updates and response registration.
- `PublicObservableValue`: Gives the `Value` property a `public` setter, so the value can be updated directly.

## InterfaceField
Allows serialization of interfaces. Works with fields and lists in the inspector.

## Other things
- `FloatRange`: A serializable struct for a range between a minimum and a maximum, with a prettier property drawer
and handy functions like `Clamp` or `Lerp`.

## Installation

In Unity: **Window > Package Manager > + > Add package from git URL**, then enter:

```
https://github.com/tea-spoons/runtime-toolbox.git
```

Pin a release by appending a tag, for example `#v0.11.1`.

### Dependencies

Unity cannot resolve git dependencies automatically, so add these to your project first:

- `com.tea-spoons.package-core` 1.4.0

## Change plan

See [CHANGE-PLAN.md](CHANGE-PLAN.md) for what changed before publishing and what is planned next.

## License

Copyright (c) 2026 Bigpoint. Authored by Muhammad Tarek Abdou.

Available for research, education and other noncommercial use under the [PolyForm Noncommercial 1.0.0](LICENSE.md)
license. Commercial use is not permitted.
