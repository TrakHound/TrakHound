# Blobs API
Blobs is a System Level API in TrakHound used to store raw bytes.

## Read
Blobs are read using an HTTP GET Request using the format below with a response that is always a File stream.

### HTTP Request
```
GET : http://localhost:8472/_blobs?blobId=0123456
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>blobId</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Blob to return</td>
        </tr>   
        <tr>
            <td>routerId</td>
            <td>String</td>
            <td>NO</td>
            <td>The Name or ID of the TrakHound Router to use to process the request</td>
        </tr>   
    </tbody>
</table>

## Publish
Blobs are written using an HTTP POST Request using the format below.

### HTTP Request
```
POST : http://localhost:8472/_blobs/publish?blobId=0123456
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>blobId</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Blob to return</td>
        </tr>   
        <tr>
            <td>routerId</td>
            <td>String</td>
            <td>NO</td>
            <td>The Name or ID of the TrakHound Router to use to process the request</td>
        </tr>   
    </tbody>
</table>

#### Request Body
The Request Body contains the raw bytes to store as the Blob Content.

## Delete
Blobs are deleted using an HTTP DELETE Request using the format below.

### HTTP Request
```
DELETE : http://localhost:8472/_blobs?blobId=0123456
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;width: 120px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>blobId</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Blob to return</td>
        </tr>   
        <tr>
            <td>routerId</td>
            <td>String</td>
            <td>NO</td>
            <td>The Name or ID of the TrakHound Router to use to process the request</td>
        </tr>   
    </tbody>
</table>