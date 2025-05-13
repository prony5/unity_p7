# Unity P7 Logger Integration

A lightweight Unity package that provides seamless integration with the [P7 Baical](https://baical.net/) logging system for professional game logging solutions.

## Features

- 📊 **Multi-level logging**: Debug, Info, Warning, Error, and Critical levels
- ⚡ **High performance**: Asynchronous logging with minimal impact on main thread
- 🛠 **Editor integration**: Configure directly from Unity Inspector
- 🌐 **Network logging**: Real-time log streaming to P7 servers
- 📂 **Local fallback**: Auto-saves logs when network is unavailable
- 📈 **Metadata support**: Attach custom tags and context to logs

## Installation

### Via Git URL (Recommended)
1. Open your Unity project
2. Navigate to `Window > Package Manager`
3. Click `+ > Add package from git URL`
4. Enter:  
   `https://github.com/prony5/unity_p7.git`

### Via Manifest.json
Add to your `Packages/manifest.json`:
```json
"dependencies": {
  "com.prony5.p7logger": "https://github.com/prony5/unity_p7.git"
}
```

## Quick Start

1. **Add to Scene**:
   - Create empty GameObject
   - Add `P7/Client` component
   - [Optional] Add `P7/Telemetry`

2. **[Optional] Configure Telemetry** (in Inspector)

3. **Using in Code**:
   ```csharp
    using P7;
    public class YouScript : MonoBehaviour
    {
        public Telemetry telemetry;

        void Update()
        {
            if (telemetry != null && telemetry.Count > 2)
            {
                telemetry[0].Add(transform.position.x);
                telemetry[1].Add(transform.position.y);
                telemetry[2].Add(transform.position.z);
            }
        }
    }
   ```
## Requirements

| Component      | Requirement                         |
|----------------|-------------------------------------|
| Unity Version  | 2019.4 LTS or later                 |
| Baical Server  | v5.4+                               |
| Platforms      | Windows, Linux                      |

## Troubleshooting

[📄 Read P7.pdf](/Docs/P7.pdf)
