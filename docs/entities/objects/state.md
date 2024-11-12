# State Entity
<table>
    <tbody>
        <tr>
            <td class="row-header"><b>Category</b></td>
            <td>Objects</td>
        </tr>
        <tr>
            <td class="row-header"><b>Class</b></td>
            <td>State</td>
        </tr>
    </tbody>
</table>

State Entities are used to store a status of an Object at a given Timestamp. The TTL property is used to determine whether the status is still valid.


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
            <td><b>Value</b></td>
            <td>String (64)</td>
            <td>The UUID of the Definition Entity that describes the Status</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>TTL</b></td>
            <td>Integer (64)</td>
            <td>The duration in UNIX Nanoseconds of how long the value is valid for.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>Timestamp</b></td>
            <td>Integer (64)</td>
            <td>The timestamp in UNIX Nanoseconds of when the value was observed.</td>
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
  {Value},
  {TTL},
  {Timestamp},
  {SourceUuid},
  {Created}
]
```