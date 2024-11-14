# API

API modules are used to create custom endpoint interfaces for external applications or other modules. APIs in TrakHound are meant to be similar to a typical HTTP REST API with a few key differences listed below:

- Internal Communications : When an API calls another API running on the same TrakHound Instance, no external HTTP call is made and the API call is processed as any other method call. This allows APIs to freely call one another and be built in layers without any significant overhead.

- Response : All API endpoints respond with a TrakHound API Response as shown below.

- HTTP Methods : Typical HTTP REST Methods of GET, PUT, POST, DELETE, etc. are replaced with Query, Subscribe, Publish, and Delete.


## Endpoints
API Endpoints can have one of 4 types, Query, Subscribe, Publish, or Delete. These endpoint types were chosen instead of the standard HTTP REST methods of GET, PUT, POST, DELETE, etc. so that each endpont type may use a POST method and so that subscriptions can have specialized endpoints.

### Query
Query endpoints are used to return data in a single request. Query requests can use either the GET or POST HTTP methods.

```
GET : http://localhost:8472/entities/objects?path=/
```

### Subscribe
Subscribe endpoints are used to establish a long running WebSocket connection to receive streaming data. Subscribe endpoints must end with the "/subscribe" suffix.

```
ws:/localhost:8472/entities/objects/observation/subscribe?path=*
```

### Publish
Publish endpoints are used to write data in a single request. Publish endpoints can use either the PUT or POST HTTP methods. Publish endpoints must end with the "/publish" suffix.

```
PUT : http://localhost:8472/entities/objects/timestamp/publish?path=/Test/Timestamp&value=now
```

### Delete
Delete endpoints are used to remove data in a single request. Delete endpoints can use either the DELETE or POST HTTP methods. Delete endpoints must end with the "/delete" suffix.

```
DELETE : http://localhost:8472/entities/objects/delete?path=/Test/String
```

## Response
<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Property</th>
            <th style="text-align: left;width: 170px;">DataType</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>StatusCode</td>
            <td>Integer (32)</td>
            <td>A numeric code to represent the status of the response. Typically is used similar to an HTTP Status Code</td>
        </tr>  
        <tr>
            <td>Parameters</td>
            <td>Dictionary&lt;string, string&gt;</td>
            <td>A list of key-value pairs that are sent as headers/parameters with the response</td>
        </tr>       
        <tr>
            <td>ContentType</td>
            <td>String (255)</td>
            <td>The MIME Type of the Content property</td>
        </tr>       
        <tr>
            <td>Content</td>
            <td>Byte[]</td>
            <td>The content of the response in raw bytes</td>
        </tr>     
    </tbody>
</table>