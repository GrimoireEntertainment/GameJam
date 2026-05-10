\# Codex Instructions



This is a Unity 6.3.10 project for a 36-hour game jam template.



\## Main goal



Create a universal, lightweight Unity jam template.



This project must help the team move faster during a game jam, but it must not become a ready-made game.



\## Scope



Work mainly inside:



\- Assets/Game/

\- Packages/

\- ProjectSettings/ only when explicitly required

Main project working folder is now `Assets/Game/`.



Do not inspect or edit unless explicitly requested:



\- Library/

\- Temp/

\- Obj/

\- Build/

\- Builds/

\- Logs/

\- UserSettings/

\- .vs/

\- .idea/

\- .vscode/

\- \*.csproj

\- \*.sln



\## Unity rules



\- Unity version: 6.3.10.

\- Do not manually edit `.meta` files.

\- Do not create gameplay-specific systems unless requested.

\- Do not add third-party dependencies unless requested.

\- Prefer small focused changes.

\- Do not change unrelated files.

\- After each task, list changed/created files.

\- Explain how to verify the result in Unity Editor.

\- Do not auto-create complex scenes unless explicitly requested.

\- Do not modify Build Settings unless the task explicitly requires it.



\## Allowed packages



These packages may be used when they simplify implementation:



\- DOTween

\- UniTask

\- Easy Save

\- SignalBus



Use them only when they clearly reduce complexity or speed up development.



Recommended use:



\- DOTween: UI animations, simple movement, fades, scale punch, transitions.

\- UniTask: async scene loading, timers, delays, lightweight async flows.

\- Easy Save: fast save/load for settings and simple game data.

\- SignalBus: simple decoupled events if direct references become messy.



Do not build unnecessary architecture around these packages.



\## Code style



\- Language: C#.

\- Namespace root: `Game`.

\- Private fields use leading underscore: `\_example`.

\- Do not use trailing underscore: `example\_`.

\- Serialized private fields should look like this:



```csharp

\[SerializeField] private float \_moveSpeed = 5f;

## Input System

- The project uses Unity New Input System.
- Do not use `UnityEngine.Input.GetKeyDown`, `GetKey`, `GetAxis`, or legacy Input API.
- Use `UnityEngine.InputSystem` APIs or Input Actions.

