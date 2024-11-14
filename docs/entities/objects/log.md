# Log Entity

Log Entities are used to store log entries with a specified LogLevel at a given Timestamp.


## Properties
<table style="width: 100%;">
    <thead>
        <tr>
            <th>Name</th>
            <th>DataType</th>
            <th>Description</th>
            <th>Required</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td><b>Uuid</b></td>
            <td>String (64)</td>
            <td>The Globally Unique Identifier that identifies this specific Entity.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>ObjectUuid</b></td>
            <td>String (64)</td>
            <td>The UUID of the Object this entity is associated with.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>LogLevel</b></td>
            <td>Integer (8)</td>
            <td>The number representation of the Level that the message was logged at.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>Message</b></td>
            <td>String (5000)</td>
            <td>The message that was logged.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>Code</b></td>
            <td>String (500)</td>
            <td>The code that is associated with the message.</td>
            <td>Optional</td>
        </tr>
        <tr>
            <td><b>Timestamp</b></td>
            <td>Integer (64)</td>
            <td>The timestamp in UNIX Nanoseconds of when the message was recorded.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>SourceUuid</b></td>
            <td>String (64)</td>
            <td>The UUID of the Source Entity that created this Entity.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>Created</b></td>
            <td>Integer (64)</td>
            <td>The timestamp in UNIX Nanoseconds of when the Entity was created.</td>
            <td>Required</td>
        </tr>
    </tbody>
</table>

## UUID
A UUID property follows the below format:
```
SHA256({ObjectUuid}:{LogLevel}:{Message}:{Timestamp})
```

## JSON Format
```json
[
  {ObjectUuid},
  {LogLevel},
  {Message},
  {Code},
  {Timestamp},
  {SourceUuid},
  {Created}
]
```