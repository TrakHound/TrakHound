# Assignment Entity
<table>
    <tbody>
        <tr>
            <td class="row-header"><b>Category</b></td>
            <td>Objects</td>
        </tr>
        <tr>
            <td class="row-header"><b>Class</b></td>
            <td>Assignment</td>
        </tr>
    </tbody>
</table>

Assignment Entities are used to store temporary relationships between objects. Time is tracked from when the assignment was added to when it was removed.


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
            <td><b>AssigneeUuid</b></td>
            <td>String (64)</td>
            <td>The UUID of the Object this entity that represents what the Member Object is assigned to</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>MemberUuid</b></td>
            <td>String (64)</td>
            <td>The UUID of the Object this entity that represents the Member Object</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>AddTimestamp</b></td>
            <td>Integer (64)</td>
            <td>The timestamp in UNIX Nanoseconds of when the Member Object was added</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>AddSourceUuid</b></td>
            <td>String (64)</td>
            <td>The UUID of the Source Entity that added the Member Object</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>RemoveTimestamp</b></td>
            <td>Integer (64)</td>
            <td>The timestamp in UNIX Nanoseconds of when the Member Object was removed</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>RemoveSourceUuid</b></td>
            <td>String (64)</td>
            <td>The UUID of the Source Entity that removed the Member Object</td>
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
SHA256({AssigneeUuid}:{MemberUuid}:{AddTimestamp})
```

## JSON Format
```json
[
  {AssigneeUuid},
  {MemberUuid},
  {AddTimestamp},
  {AddSourceUuid},
  {RemoveTimestamp},
  {RemoveSourceUuid},
  {Created}
]
```