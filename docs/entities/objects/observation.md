# Observation Entity

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

## DataType
<table style="width: 100%;">
    <thead>
        <tr>
            <th style="width: 40px;">ID</th>
            <th style="width: 100px;">Name</th>
            <th>Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>0</td>
            <td><b>String</b></td>
            <td>Text</td>
        </tr>
        <tr>
            <td>1</td>
            <td><b>Boolean</b></td>
            <td>1 bit True or False</td>
        </tr>
        <tr>
            <td>2</td>
            <td><b>Byte</b></td>
            <td>8 bit Signed Integer</td>
        </tr>
        <tr>
            <td>3</td>
            <td><b>Int16</b></td>
            <td>16 bit Signed Integer</td>
        </tr>
        <tr>
            <td>4</td>
            <td><b>Int32</b></td>
            <td>32 bit Signed Integer</td>
        </tr>
        <tr>
            <td>5</td>
            <td><b>Int64</b></td>
            <td>64 bit Signed Integer</td>
        </tr>
        <tr>
            <td>6</td>
            <td><b>Float</b></td>
            <td>32 bit Floating Point Integer</td>
        </tr>
        <tr>
            <td>7</td>
            <td><b>Double</b></td>
            <td>64 bit Floating Point Integer</td>
        </tr>
        <tr>
            <td>8</td>
            <td><b>Decimal</b></td>
            <td>128 bit numeric type for financial and monetary calculations</td>
        </tr>
        <tr>
            <td>9</td>
            <td><b>Duration</b></td>
            <td>64 bit Signed Integer representing the Nanoseconds that have elapsed</td>
        </tr>
        <tr>
            <td>10</td>
            <td><b>Reference</b></td>
            <td>64 bit string of the Object UUID that is referenced</td>
        </tr>
        <tr>
            <td>11</td>
            <td><b>Timestamp</b></td>
            <td>64 bit Signed Integer representing UNIX Nanoseconds</td>
        </tr>
        <tr>
            <td>12</td>
            <td><b>Vocabulary</b></td>
            <td>264 bit string of the Definition UUID that is referenced</td>
        </tr>
    </tbody>
</table>