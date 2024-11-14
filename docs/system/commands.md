# Commands System API
Commands is a System Level API in TrakHound used to perform a custom request/response.

Commands are implemented through a Driver using the 'Commands.Run' route.

## Run
The Run endpoint is used to run the command and receive a response.

### HTTP Request
```
GET/POST : http://localhost:8472/_commands/run?commandId=c-123
```

<table style="width: 100%;">
    <thead>
        <tr>
            <th style="text-align: left;width: 120px;">Parameter</th>
            <th style="text-align: left;width: 100px;">DataType</th>
            <th style="text-align: left;width: 80px;">Required</th>
            <th style="text-align: left;">Description</th>
        </tr>
    </thead>
    <tbody>
        <tr>
            <td>commandId</td>
            <td>String</td>
            <td>YES</td>
            <td>The ID of the Command to run</td>
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
The Request Body message is used to send a Key/Value JSON object of the Parameters using the format below:
```json
{
    "Key": "Value",
    "Key": "Value",
    "Key": "Value"
}
```
