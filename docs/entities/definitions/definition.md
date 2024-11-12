# Definition Entity
<table>
    <tbody>
        <tr>
            <td class="row-header"><b>Category</b></td>
            <td>Definitions</td>
        </tr>
        <tr>
            <td class="row-header"><b>Class</b></td>
            <td>Definition</td>
        </tr>
    </tbody>
</table>

Definitions are used to assign a Type and provide a description


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
            <td>Uuid</td>
            <td>String</td>
            <td>The Globally Unique Identifier that identifies this specific Entity.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td>Id</td>
            <td>String</td>
            <td>The Identifier that identifies what this Definition represents.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td>Version</td>
            <td>String</td>
            <td>The version of the Definition.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td>Description</td>
            <td>String</td>
            <td>Text that describes the Type</td>
            <td>Required</td>
        </tr>
        <tr>
            <td>ParentUuid</td>
            <td>String</td>
            <td>The UUID of the Parent Definition Entity. This is used to inherit from another Definition.</td>
            <td>Optional</td>
        </tr>
        <tr>
            <td>TransactionUuid</td>
            <td>String</td>
            <td>The UUID of the Transaction Entity that created this Entity.</td>
            <td>Required</td>
        </tr>
        <tr>
            <td>Created</td>
            <td>Int64</td>
            <td>The timestamp in UNIX Nanoseconds of when the Entity was created.</td>
            <td>Required</td>
        </tr>
    </tbody>
</table>

## UUID
A Definition UUID property follows the below format:
```
SHA256({Id})
```

## JSON Format
```json
[{Id},{Version},{Description},{ParentUuid},{TransactionUuid},{Created}]
```