# tCOFELET Scenario Editor

[![Unity](https://img.shields.io/badge/Unity-6000.4.6f1%20(Unity%206)-black?logo=unity)](https://unity.com/)
[![Platform](https://img.shields.io/badge/Platform-WebGL%20%7C%20Desktop%20Browser-blue)](#tech-stack)
[![Language](https://img.shields.io/badge/Language-C%23-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![Format](https://img.shields.io/badge/Data%20Format-SGTTX%20JSON-orange)](#data-format)

A **Unity WebGL** visual authoring tool for creating, viewing, editing, validating, and exporting **tCOFELET** cybersecurity serious game scenarios in SGTTX JSON format.

---

## 📌 About

This editor is part of the **tCOFELET framework** — an extension of the COFELET framework for developing immersive tabletop cybersecurity training scenarios. 

It provides an intuitive graphical interface (forms, responsive dropdowns, live list browsers) that enables non-programmer scenario authors to inspect and modify complex scenario logic without manually editing thousands of lines of raw JSON.

The editor runs natively inside modern web browsers via **WebAssembly (Wasm)**, requiring zero plugins or installation.

---

## ✨ Features Implemented

- [x] **Large Scenario Parser:** Seamlessly parses, validates, and serializes large, complex scenarios (e.g., `SGTTX-20.json` with 6,500+ lines) using tolerant enum mapping.
- [x] **Base Asset Store Module (`Panel_AssetStore`):**
  - Live list browsing of base asset templates.
  - Form editing: ID, Title, Caption, Description.
  - Schema-compliant dropdowns: `AssetType`, `OperationalState`, `SecurityState`.
  - Add new base assets and delete existing ones.
- [x] **Base Action Store Module (`Panel_ActionStore`):**
  - Live list browsing of base action templates.
  - Semantic triple editing: `Property`, `SubjectType`, `ObjectType`.
  - Resource cost controls: `ActionPoints` (AP) and `budget`.
  - Execution semantics: `actionRepeatMode` configuration.
  - Add and delete action templates.
- [x] **Step Rounds Module (`Panel_Rounds`):**
  - Inspection and editing of game rounds (e.g., Rounds 1–5).
  - Fine-tuning round metadata (`id`, `label`).
  - Configuration of critical round constraints and metrics:
    - **Action Points (AP)**
    - **Budget**
    - **Cybersecurity (CS)**
    - **Business Impact (BI)**
    - **Trust**
    - **Deadline**
- [x] **Goals Module (`Panel_Goals`):**
  - Definition and management of scenario win/progress goals.
  - Goal types, target evaluation round, and Simple Evaluation Function (SEF) threshold/target tracking.
- [x] **Integrity & Validation Engine (`ScenarioValidator`):**
  - Comprehensive referential integrity checks (missing metadata, duplicate IDs, orphaned goals, invalid round indices).
  - In-app interactive **Validation Modal** providing instant visual feedback (green pass / red issue list).
- [x] **WebGL Native File Downloader:**
  - Integrated JavaScript interop (`FileDownload.jslib`) triggering automatic browser downloads (`Exported_Scenario.json`) directly to the user's local machine.

---

## 🛠️ Tech Stack

- **Engine:** Unity 6 (`6000.4.6f1`)
- **Compilation Target:** WebGL (IL2CPP $\rightarrow$ WebAssembly via Emscripten)
- **UI System:** Unity UI (uGUI Canvas with TextMeshPro, 1920×1080 scalable presentation)
- **Serialization:** `Newtonsoft.Json` (Json.NET) with custom `TolerantEnumConverter`
- **Platform Interop:** JavaScript Plugin Bridge (`FileDownload.jslib`)

---

## 📂 Project Structure

```text
Assets/
├── Plugins/
│   └── WebGL/
│       └── FileDownload.jslib      # Native browser file download bridge
├── Prefabs/
│   └── AssetListItem.prefab        # Dynamic list item template for Assets/Actions
├── SampleScenarios/
│   ├── SGTTX-20.json               # Master baseline reference scenario
│   └── Exported_Scenario.json      # Output scenario from editor exports
├── Scenes/
│   └── SampleScene.unity           # Main authoring workstation scene
└── Scripts/
    ├── DataModel/
    │   └── DataModel.cs            # Complete tCOFELET C# data contracts & enums
    ├── AssetStoreController.cs     # Logic for Asset Store authoring UI
    ├── ActionStoreController.cs    # Logic for Action Store authoring UI
    ├── RoundsController.cs         # Logic for Step Rounds authoring UI
    ├── GoalsController.cs          # Logic for Goals authoring UI
    ├── ScenarioManager.cs          # Singleton state controller & JSON I/O
    ├── ScenarioValidator.cs        # Rule validation & referential integrity engine
    ├── UIManager.cs                # Navigation tab switcher & layout management
    └── ValidationModalController.cs# UI controller for validation reports
```

---

## 🚀 How to Run & Build

### 1. In Unity Editor
1. Clone this repository:
   ```bash
   git clone https://github.com/Dimalex7/tcofe-scenario-editor.git
   ```
2. Open the project in **Unity 6 (6000.4.6f1)**.
3. Open `Assets/Scenes/SampleScene.unity`.
4. Press **Play** ▶️ to test inside the Editor.

### 2. Building for WebGL
1. In Unity, go to **`File` $\rightarrow$ `Build Profiles`**.
2. Select the **`Web - Desktop - Development`** (or `Release`) profile.
3. Ensure `Scenes/SampleScene` is checked in the scene list.
4. Click **`Build And Run`** and select an output folder outside the repository (e.g., `Build_WebGL/`).
5. Unity will spin up a local server and open the live web app in your default browser.

---

## 📄 License & Attribution

Developed as part of university laboratory research on cybersecurity training games and the **tCOFELET** framework.
