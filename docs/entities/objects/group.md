# Group Entity
<table>
    <tbody>
        <tr>
            <td class="row-header"><b>Category</b></td>
            <td>Objects</td>
        </tr>
        <tr>
            <td class="row-header"><b>Class</b></td>
            <td>Group</td>
        </tr>
    </tbody>
</table>

Group Entities are used to store references to multiple Objects.


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
            <td><b>GroupUuid</b></td>
            <td>String (64)</td>
            <td>The UUID of the Object represents the Group Object.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>MemberUuid</b></td>
            <td>String (64)</td>
            <td>The UUID of the Object represents the Member Object.</td>
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
SHA256({GroupUuid}:{MemberUuid}})
```

## JSON Format
```json
[
  {ObjectUuid},
  {MemberUuid},
  {SourceUuid},
  {Created}
]
```