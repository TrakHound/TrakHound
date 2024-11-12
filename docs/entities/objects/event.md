# Event Entity
<table>
    <tbody>
        <tr>
            <td class="row-header"><b>Category</b></td>
            <td>Objects</td>
        </tr>
        <tr>
            <td class="row-header"><b>Class</b></td>
            <td>Event</td>
        </tr>
    </tbody>
</table>

Event Entities are used to store a reference to an Object Entity at a specified Timestamp.


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
            <td><b>TargetUuid</b></td>
            <td>String (64)</td>
            <td>The UUID of the Object that occurred at the Timestamp</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>Timestamp</b></td>
            <td>Integer (64)</td>
            <td>The timestamp in UNIX Nanoseconds of when the Target Object occurred.</td>
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
SHA256({ObjectUuid}:{Timestamp})
```

## JSON Format
```json
[
  {ObjectUuid},
  {TargetUuid},
  {Timestamp},
  {SourceUuid},
  {Created}
]
```