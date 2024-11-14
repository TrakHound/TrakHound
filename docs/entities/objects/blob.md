# Blob Entity

Blob Entities are used to store a reference to a Blob. Blobs are typically used to store a file.


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
            <td><b>BlobId</b></td>
            <td>String (255)</td>
            <td>The ID of the Blob. This references the Blob System API</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>ContentType</b></td>
            <td>String (255)</td>
            <td>The MIME type of the Blob content</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>Size</b></td>
            <td>Integer (64)</td>
            <td>The Size in Bytes of the Blob Content</td>
            <td>Required</td>
        </tr>
        <tr>
            <td><b>Filename</b></td>
            <td>String (255)</td>
            <td>The filename to use for the Blob when downloaded</td>
            <td>Optional</td>
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
SHA256({ObjectUuid}:blob)
```

## JSON Format
```json
[
  {ObjectUuid},
  {BlobId},
  {ContentType},
  {SourceUuid},
  {Created}
]
```