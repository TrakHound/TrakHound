# API

API modules are used to create custom endpoint interfaces for external applications or other modules. APIs in TrakHound are meant to be similar to a typical HTTP REST API with a few key differences listed below:

- Internal Communications : When an API calls another API running on the same TrakHound Instance, no external HTTP call is made and the API call is processed as any other method call. This allows APIs to freely call one another and be built in layers without any significant overhead.

- Response : All API endpoints respond with a TrakHound API Response as shown below.

- HTTP Methods : Typical HTTP REST Methods of GET, PUT, POST, DELETE, etc. are replaced with Query, Subscribe, Publish, and Delete.


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