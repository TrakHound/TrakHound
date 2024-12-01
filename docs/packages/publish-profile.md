# Publish Profile
Publish profiles are used to define the Version to publish and Management Server to publish to.

## Format
```json
{
  "version": "1.0.5",
  "destinations": [
    {
      "managementServer": "https://www.trakhound.com/management",
      "organization": "Public"
    },
    {
      "managementServer": "http://localhost:8472",
      "organization": "Development"
    }
  ]
}
```