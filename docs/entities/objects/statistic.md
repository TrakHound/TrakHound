# Statistic Entity

Statistic Entities are used to store a value for a specific Time Range. This can be used to record aggregate data such as Min, Max, Average, etc.


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
            <td><b>TimeRangeStart</b></td>
            <td>Integer (64)</td>
            <td>The timestamp in UNIX Nanoseconds of the Start of the TimeRange.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>TimeRangeEnd</b></td>
            <td>Integer (64)</td>
            <td>The timestamp in UNIX Nanoseconds of the End of the TimeRange.</td>
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
            <td>The value that was recorded.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>Timestamp</b></td>
            <td>Integer (64)</td>
            <td>The timestamp in UNIX Nanoseconds of when the value was recorded.</td>
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
SHA256({ObjectUuid}:{TimeRangeStart}:{TimesRangeEnd})
```

## JSON Format
```json
[
  {ObjectUuid},
  {TimeRangeStart},
  {TimesRangeEnd},
  {DataType},
  {Value},
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
            <td><b>Byte</b></td>
            <td>8 bit Signed Integer</td>
        </tr>
        <tr>
            <td>1</td>
            <td><b>Int16</b></td>
            <td>16 bit Signed Integer</td>
        </tr>
        <tr>
            <td>2</td>
            <td><b>Int32</b></td>
            <td>32 bit Signed Integer</td>
        </tr>
        <tr>
            <td>3</td>
            <td><b>Int64</b></td>
            <td>64 bit Signed Integer</td>
        </tr>
        <tr>
            <td>4</td>
            <td><b>Float</b></td>
            <td>32 bit Floating Point Integer</td>
        </tr>
        <tr>
            <td>5</td>
            <td><b>Double</b></td>
            <td>64 bit Floating Point Integer</td>
        </tr>
        <tr>
            <td>6</td>
            <td><b>Decimal</b></td>
            <td>128 bit numeric type for financial and monetary calculations</td>
        </tr>
        <tr>
            <td>7</td>
            <td><b>Duration</b></td>
            <td>64 bit Signed Integer representing the Nanoseconds that have elapsed</td>
        </tr>
    </tbody>
</table>