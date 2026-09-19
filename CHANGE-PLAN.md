# Change plan

> Draft. This file tracks what changed on the way to this repo and what I plan to change next. Edit freely.

## Origin

Originally developed at Bigpoint. Published here with Bigpoint's permission for research, education and other noncommercial use. Copyright (c) 2026 Bigpoint; see [LICENSE.md](LICENSE.md).

## Changes made before publishing

- Namespaces are now `TeaSpoons.*` and the package id is `com.tea-spoons.runtime-toolbox` (assemblies renamed to match).
- Internal build, registry and tracker references were removed; the repo uses GitHub Actions (`CI` and `Release`) built on `unity-ci-kit`.
- Added `LICENSE.md` (PolyForm Noncommercial 1.0.0), an install section in the README, and package metadata (author, license and documentation URLs).

## Planned changes

- [x] Tag and publish `v0.11.1` with the Release workflow.
- [ ] Run this package's tests in CI with `unity-ci-kit` (needs a small test-project helper in the kit).
- [ ] Make installs resolve dependencies automatically, for example through a registry such as OpenUPM.
<!-- review-items:start -->
- [ ] **P0** `package.json` has no `unity` field. Set it to the lowest Unity version that is actually tested (only 6000.3.8f1 was tested in this review).
- [ ] **P1** Use `EqualityComparer<T>.Default` (optionally a comparer argument) and test it with a struct that counts `Equals` calls.
- [ ] **P1** Add EditMode tests for `ObservableValue`, `SmartEvent`, `RadioActivationGroup`, `FloatRange` and `FixedUpdateInterpolation`.
- [ ] **P1** Decide how a throwing subscriber is handled (catch and log per subscriber, or document that it propagates) and test it.
- [ ] **P2** Return an `IDisposable` from `AddUpdatedResponse` so unsubscribing is harder to forget (the Rx style that R3 uses).
- [ ] **P2** Document the `CachedTypeList` generation step, and run it from a build preprocessor so a build cannot ship without it.
- [ ] **P2** Make the package-core dependency optional.
- [ ] **P2** Add a `CHANGELOG.md`. Unity's package layout lists one next to `README.md`, and the `unity-ci-kit` validator warns without it.
<!-- review-items:end -->

<!-- review:start -->
## Review (September 2026)

Reviewed as a senior Unity engineer would: I read the code and compared the package with similar open-source projects (September 2026). Those projects are listed for ideas only. Nothing was copied from them, and their licenses are noted in case code is ever reused. Priorities: **P0** correctness bug or broken metadata, **P1** should be done soon, **P2** nice to have.

### Compared with

| Project | License | Worth noting |
|---|---|---|
| [Cysharp/R3](https://github.com/Cysharp/R3) | not checked | Successor of UniRx. `ReactiveProperty` is a subject that also drops duplicate values; there is a serializable variant for the Inspector. |

### Findings from reading the code

- **[Perf]** `ObservableValue<T>.SetValue` compares with `Equals(value, this.value)` (`ObservableValue.cs`), which boxes value types on every set. `EqualityComparer<T>.Default` avoids that.
- **[Robustness]** `updated.Invoke(value)` runs the subscribers in one go. A subscriber that throws stops the ones after it, and the value has already changed.
- **[Resources]** `CachedTypeList.Load<T>` reads `Resources.Load("Generated/<Type>")` and, in a player, `FindDerivedTypes` returns nothing. It only works when the editor generated the asset first. That step is not documented or enforced at build time.
- **[Tests]** 20 test lines for about 1,100 lines of code, and `package.json` has no `unity` field.
<!-- review:end -->

## Notes and ideas

_Add your own here._
