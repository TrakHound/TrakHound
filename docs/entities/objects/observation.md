# Observation Entity
<table>
    <tbody>
        <tr>
            <td class="row-header"><b>Category</b></td>
            <td>Objects</td>
        </tr>
        <tr>
            <td class="row-header"><b>Class</b></td>
            <td>Observation</td>
        </tr>
    </tbody>
</table>

Observation Entities are used to store a Value at a given Timestamp. Observations contain information to verify the order using the BatchId and Sequence properties.


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
            <td><b>DataType</b></td>
            <td>Integer (8)</td>
            <td>The identifier to determine what type of data the Value contains</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>Value</b></td>
            <td>String</td>
            <td>The value that was observed</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>BatchId</b></td>
            <td>Integer (64)</td>
            <td>An identifier used to associate multiple observations together</td>
            <td>Optional</td>
        </tr>
        <tr>
            <td><b>Sequence</b></td>
            <td>Integer (64)</td>
            <td>A sequential number to determine the order that observations were made with the same BatchId</td>
            <td>Optional</td>
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
  {DataType},
  {BatchId},
  {Sequence},
  {Timestamp},
  {SourceUuid},
  {Created}
]
```