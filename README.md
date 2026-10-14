# tCOFELET Scenario Editor

A **Unity WebGL** visual authoring tool for creating, editing, validating and exporting 
tCOFELET cybersecurity serious game scenarios in JSON format.

## About

This editor is part of the **tCOFELET framework** — an extension of COFELET for 
developing immersive cybersecurity training scenarios. It allows non-programmer 
scenario designers to build complete game scenarios through a graphical interface 
(forms, dropdowns, visual editors) without writing JSON manually.

The output is a valid **SGTTX JSON** file, ready to be loaded into the game.

## Planned Modules

| Module | Description |
|--------|-------------|
| **Asset Store Browser** | View and create base asset templates |
| **Action Store Browser** | View and create base action templates |
| **Round Editor** | Create rounds and set metrics (AP, CS, BI, Budget, Deadline) |
| **Asset Instance Editor** | Place asset instances into rounds from store templates |
| **Action Assignment Editor** | Assign actions to assets |
| **Goal Builder** | Define round goals and evaluation criteria (SEFs) |
| **Trigger Builder** | Build typed triggers per TriggerType |
| **Watch Board Editor** | Manage scheduled tasks, threats, and flash news |
| **Timeline Editor** | Define time/phase-based scenario events |
| **Validation Panel** | Check referential integrity before export |
| **JSON Export / Import** | Load existing and export valid SGTTX JSON |

## Tech Stack

- **Engine:** Unity (WebGL build target)
- **Language:** C#
- **UI:** Unity UI Toolkit
- **Serialization:** Newtonsoft.Json (Json.NET for Unity)
- **Data Model:** tCOFELET DataModel (`DataModel.cs`)

## Project Structure
Assets/ ├── Scripts/ │ ├── DataModel/ # Core data classes and enums (DataModel.cs) │ ├── Editor/ # Editor UI logic (panels, forms) │ ├── Serialization/ # JSON load/export utilities │ └── Validation/ # Validation rules ├── UI/ │ ├── UXML/ # UI layout files │ └── USS/ # UI stylesheets └── Resources/ └── SampleScenarios/ # Reference JSON files (SGTTX-20)


## Status
**In Development** — Initial setup phase.
